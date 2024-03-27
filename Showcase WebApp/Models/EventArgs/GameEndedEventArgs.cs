namespace Showcase_WebApp.Models.EventArgs
{
    public class GameEndedEventArgs
    {
        public GameBoardModel Board { get; set; }

        public Player Player1 { get; set; }

        public Player Player2 { get; set; }

        public GameEndedEventArgs(GameBoardModel board, Player player1, Player player2)
        {
            Board = board;
            Player1 = player1;
            Player2 = player2;
        }
    }
}
