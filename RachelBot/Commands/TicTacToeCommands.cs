using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using RachelBot.Core.Configs;
using RachelBot.Core.Games.TicTacToe;
using RachelBot.Lang;
using RachelBot.Services.Storage;
using RachelBot.Utils;
using RachelBot.Preconditions;

namespace RachelBot.Commands
{
    [RequirePublicChannel]
    public class TicTacToeCommands : InteractiveBase<SocketCommandContext>
    {
        private readonly IStorageService _storage;

        public TicTacToeCommands(IStorageService storage)
        {
            _storage = storage;
        }

        [Command("TicTacToe", RunMode = RunMode.Async)]
        public async Task TicTacToe(SocketUser user = null)
        {
            SocketGuild guild = Context.Guild;
            GuildConfig config = new GuildConfigs(guild.Id, _storage).GetGuildConfig();
            AlertsHandler alerts = new AlertsHandler(config);
            Game game = new Game();
            Player player = new Player(Context.User.Id, "❌");

            if (user == null)
            {
                Player ai = new Player(Context.Client.CurrentUser.Id, "⭕");

                await Context.Channel.SendMessageAsync(alerts.GetAlert("TICTACTOE_NO_OPPONENT"));

                PlayGameSP(game, player, ai, alerts);
            }
            else if (user.Id == Context.Client.CurrentUser.Id)
            {
                Player ai = new Player(Context.Client.CurrentUser.Id, "⭕");

                await Context.Channel.SendMessageAsync(alerts.GetAlert("TICTACTOE_PLAY_WITH_RACHEL"));

                PlayGameSP(game, player, ai, alerts);
            }
            else if (user.IsBot)
            {
                Player ai = new Player(Context.Client.CurrentUser.Id, "⭕");

                await Context.Channel.SendMessageAsync(alerts.GetAlert("TICTACTOE_PLAY_WITH_BOT"));

                PlayGameSP(game, player, ai, alerts);
            }
            else
            {
                Player opponent = new Player(user.Id, "⭕");

                PlayGameMP(game, player, opponent, alerts);
            }
        }

        private async void PlayGameSP(Game game, Player firstPlayer, Player secondPlayer, AlertsHandler alerts)
        {
            RestUserMessage boardMessage = await Context.Channel.SendMessageAsync($"<@{firstPlayer.Id}> vs <@{secondPlayer.Id}>\n\n{game.ShowTable()}");

            for (int i = 0; i < 11; i++)
            {
                if (game.FirstPlayerTurn)
                {
                    await boardMessage.ModifyAsync(m => m.Content = $"<@{firstPlayer.Id}> vs <@{secondPlayer.Id}>\n\n{game.ShowTable()}");

                    if (await CheckIfGameEnded(game, firstPlayer, secondPlayer, alerts, i, boardMessage))
                    {
                        return;
                    }

                    SocketMessage message = await NextMessageAsync();

                    if (message == null)
                    {
                        await Context.Channel.SendMessageAsync(alerts.GetFormattedAlert("TICTACTOE_GAVE_UP", Context.User.Username));
                        return;
                    }

                    var cords = Player.GetCords(message.Content);
                    await Task.Delay(1000);

                    PermissionUtils.TryRemoveMessageAsync(Context, message);

                    if (!cords.HasValue)
                    {
                        i--;
                        await Context.Channel.SendMessageAsync(alerts.GetAlert("TICTACTOE_WRONG_INPUT"));
                        continue;
                    }

                    game.ChangeTableElement(cords.Value.Key, cords.Value.Value, firstPlayer.Character);
                    game.ChangePlayer();
                }
                else
                {
                    if (await CheckIfGameEnded(game, firstPlayer, secondPlayer, alerts, i, boardMessage))
                    {
                        return;
                    }

                    int input = Core.Games.TicTacToe.AI.HardCodedAI.MakeMove(game, firstPlayer, secondPlayer);

                    int row = (input - 1) / 3;
                    int column = (input - 1) % 3;

                    game.ChangeTableElement(row, column, secondPlayer.Character);
                    game.ChangePlayer();
                }
            }
        }

