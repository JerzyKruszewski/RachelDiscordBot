using Discord;
using Discord.WebSocket;
using RachelBot.Core.Configs;

namespace RachelBot.Utils;

public class Utility
{
    public const string Version = "1.10.0";

    public const ulong MotherServerId = 701156872698462299;

    public const string DiscordInviteLink = @"https://discord.gg/TjCDEQU";

    public const string GitHubPage = @"https://github.com/JerzyKruszewski/RachelDiscordBot";

    public const string TopGGPage = @"https://top.gg/bot/810093575500726302";

    public const string PatreonPage = @"https://www.patreon.com/bajarzdevelopment?fan_landing=true";

    public const string SaraArtistInstagram = @"https://www.instagram.com/aster_atheris/";

    public const string OwnerSignature = @"Kind regards
Jerzy Kruszewski (@Jurij98#2750)
Creator of ℜ𝔞𝔠𝔥𝔢𝔩";

    public const int MessageLengthBuffer = 1950;

    public static readonly Random random = new Random();

    public static IEnumerable<Emoji> AnswersEmojis { get; } = new List<Emoji>()
    {
        new Emoji("🇦"),
        new Emoji("🇧"),
        new Emoji("🇨"),
        new Emoji("🇩"),
        new Emoji("🇪"),
        new Emoji("🇫"),
        new Emoji("🇬"),
        new Emoji("🇭"),
        new Emoji("🇮"),
        new Emoji("🇯"),
        new Emoji("🇰"),
        new Emoji("🇱"),
        new Emoji("🇲"),
        new Emoji("🇳"),
        new Emoji("🇴"),
        new Emoji("🇵"),
        new Emoji("🇷"),
        new Emoji("🇸"),
        new Emoji("🇹"),
        new Emoji("🇺")
    };

    public static ISocketMessageChannel GetMessageChannelById(SocketGuild guild, ulong id)
    {
        return guild.TextChannels.SingleOrDefault(c => c.Id == id);
    }

    public static IRole GetRoleById(SocketGuild guild, ulong id)
    {
        return guild.Roles.SingleOrDefault(r => r.Id == id);
    }

    public static SocketGuildUser GetGuildUserById(SocketGuild guild, ulong id)
    {
        return guild.Users.SingleOrDefault(u => u.Id == id);
    }

    public static string ParseReason(string reason, GuildConfig config)
    {
        if (config.PointBasedWarns)
        {
            string[] words = reason.Split(' ');
            reason = words[1];

            for (int i = 2; i < words.Length; i++)
            {
                reason += $" {words[i]}";
            }
        }

        return reason;
    }

    public static SocketGuild GetGuildFromSocketMessageComponent(DiscordSocketClient client, SocketMessageComponent messageComponent)
    {
        return client.Guilds.Single(g => g.TextChannels.Any(ch => ch.Id == messageComponent.Channel.Id));
    }
}
