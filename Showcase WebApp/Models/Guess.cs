using Microsoft.IdentityModel.Protocols;
using Showcase_WebApp.Enums;

namespace Showcase_WebApp.Models
{
    public class Guess
    {
        public static readonly int wordSize = 5;

        public KeyValuePair<char, LetterState>[] Letters {  get; set; }

        public Guess(KeyValuePair<char, LetterState>[] letters)
        {
            Letters = letters;
        }

        public override string ToString()
        {
            var returnString = "";
            foreach (var letter in Letters)
            {
                returnString += letter.Key;
            }
            return returnString;
        }
    }
}