        private async void PlayGameMP(Game game, Player firstPlayer, Player secondPlayer, AlertsHandler alerts)
        {
            RestUserMessage boardMessage = await Context.Channel.SendMessageAsync($"<@{firstPlayer.Id}> vs <@{secondPlayer.Id}>\n\n{game.ShowTable()}");

            for (int i = 0; i < 11; i++)
            {
                if (game.FirstPlayerTurn)
                {
                    await boardMessage.ModifyAsync(m => m.Content = $"<@{firstPlayer.Id}> vs <@{secondPlayer.Id}>\n\n{game.ShowTable()}");

                    if (await CheckIfGameEnded(game, firstPlayer, secondPlayer, alerts, i, boardMessage))
                    {
                        return;
                    }

                    SocketMessage message = await NextMessageAsync();

                    if (message == null)
                    {
                        await Context.Channel.SendMessageAsync(alerts.GetFormattedAlert("TICTACTOE_GAVE_UP", Context.User.Username));
                        return;
                    }

                    var cords = Player.GetCords(message.Content);
                    await Task.Delay(1000);

                    PermissionUtils.TryRemoveMessageAsync(Context, message);

                    if (!cords.HasValue)
                    {
                        i--;
                        await Context.Channel.SendMessageAsync(alerts.GetAlert("TICTACTOE_WRONG_INPUT"));
                        continue;
                    }

                    game.ChangeTableElement(cords.Value.Key, cords.Value.Value, firstPlayer.Character);
                    game.ChangePlayer();
                }
                else
                {
                    await boardMessage.ModifyAsync(m => m.Content = $"<@{firstPlayer.Id}> vs <@{secondPlayer.Id}>\n\n{game.ShowTable()}");

                    if (await CheckIfGameEnded(game, firstPlayer, secondPlayer, alerts, i, boardMessage))
                    {
                        return;
                    }

                    Criteria<SocketMessage> criterion = new Criteria<SocketMessage>();

                    criterion.AddCriterion(new EnsureFromUserCriterion(secondPlayer.Id));
                    criterion.AddCriterion(new EnsureSourceChannelCriterion());

                    SocketMessage message = await NextMessageAsync(criterion);

                    if (message == null)
                    {
                        await Context.Channel.SendMessageAsync(alerts.GetFormattedAlert("TICTACTOE_GAVE_UP", Context.Guild.Users.Single(u => u.Id == secondPlayer.Id).Username));
                        return;
                    }

                    var cords = Player.GetCords(message.Content);
                    await Task.Delay(1000);

                    PermissionUtils.TryRemoveMessageAsync(Context, message);

                    if (!cords.HasValue)
                    {
                        i--;
                        await Context.Channel.SendMessageAsync(alerts.GetAlert("TICTACTOE_WRONG_INPUT"));
                        continue;
                    }

                    game.ChangeTableElement(cords.Value.Key, cords.Value.Value, secondPlayer.Character);
                    game.ChangePlayer();
                }
            }
        }

        private async Task<bool> CheckIfGameEnded(Game game, Player firstPlayer, Player secondPlayer, AlertsHandler alerts, int moveCount, RestUserMessage boardMessage)
        {
            if (moveCount >= 5)
            {
                if (game.CheckScore())
                {
                    await boardMessage.ModifyAsync(m => m.Content = $"<@{firstPlayer.Id}> vs <@{secondPlayer.Id}>\n\n{game.ShowTable()}");
                    await Context.Channel.SendMessageAsync(alerts.GetFormattedAlert("TICTACTOE_WIN", (moveCount % 2 == 1) ? firstPlayer.Character : secondPlayer.Character));
                    return true;
                }
            }
            if (moveCount == 9)
            {
                await boardMessage.ModifyAsync(m => m.Content = $"<@{firstPlayer.Id}> vs <@{secondPlayer.Id}>\n\n{game.ShowTable()}");
                await Context.Channel.SendMessageAsync(alerts.GetAlert("TICTACTOE_DRAW"));
                return true;
            }

            return false;
        }
    }
}
