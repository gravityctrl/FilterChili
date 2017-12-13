using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GravityCTRL.FilterChili.Phonetics
{
    public static class Soundex
    {
        public static string ToSoundex(this string word)
        {
            word = word.Trim().ToLowerInvariant();

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
                if (codeLength == 0 || sb[codeLength - 1] != code)
                {
                    sb.Append(code);
                    count++;
                }
            }

            for (var index = 0; index < length && count < 3; index++)
            {
                var character = word[index];
                if (index == 0)
                {
                    sb.Append(character);
                    sb.Append('-');
                    continue;
                }

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (character)
                {
                    case 'b':
                    case 'f':
                    case 'p':
                    case 'v':
                    {
                        Append('1');
                        break;
                    }
                    case 'c':
                    case 'g':
                    case 'j':
                    case 'k':
                    case 'q':
                    case 's':
                    case 'x':
                    case 'z':
                    {
                        Append('2');
                        break;
                    }
                    case 'd':
                    case 't':
                    {
                        Append('3');
                        break;
                    }
                    case 'l':
                    {
                        Append('4');
                        break;
                    }
                    case 'm':
                    case 'n':
                    {
                        Append('5');
                        break;
                    }
                    case 'r':
                    {
                        Append('6');
                        break;
                    }
                }
            }

            // ReSharper disable once NotAccessedVariable
            for (var index = count; count < 3; index++)
            {
                Append('0');
            }

            return sb.ToString();
        }
    }
}
