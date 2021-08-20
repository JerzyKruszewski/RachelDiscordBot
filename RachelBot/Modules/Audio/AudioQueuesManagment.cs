using Discord.WebSocket;
using RachelBot.Core.Configs;
using RachelBot.Lang;
using RachelBot.Modules.Audio.Objects;
using RachelBot.Modules.Audio.Services;
using RachelBot.Services.Storage;
using SharpLink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Modules.Audio
{
    public class AudioQueuesManagment
    {
        public static async Task LavalinkManager_TrackEnd(LavalinkPlayer arg1, LavalinkTrack arg2, string arg3)
        {
            if (arg3 == "FINISHED")
            {
                await RemoveAndPlay(arg1, arg2);
            }
            return;
        }

        public static async Task RemoveAndPlay(LavalinkPlayer player, LavalinkTrack track)
        {
            AudioQueue audioQueue = AudioQueueService.GetAudioQueue(player.VoiceChannel.Guild as SocketGuild);

            RemoveTrack(audioQueue, track);
            await player.StopAsync();
            await PlayNextAfterRemove(audioQueue, player);
        }

        private static void RemoveTrack(AudioQueue audioQueue, LavalinkTrack track)
        {
            audioQueue.Queue.Remove(audioQueue.Queue[audioQueue.PlayingTrackIndex]);
            AudioQueueService.SaveQueues();
            return;
        }

        private static async Task PlayNextAfterRemove(AudioQueue audioQueue, LavalinkPlayer player)
        {
            AudioQueueItem queueItem = audioQueue.Queue.ElementAtOrDefault(0);

            if (queueItem is not null)
            {
                audioQueue.PlayingTrackIndex = 0;
                AudioQueueService.SaveQueues();

                LavalinkTrack track = queueItem.Track;

                GuildConfig config = new GuildConfigs(player.VoiceChannel.Guild.Id, storage: new JsonStorageService()).GetGuildConfig();
                AlertsHandler alerts = new AlertsHandler(config);

                ISocketMessageChannel channel = (ISocketMessageChannel)await player.VoiceChannel.Guild.GetTextChannelAsync(queueItem.RequestedInChannelId);
                await channel.SendMessageAsync(alerts.GetFormattedAlert("NEXT_TRACK", track.Title, track.Length, track.Url));

                await player.PlayAsync(track);
            }

            await Task.CompletedTask;
        }

        public static List<string> CreateListOfSongs(List<AudioQueueItem> queue)
        {
            List<string> videosList = new List<string>();
            int count = 1;

            foreach (AudioQueueItem item in queue)
            {
                videosList.Add($"{count}. [{item.Track.Title}]({item.Track.Url}) `{item.Track.Length}` \n");
                count++;
            }

            return videosList;
        }

        public static List<List<string>> CreateListOfPages(List<string> videoList)
        {
            List<List<string>> pages = new List<List<string>>();

            if (videoList.Count < 11)
            {
                pages.Add(videoList.GetRange(0, videoList.Count));
            }

            else if (videoList.Count > 10 && videoList.Count < 21)
            {
                pages.Add(videoList.GetRange(0, 10));
                pages.Add(videoList.GetRange(10, videoList.Count - 10));
            }

            else if (videoList.Count > 20 && videoList.Count < 31)
            {
                pages.Add(videoList.GetRange(0, 10));
                pages.Add(videoList.GetRange(10, 10));
                pages.Add(videoList.GetRange(20, videoList.Count - 20));
            }

            else if (videoList.Count > 30 && videoList.Count < 41)
            {
                pages.Add(videoList.GetRange(0, 10));
                pages.Add(videoList.GetRange(10, 10));
                pages.Add(videoList.GetRange(20, 10));
                pages.Add(videoList.GetRange(30, videoList.Count - 30));
            }

            else if (videoList.Count > 40)
            {
                pages.Add(videoList.GetRange(0, 10));
                pages.Add(videoList.GetRange(10, 10));
                pages.Add(videoList.GetRange(20, 10));
                pages.Add(videoList.GetRange(30, 10));
                pages.Add(videoList.GetRange(40, videoList.Count - 40));
            }

            return pages;
        }

        public static string[] AssignContentToPages(string[] pages, List<List<string>> pagesContent)
        {
            pages[0] = string.Join("\n", pagesContent[0].ToArray());

            if (pagesContent.Count > 1)
            {
                pages[1] = string.Join("\n", pagesContent[1].ToArray());
            }

            if (pagesContent.Count > 2)
            {
                pages[2] = string.Join("\n", pagesContent[2].ToArray());
            }

            if (pagesContent.Count > 3)
            {
                pages[3] = string.Join("\n", pagesContent[3].ToArray());
            }

            if (pagesContent.Count > 4)
            {
                pages[4] = string.Join("\n", pagesContent[4].ToArray());
            }

            return pages;
        }
    }
}
