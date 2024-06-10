using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Client;
using Showcase_WebApp.data.DataAccessObjects;
using Showcase_WebApp.Managers;
using Showcase_WebApp.Models;
using Showcase_WebApp.Models.EventArgs;
using System.Diagnostics;

namespace Showcase_WebApp.hubs
{
    [Authorize(Roles = "gameUser, admin")]
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

                session.NotifyBoardFull += NotifyBoardFull;
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

            try
            {
                bool gameStarted = await _gameManager.QueuePlayer(connectionID, userName);

                if (!gameStarted) await Clients.Client(connectionID).SendAsync("OnPlayerEnqueued");
            }
            catch(Exception ex) { }
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

            Player winner = board1.Player.Score > board2.Player.Score ? board1.Player : board2.Player;

            await Clients.Client(board1.Player.ConnectionID).SendAsync("GameEnded", winner.Name, winner.Score);
            await Clients.Client(board2.Player.ConnectionID).SendAsync("GameEnded", winner.Name, winner.Score);
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

        private async void NotifyBoardFull(object? sender, BoardUpdatedEventArgs args)
        {
            Player player1;
            Player player2;

            if (sender is GameSessionModel session)
            {
                player1 = session.GameBoard1.Player;
                player2 = session.GameBoard2.Player;
            }
            else return;

            await Clients.Client(player1.ConnectionID).SendAsync("BoardFull", args.Board.Player.Name, args.Board.Guesses, args.Board.Word, args.Board.Player.Score);
            await Clients.Client(player2.ConnectionID).SendAsync("BoardFull", args.Board.Player.Name, args.Board.Guesses, args.Board.Word, args.Board.Player.Score);
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
    }
}
