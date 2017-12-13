using System;
using System.Linq;
using System.Text;

namespace GravityCTRL.FilterChili.Phonetics
{
    public static class Soundex
    {
        private const int MAX_DIGITS = 3;

        public static string ToSoundex(this string word)
        {
            word = word.Trim().ToUpperInvariant();

            var splitters = word.Where(character => !char.IsLetter(character));
            var words = word.Split(splitters.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            return string.Join(' ', words.Select(SoundexForWord));
        }

        private static string SoundexForWord(string word)
        {
            var sb = new StringBuilder();
            var length = word.Length;
            var count = 0;

            void Append(char code)
            {
                var codeLength = sb.Length;
                if (codeLength > 0 && sb[codeLength - 1] == code)
                {
                    return;
                }

                sb.Append(code);
                count++;
            }

            for (var index = 0; index < length && count < MAX_DIGITS; index++)
            {
                var character = word[index];
                if (index == 0)
                {
                    sb.Append(character);
                    continue;
                }

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (character)
                {
                    case 'B':
                    case 'F':
                    case 'P':
                    case 'V':
                    {
                        Append('1');
                        break;
                    }
                    case 'C':
                    case 'G':
                    case 'J':
                    case 'K':
                    case 'Q':
                    case 'S':
                    case 'X':
                    case 'Z':
                    {
                        Append('2');
                        break;
                    }
                    case 'D':
                    case 'T':
                    {
                        Append('3');
                        break;
                    }
                    case 'L':
                    {
                        Append('4');
                        break;
                    }
                    case 'M':
                    case 'N':
                    {
                        Append('5');
                        break;
                    }
                    case 'R':
                    {
                        Append('6');
                        break;
                    }
                }
            }

            for (var i = 0; i < MAX_DIGITS - count; i++)
            {
                sb.Append('0');
            }

            return sb.ToString();
        }
    }
}
