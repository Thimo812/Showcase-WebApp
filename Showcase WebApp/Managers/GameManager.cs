﻿using Showcase_WebApp.data.DataAccessObjects;
using Showcase_WebApp.Models;
using Showcase_WebApp.Models.EventArgs;

namespace Showcase_WebApp.Managers
{
    public class GameManager
    {
        public event EventHandler<GameStartedEventArgs> GameStarted;

        private GameDAO _gameDAO;

        public List<GameSessionModel> Sessions { get; private set; }

        private Queue<Player> playerQueue;

        public GameManager(GameDAO gameDAO)
        {
            _gameDAO = gameDAO;

            Sessions = new List<GameSessionModel>();

            playerQueue = new Queue<Player>();
        }

        public async Task RemoveSubscribedEvents()
        {
            GameStarted = null;
        }

        public async Task<bool> QueuePlayer(string connectionID, string userName)
        {
            if (playerQueue.Any(Player => Player.Name == userName)) throw new Exception("player already in queue");

            playerQueue.Enqueue(new Player(userName, connectionID));

            if (playerQueue.Count < 2) return false;

            Player player1 = playerQueue.Dequeue();
            Player player2 = playerQueue.Dequeue();

            var session = await StartSession(player1, player2);

            GameStarted?.Invoke(this, new GameStartedEventArgs(session));

            return true;
        }

        public async Task DequeuePlayer(string connectionID)
        {
            playerQueue = new Queue<Player>(playerQueue.Where(x=> x.ConnectionID != connectionID));
        }

        public async Task<bool> SendWord(string word, string connectionID)
        {
            bool isValid = await _gameDAO.CheckWord(word);

            if(!isValid) return false;

            var session = await GetSessionFromPlayerID(connectionID);

            if(session == null) return false;

            await session.InsertGuessFromConnectionID(connectionID, word);

            return true;
        }

        private async Task<GameSessionModel> StartSession(Player player1, Player player2)
        {
            string word1 = await _gameDAO.GetRandomWord();
            string word2 = await _gameDAO.GetRandomWord();

            var board1 = new GameBoardModel(word1, player1);
            var board2 = new GameBoardModel(word2, player2);

            var session = new GameSessionModel(board1, board2);

            Sessions.Add(session);

            return session;
        }

        private async Task<GameSessionModel> GetSessionFromPlayerID(string connectionID)
        {
            var session = Sessions.FirstOrDefault(x => x.GameBoard1.Player.ConnectionID == connectionID || x.GameBoard2.Player.ConnectionID == connectionID);
            return session;
        }
    }
}
