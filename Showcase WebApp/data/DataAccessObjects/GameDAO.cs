namespace Showcase_WebApp.data.DataAccessObjects
{
    public class GameDAO
    {
        public static readonly string _filePath = "/data/DataAccessObjects/wordle_words.txt";

        public List<string> Words { get; private set; }

        public GameDAO()
        {
            RetrieveWords();
        }

        public async Task RetrieveWords()
        {
            try
            {
                string currentPath = Directory.GetCurrentDirectory();

                string fullPath = currentPath + _filePath;

                string fileContent = File.ReadAllText(fullPath);

                char[] delimiters = new char[] { ' ', '\n', '\r' };

                List<string> words = new List<string>(fileContent.Split(delimiters, StringSplitOptions.RemoveEmptyEntries));

                Words = words;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("De data kon niet uitgelezen worden: " + ex.Message);
            }
        }

        public async Task<bool> CheckWord(string word)
        {
            return Words.Contains(word);
        }

        public async Task<string> GetRandomWord()
        {
            Random random = new();

            int randomIndex = random.Next(0, Words.Count);

            return Words[randomIndex];
        }

    }
}
