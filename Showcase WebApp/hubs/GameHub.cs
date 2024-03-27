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

        public GameHub(GameManager gameManager)
        {
            _gameManager = gameManager;

            HandleEventListeners();
        }

        private async void HandleEventListeners()
        {
            await _gameManager.RemoveSubscribedEvents();

            _gameManager.GameStarted += NotifyGameStarted;

            foreach (var session in _gameManager.Sessions)
            {
                await session.RemoveSubscribedEvents();
                session.NotifyBoardUpdated += NotifyBoardUpdated;
                session.GameEnded += NotifyGameEnded;
            }
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

        private async void NotifyGameEnded(object? sender, EventArgs args)
        {
            GameBoardModel board1;
            GameBoardModel board2;

            if (sender is GameSessionModel session)
            {
                board1 = session.GameBoard1;
                board2 = session.GameBoard2;
            }
            else return;

            await Clients.Client(board1.Player.ConnectionID).SendAsync("GameEnded", board1.Guesses, board2.Guesses, board1.Player, board2.Player);
            await Clients.Client(board2.Player.ConnectionID).SendAsync("GameEnded", board1.Guesses, board2.Guesses, board1.Player, board2.Player);
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

            await Clients.Client(player1.ConnectionID).SendAsync("UpdateBoard", args.Board.Player.Name, args.Board.Guesses);
            await Clients.Client(player2.ConnectionID).SendAsync("UpdateBoard", args.Board.Player.Name, args.Board.Guesses);
        }

        private async void NotifyGameStarted(object? sender, GameStartedEventArgs args)
        {
            var player1 = args.Session.GameBoard1.Player;
            var player2 = args.Session.GameBoard2.Player;

            await Clients.Client(player2.ConnectionID).SendAsync("GameStarted", player1.Name);
            await Clients.Client(player1.ConnectionID).SendAsync("GameStarted", player2.Name);
        }

        private async Task NotifyInvalidWord(string connectionID)
        {
            await Clients.Client(connectionID).SendAsync("InvalidWord");
        }

        private async void NotifyError(string connectionID, Exception ex)
        {

        }
    }
}
