using System.Diagnostics;

namespace Showcase_WebApp.data.DataAccessObjects
{
    public class GameDAO
    {
        public static readonly string allWordsFilePath = "/PossibleWords.txt";

        public static readonly string possibleWordsFilePath = "/wordle_words.txt";

        private List<string> possibleWords;

        public List<string> allWords;

        public GameDAO()
        {
            RetrievePossibleWords();
            RetrieveAllWords();
        }

        public async void RetrievePossibleWords()
        {
            possibleWords = await RetrieveWords(possibleWordsFilePath);
        }

        public async void RetrieveAllWords()
        {
            allWords = await RetrieveWords(allWordsFilePath);
        }

        private async Task<List<string>> RetrieveWords(string filePath)
        {
            try
            {
                string currentPath = Directory.GetCurrentDirectory();

                string fullPath = currentPath + filePath;

                string fileContent = File.ReadAllText(fullPath);

                char[] delimiters = new char[] { ' ', '\n', '\r' };

                List<string> words = new List<string>(fileContent.Split(delimiters, StringSplitOptions.RemoveEmptyEntries));

                return words;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("De data kon niet uitgelezen worden: " + ex.Message);
                return new List<string>();

            }
        }

        public async Task<bool> CheckWord(string word)
        {
            return allWords.Contains(word.ToUpper());
        }

        public async Task<string> GetRandomWord()
        {
            Random random = new();

            int randomIndex = random.Next(0, possibleWords.Count);

            return possibleWords[randomIndex];
        }

    }
}
