namespace Showcase_WebApp.Models.EventArgs
{
    public class BoardUpdatedEventArgs
    {

        public GameBoardModel Board { get; set; }

        public BoardUpdatedEventArgs(GameBoardModel board)
        {
            Board = board;
        }
    }
}
