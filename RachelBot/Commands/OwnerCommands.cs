﻿using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using RachelBot.Utils;
using RachelBot.Preconditions;
using RachelBot.Core.Dialogues;

namespace RachelBot.Commands;

[RequireOwner]
[RequirePublicChannel]
public class OwnerCommands : InteractiveBase<SocketCommandContext>
{
    [Command("test")]
    [RequireMotherServerMember]
    public async Task Test()
    {
        string msg = "";

        foreach (string item in Dialogue.FilterDialogues(Context))
        {
            msg += $"{item}\n";
        }

        await Context.Channel.SendMessageAsync(msg);
    }

    [Command("Guilds")]
    public async Task GetGuilds()
    {
        await Context.Channel.SendMessageAsync($"Guilds: {Context.Client.Guilds.Count}");
    }

    [Command("BotStatus", RunMode = RunMode.Async)]
    public async Task GetStatus()
    {
        List<ulong> userList = new List<ulong>();

        foreach (SocketGuild guild in Context.Client.Guilds)
        {
            await guild.DownloadUsersAsync();

            foreach (SocketGuildUser user in guild.Users)
            {
                if (!user.IsBot && !userList.Contains(user.Id))
                {
                    userList.Add(user.Id);
                }
            }
        }

        await Context.Channel.SendMessageAsync($"Guilds: {Context.Client.Guilds.Count}\nUsers: {userList.Count}");
    }

    [Command("CheckOwnerships")]
    public async Task CheckOwnerships()
    {
        Dictionary<ulong, int> ownerships = new Dictionary<ulong, int>();

        foreach (SocketGuild guild in Context.Client.Guilds)
        {
            if (ownerships.ContainsKey(guild.OwnerId))
            {
                ownerships[guild.OwnerId]++;
            }
            else
            {
                ownerships.Add(guild.OwnerId, 1);
            }
        }

        string message = "";

        foreach (KeyValuePair<ulong, int> ownership in ownerships)
        {
            message += $"{ownership.Key}:{ownership.Value}\n";
        }

        await Context.Channel.SendMessageAsync(message);
    }

    [Command("DebugGuildConfig")]
    public async Task DebugConfig()
    {
        IDMChannel channel = await Context.User.CreateDMChannelAsync();
        await channel.SendFileAsync($"./Guilds/{Context.Guild.Id}/Config.json");
    }

    [Command("CommunicateWithOwners", RunMode = RunMode.Async)]
    public async Task CommunicateWithOwners([Remainder]string message)
    {
        IDMChannel channel;

        foreach (SocketGuild guild in Context.Client.Guilds)
        {
            try
            {
                channel = await guild.Owner.CreateDMChannelAsync();
                await channel.SendMessageAsync($"{message}\n\n{Utility.OwnerSignature}");
                await Task.Delay(1000);
            }
            catch (Exception)
            {
                continue;
            }
        }

        await Context.Channel.SendMessageAsync("Done");
    }

    [Command("TestBtn")]
    public async Task TestButtons()
    {
        ComponentBuilder builder = new ComponentBuilder();

        builder.WithButton(label: "Lorem", customId: "LoremId", style: ButtonStyle.Success)
               .WithButton(label: "Ipsum", customId: "IpsumId", style: ButtonStyle.Danger);

        await ReplyAsync("Btn test:", component: builder.Build());
    }
}
