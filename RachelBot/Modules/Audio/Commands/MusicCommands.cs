using Discord;
using Discord.Commands;
using Discord.WebSocket;
using RachelBot.Core.Configs;
using RachelBot.Lang;
using RachelBot.Modules.Audio.Objects;
using RachelBot.Modules.Audio.Services;
using RachelBot.Services.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.Modules.Audio.Commands
{
    public class MusicCommands : ModuleBase<SocketCommandContext>
    {
        private readonly IStorageService _storage;

        public MusicCommands(IStorageService storage)
        {
            _storage = storage;
        }

        [Command("play", RunMode = RunMode.Async)]
        [Alias("zagraj nam", "zagraj mi")]
        public async Task PlayAsync([Remainder] string song = "")
        {
            try
            {
                await AudioService.PlayAsync(Context, song);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [Command("leave")]
        [Alias("opuść")]
        public async Task LeaveAsync()
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            await Context.Channel.SendMessageAsync(alerts.GetAlert("LEAVE_VOICECHANNEL"));
            await AudioService.LeaveAsync(Context.Guild);
        }

        [Command("Skip")]
        [Alias("skipnij")]
        public async Task SkipAsync()
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            AudioQueue audioQueue = AudioQueueService.GetAudioQueue(Context.Guild);
            string skippedTrackTitle = audioQueue.Queue[audioQueue.PlayingTrackIndex].Track.Title;
            await AudioService.SkipAsync(Context.Guild);
            await ReplyAsync(alerts.GetFormattedAlert("TRACK_SKIPPED", skippedTrackTitle));
        }

        [Command("Queue")]
        [Alias("kolejka")]
        public async Task QueueAsync()
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            TimeSpan time = new TimeSpan();
            AudioQueue audioQueue = AudioQueueService.GetAudioQueue(Context.Guild);
            string avatar = Context.Message.Author.GetAvatarUrl() ?? Context.Message.Author.GetDefaultAvatarUrl();

            if (audioQueue.Queue.ElementAtOrDefault(0) is null)
            {
                EmbedBuilder builderNull = new EmbedBuilder().WithAuthor(Context.Message.Author.Username, avatar)
                                                             .WithDescription(alerts.GetAlert("EMPTY_QUEUE"))
                                                             .WithColor(new Color(1, 69, 44));

                await ReplyAsync("", false, builderNull.Build());
                return;
            }

            foreach (AudioQueueItem item in audioQueue.Queue)
            {
                time += item.Track.Length;
            }

            List<string> queue = AudioQueuesManagment.CreateListOfSongs(audioQueue.Queue);
            List<List<string>> pagesContent = AudioQueuesManagment.CreateListOfPages(queue);
            string[] pages = new string[5];
            pages = AudioQueuesManagment.AssignContentToPages(pages, pagesContent);

            await Pager(pages, time, alerts);
        }

        private async Task Pager(string[] pages, TimeSpan time, AlertsHandler alerts)
        {
            StringBuilder builder = new StringBuilder(alerts.GetAlert("QUEUE"));

            foreach (string page in pages)
            {
                builder.Append($"{page}\n");
            }

            builder.Append(alerts.GetFormattedAlert("QUEUE_FULL_TIME", time));

            Embed embed = new EmbedBuilder()
            {
                Description = builder.ToString(),
                Color = new Color(1, 69, 44)
            }.Build();

            await Context.Channel.SendMessageAsync("", embed: embed);
        }
    }
}
