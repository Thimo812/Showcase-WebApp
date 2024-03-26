using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Client;
using Showcase_WebApp.data.DataAccessObjects;
using Showcase_WebApp.Managers;
using Showcase_WebApp.Models;
using Showcase_WebApp.Models.EventArgs;

namespace Showcase_WebApp.hubs
{
    public class GameHub : Hub
    {
        private GameManager _gameManager;

        private readonly Connections<GameHub> _connections;

        public GameHub(Connections<GameHub> connections)
        {
            GameDAO DAO = new GameDAO();

            _gameManager = new GameManager(DAO);

            _connections = connections;

            _gameManager.GameStarted += HandleGameStarted;
        }

        private async void HandleGameStarted(object? sender, GameStartedEventArgs args)
        {
            args.Session.NotifyBoardUpdated += NotifyBoardUpdated;
            args.Session.GameEnded += NotifyGameEnded;

            var player1 = args.Session.GameBoard1.Player;
            var player2 = args.Session.GameBoard2.Player;

            await NotifyGameStarted(player1, player2);
        }

        public async void GuessWord(string word)
        {
            string connectionID = Context.ConnectionId;
            bool isSucces = await _gameManager.SendWord(word, connectionID);

            if (!isSucces)
            {
                await NotifyInvalidWord(connectionID);
            }
        }

        public async Task FindGame(string userName)
        {
            string connectionID = Context.ConnectionId;

            await _gameManager.QueuePlayer(connectionID, userName);
        }

        public async Task CancelFindGame()
        {
            string connectionID = Context.ConnectionId;

            await _gameManager.DequeuePlayer(connectionID);
        }

        public override Task OnConnectedAsync()
        {
            _connections.All.TryAdd(Context.ConnectionId, Context);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            _connections.All.TryRemove(Context.ConnectionId, out _);

            return base.OnDisconnectedAsync(exception);
        }


        private async void NotifyGameEnded(object? sender, System.EventArgs args)
        {
            GameBoardModel board1;
            GameBoardModel board2;

            if (sender is GameSessionModel session)
            {
                board1 = session.GameBoard1;
                board2 = session.GameBoard2;
            }
            else return;

            await Clients.Client(board1.Player.ConnectionID).SendAsync("GameEnded", board1, board2);
            await Clients.Client(board2.Player.ConnectionID).SendAsync("GameEnded", board1, board2);
        }

        private async void NotifyBoardUpdated(object? sender, BoardUpdatedEventArgs args)
        {
            Player player1;
            Player player2;

            if (sender is GameSessionModel session)
            {
                player1 = session.GameBoard1.Player;
                player2 = session.GameBoard2.Player;
            }
            else return;

            await Clients.Client(player1.ConnectionID).SendAsync("UpdateBoard", args.Board);
            await Clients.Client(player2.ConnectionID).SendAsync("UpdateBoard", args.Board);
        }

        private async Task NotifyGameStarted(Player player1, Player player2)
        {
            await Clients.Client(player2.ConnectionID).SendAsync("GameStarted", player1.Name);
            await Clients.Client(player1.ConnectionID).SendAsync("GameStarted", player2.Name);
        }

        private async Task NotifyInvalidWord(string connectionID)
        {
            await Clients.Client(connectionID).SendAsync("InvalidWord");
        }
    }
}
