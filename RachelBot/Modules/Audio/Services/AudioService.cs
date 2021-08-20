using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using RachelBot.Core.Configs;
using RachelBot.Lang;
using RachelBot.Modules.Audio.Objects;
using RachelBot.Services.Storage;
using SharpLink;

namespace RachelBot.Modules.Audio.Services
{
    public class AudioService
    {
        private const int MaxTracksInQueue = 15;
        private static readonly DiscordSocketClient _client = new DiscordSocketClient();
        public static LavalinkManager lavalinkManager = new LavalinkManager(_client);

        public static async Task PlayAsync(SocketCommandContext Context, string song)
        {
            try
            {
                SocketGuild guild = Context.Guild;
                SocketUserMessage message = Context.Message;
                IVoiceChannel voiceChannel = (Context.User as IVoiceState).VoiceChannel;
                ISocketMessageChannel channel = Context.Channel;
                GuildConfig config = new GuildConfigs(guild.Id, storage: new JsonStorageService()).GetGuildConfig();
                AlertsHandler alerts = new AlertsHandler(config);

                if (await VoiceChannelIsNull(channel, voiceChannel, alerts) is true) return;
                LavalinkPlayer player = lavalinkManager.GetPlayer(guild.Id) ?? await lavalinkManager.JoinAsync(voiceChannel);
                AudioQueue audioQueue = AudioQueueService.GetAudioQueue(guild);
                if (await SongIsEmpty(channel, player, audioQueue, song, alerts) is true) return;

                LoadTracksResponse response = await lavalinkManager.GetTracksAsync($"ytsearch:{song}");
                LavalinkTrack track = response.Tracks.First();

                // Maximum songs in queue is 15
                if (await QueueIsFull(channel, audioQueue.Queue.Count, alerts) is true) return;

                if (audioQueue.Queue.Count == 0)
                {
                    await player.PlayAsync(track);
                }

                string alertKey = (audioQueue.Queue.Count == 0) ? "NEXT_TRACK" : "TRACK_ADDED_TO_QUEUE";

                await channel.SendMessageAsync(alerts.GetFormattedAlert(alertKey, track.Title, track.Length, track.Url));

                audioQueue.Queue = AudioQueueService.GetOrCreateGuildQueue(track, audioQueue, channel.Id);

                if (await SongIsFirst(player, audioQueue, track) is false) return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

        internal static void DeleteQueues()
        {
            string filePath = "./Resources/AudioQueues.json";

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private static async Task<bool> VoiceChannelIsNull(ISocketMessageChannel channel, IVoiceChannel voiceChannel, AlertsHandler alerts)
        {
            if (voiceChannel is null)
            {
                await channel.SendMessageAsync(alerts.GetAlert("UNKNOWN_CHANNEL"));
                return true;
            }

            return false;
        }

        private static async Task<bool> SongIsEmpty(ISocketMessageChannel channel,
                                                    LavalinkPlayer player,
                                                    AudioQueue audioQueue,
                                                    string song,
                                                    AlertsHandler alerts)
        {
            if (song == "" && player.Playing)
            {
                return true;
            }

            if (song == "" && !player.Playing)
            {
                if (player.CurrentTrack is null)
                {
                    await channel.SendMessageAsync(alerts.GetAlert("UNKNOWN_TRACK"));
                    return true;
                }

                if (audioQueue.Queue.FirstOrDefault() is not null)
                {
                    await player.ResumeAsync();
                }

                return true;
            }

            return false;
        }

        private static async Task<bool> QueueIsFull(ISocketMessageChannel channel, int queueCount, AlertsHandler alerts)
        {
            if (queueCount > MaxTracksInQueue)
            {
                await channel.SendMessageAsync(alerts.GetFormattedAlert("QUEUE_IS_FULL", MaxTracksInQueue));
                return true;
            }
            return false;
        }

        private static async Task<bool> SongIsFirst(LavalinkPlayer player, AudioQueue audioQueue, LavalinkTrack track)
        {
            AudioQueueItem secondTrack = audioQueue.Queue.ElementAtOrDefault(1);

            if (secondTrack is null)
            {
                audioQueue.PlayingTrackIndex = 0;
                AudioQueueService.SaveQueues();

                await player.PlayAsync(track);

                return true;
            }

            return false;
        }

        public static async Task LeaveAsync(SocketGuild guild)
        {
            LavalinkPlayer player = lavalinkManager.GetPlayer(guild.Id);
            if (player is null) return;

            await lavalinkManager.LeaveAsync(guild.Id);
            await RemoveAllTracks(guild);
        }

        private static async Task RemoveAllTracks(SocketGuild guild)
        {
            AudioQueue audioQueue = AudioQueueService.GetAudioQueue(guild);
            audioQueue.Queue.Clear();
            AudioQueueService.SaveQueues();
            await Task.CompletedTask;
        }

        public static async Task StopAsync(ulong guildId)
        {
            LavalinkPlayer player = lavalinkManager.GetPlayer(guildId);
            if (!player.Playing) return;

            await player.PauseAsync();
        }

        public static async Task SkipAsync(SocketGuild guild)
        {
            LavalinkPlayer player = lavalinkManager.GetPlayer(guild.Id);
            await AudioQueuesManagment.RemoveAndPlay(player, player.CurrentTrack);
        }
    }
}
