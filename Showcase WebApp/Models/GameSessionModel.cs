using Microsoft.Identity.Client;
using Showcase_WebApp.Models.EventArgs;

namespace Showcase_WebApp.Models
{
    public class GameSessionModel
    {
        public event EventHandler GameEnded;

        public event EventHandler<BoardUpdatedEventArgs> NotifyBoardUpdated;

        public event EventHandler<BoardUpdatedEventArgs> NotifyBoardFull;

        public GameBoardModel GameBoard1 { get; set; }

        public GameBoardModel GameBoard2 { get; set; }

        private int count;

        private bool active;

        public GameSessionModel(GameBoardModel board1, GameBoardModel board2)
        {
            GameBoard1 = board1;
            GameBoard2 = board2;

            GameBoard1.BoardUpdated += OnBoardUpdated;
            GameBoard2.BoardUpdated += OnBoardUpdated;

            GameBoard1.BoardFull += CheckGame;
            GameBoard2.BoardFull += CheckGame;

            GameBoard1.BoardFull += (sender, args) =>
            {
                if (sender is GameBoardModel board)
                    NotifyBoardFull?.Invoke(this, new BoardUpdatedEventArgs(board));
            };

            GameBoard2.BoardFull += (sender, args) =>
            {
                if (sender is GameBoardModel board)
                    NotifyBoardFull?.Invoke(this, new BoardUpdatedEventArgs(board));
            };

            active = true;

            StartBackgroundCounter();
        }

        public async Task InsertGuessFromConnectionID(string connectionID, string word)
        {
            if (GameBoard1.Player.ConnectionID == connectionID)
            {
                GameBoard1.InsertGuess(word);
            }
            else if (GameBoard2.Player.ConnectionID == connectionID)
            {
                GameBoard2.InsertGuess(word);
            }
            else throw new Exception("Connection ID not found");
        }

        public async Task RemoveSubscribedEvents()
        {
            GameEnded = null;
            NotifyBoardUpdated = null;
            NotifyBoardFull = null;
        }

        private async void OnBoardUpdated(object? sender, System.EventArgs e)
        {
            if (sender is GameBoardModel board)
                NotifyBoardUpdated?.Invoke(this, new BoardUpdatedEventArgs(board));
        }

        private async void CheckGame(object? sender, System.EventArgs args)
        {
            if (sender is GameBoardModel board)
            {
                await CalculateScore(board);

                if (!GameBoard1.IsActive && !GameBoard2.IsActive)
                {
                    active = false;

                    GameEnded?.Invoke(this, System.EventArgs.Empty);
                }
            }
        }

        private async Task CalculateScore(GameBoardModel board)
        {
            int score = (int)((1150 - board.Tries * 150) * Math.Pow(0.995, count));
            board.Player.Score = score;
        }

        private async void StartBackgroundCounter()
        {
            while (active)
            {
                await Task.Delay(1000);
                count++;
            }
        }
    }
}
