using Showcase_WebApp.Enums;
using System.Diagnostics;

namespace Showcase_WebApp.Models
{
    public class GameBoardModel
    {
        public event EventHandler<System.EventArgs> BoardFull;

        public event EventHandler<System.EventArgs> BoardUpdated;

        public static readonly int maxTries = 6;

        public static readonly int wordLength = 5;

        public Player Player { get; set; }

        public string Word { get; private set; }

        public int Tries {  get; private set; }

        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;

            set
            {
                _isActive = value;

                if (!value)
                {
                    BoardFull.Invoke(this, System.EventArgs.Empty);
                }
            }
        }

        public Guess[] guesses = new Guess[maxTries];

        public GameBoardModel(string word, Player player)
        {
            Word = word;
            Player = player;

            IsActive = true;
        }

        public async Task InsertGuess(string word)
        {
            for (int i = 0; i < guesses.Length; i++)
            {
                if (guesses[i] == null)
                {
                    guesses[i] = CheckWord(word);

                    Tries++;

                    if (Word.Equals(word)) IsActive = false;

                    else BoardUpdated.Invoke(this, System.EventArgs.Empty);

                    return;
                }
            }

            IsActive = false;
        }

        private Guess CheckWord(string word)
        {
            word = word.ToLower();

            KeyValuePair<char, LetterState>[] letterArray = new KeyValuePair<char, LetterState>[wordLength];

            char[] letters = word.ToCharArray();

            for (int i = 0; i < letters.Length; i++)
            {
                LetterState state;

                if (letters[i] == Word.ToCharArray()[i]) state = LetterState.correct;

                else if (Word.Contains(letters[i])) state = LetterState.wrongPlace;

                else state = LetterState.incorrect;

                letterArray[i] = new KeyValuePair<char, LetterState>(letters[i], state);
            }

            return new Guess(letterArray);
        }
    }
}
