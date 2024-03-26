namespace Showcase_WebApp.Models.EventArgs
{
    public class GameStartedEventArgs : System.EventArgs
    {
        public GameSessionModel Session { get; set; }

        public GameStartedEventArgs(GameSessionModel session)
        {
            Session = session;
        }
    }
}
