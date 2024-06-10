using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.DataCollection;
using Showcase_WebApp.data.DataAccessObjects;
using Showcase_WebApp.Managers;

namespace Showcase_tests
{
    public class Tests
    {
        private GameManager _gameManager;
        private GameDAO _gameDAO;

        [SetUp]
        public void Setup()
        {
            _gameDAO = new GameDAO();
            _gameManager = new GameManager(_gameDAO);
        }

        [Test]
        public async Task QueuePlayer()
        {
            await _gameManager.QueuePlayer("test", "test");
            await _gameManager.QueuePlayer("test2", "test2");

            Assert.That(_gameManager.Sessions.Count == 1);
        }

        [Test]
        public async Task GameStartIsThrown()
        {
            bool isRaised = false;

            _gameManager.GameStarted += (sender, args) => isRaised = true;

            await _gameManager.QueuePlayer("test", "test");
            await _gameManager.QueuePlayer("test2", "test2");

            await  _gameManager.RemoveSubscribedEvents();

            Assert.True(isRaised);
        }

        [Test]
        public async Task DequeuePlayer()
        {
            await _gameManager.QueuePlayer("test", "test");
            await _gameManager.DequeuePlayer("test");
            await _gameManager.QueuePlayer("test2", "test2");

            Assert.That(_gameManager.Sessions.Count == 0);
        }

        [TestCase("house", true)]
        [TestCase("thimo", false)]

        public async Task GuessWord(string word, bool isTrue)
        {
            await _gameManager.QueuePlayer("test", "test");
            await _gameManager.QueuePlayer("test2", "test2");

            var isSucces = await _gameManager.SendWord(word, "test");
            Assert.That(isSucces == isTrue);
        }

        [Test]
        public async Task CheckValidWord()
        {
            await _gameManager.QueuePlayer("test", "test");
            await _gameManager.QueuePlayer("test2", "test2");

            await _gameManager.SendWord("house", "test");

            var guess = _gameManager.Sessions[0].GameBoard1.Guesses[0];

            Assert.That(guess.ToString().Equals("house"));
        }
    }
}