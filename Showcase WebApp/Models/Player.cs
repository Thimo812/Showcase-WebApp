namespace Showcase_WebApp.Models
{
    public class Player
    {
        public string Name { get; set; }

        public string ConnectionID { get; set; }

        public int? Score { get; set; }

        public Player(string name, string connectionID)
        {
            Name = name;
            ConnectionID = connectionID;
        }

        public Player(string name, string connectionID, int? score) : this(name, connectionID)
        {
            Score = score;
        }
    }
}
