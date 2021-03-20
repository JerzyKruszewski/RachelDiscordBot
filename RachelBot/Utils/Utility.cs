﻿using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;
using RachelBot.Core.Configs;

namespace RachelBot.Utils
{
    public class Utility
    {
        public const string Version = "1.7.1";

        public const string DiscordInviteLink = @"https://discord.gg/TjCDEQU";

        public const string GitHubPage = @"https://github.com/JerzyKruszewski/RachelDiscordBot";

        public const string SaraArtistInstagram = @"https://www.instagram.com/aster_atheris/";

        public const string OwnerSignature = @"Kind regards
Jerzy Kruszewski (@Jurij98#2750)
Creator of ℜ𝔞𝔠𝔥𝔢𝔩";

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
    }
}
