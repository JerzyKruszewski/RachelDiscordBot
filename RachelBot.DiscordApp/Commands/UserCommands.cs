using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RachelBot.DiscordApp.Commands;

public class UserCommands : ModuleBase<SocketCommandContext>
{
    [Command("credits", RunMode = RunMode.Async)]
    public async Task CreditsCommend([Remainder]string? text = null)
    {
        EmbedBuilder embedBuilder = new EmbedBuilder();

        embedBuilder.Color = Color.Red;
        embedBuilder.Title = "Credits";
        embedBuilder.Description = $"""
            Owner: Jerzy Kruszewski (Jurij98)
            Artist who created current avatar: {RachelBot.Utils.Utility.SandwitchArtistInstagram}
            Artist who created old avatar: {RachelBot.Utils.Utility.SaraArtistInstagram}
            Kapifili youtube link: {RachelBot.Utils.Utility.KapifiliYoutube}
            """;

        await Context.Channel.SendMessageAsync(embed: embedBuilder.Build());
    }
}
