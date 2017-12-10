// This file is part of FilterChili.
// Copyright © 2017 Sebastian Krogull.
// 
// FilterChili is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as 
// published by the Free Software Foundation, either version 3 
// of the License, or any later version.
// 
// FilterChili is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public 
// License along with FilterChili. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Linq;
using System.Text;

namespace GravityCTRL.FilterChili.Phonetics
{
    public static class SoundexExtensions
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

            void Append(char code)
            {
                var codeLength = sb.Length;
                if (codeLength == 0 || sb[codeLength - 1] != code)
                {
                    sb.Append(code);
                }
            }

            for (var index = 0; index < length; index++)
            {
                var character = word[index];

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (character)
                {
                    case 'a':
                    case 'e':
                    case 'i':
                    case 'j':
                    case 'o':
                    case 'u':
                    case 'y':
                    case 'ä':
                    case 'ö':
                    case 'ü':
                    case 'h':
                    {
                        if (index == 0)
                        {
                            Append('0');
                        }
                        break;
                    }
                    case 'b':
                    {
                        Append('1');
                        break;
                    }
                    case 'p':
                    {
                        if (index < length - 1 && word[index + 1] == 'h')
                        {
                            Append('3');
                        }
                        else
                        {
                            Append('1');
                        }
                        break;
                    }
                    case 'd':
                    case 't':
                    {
                        if (index < length - 1 && "cszß".Contains(word[index + 1]))
                        {
                            Append('8');
                        }
                        else
                        {
                            Append('2');
                        }
                        break;
                    }
                    case 'f':
                    case 'v':
                    case 'w':
                    {
                        Append('3');
                        break;
                    }
                    case 'g':
                    case 'k':
                    case 'q':
                    {
                        Append('4');
                        break;
                    }
                    case 'c':
                    {
                        if (index == 0 && index < length - 1)
                        {
                            Append("ahkloqrux".Contains(word[index + 1]) ? '4' : '8');
                        }
                        else
                        {
                            if ("szß".Contains(word[index - 1]))
                            {
                                Append('8');
                            }
                            else if (index < length - 1)
                            {
                                Append("ahkoqux".Contains(word[index + 1]) ? '4' : '8');
                            }
                        }
                        break;
                    }
                    case 'x':
                    {
                        if (index > 0 && "ckq".Contains(word[index - 1]))
                        {
                            Append('8');
                        }
                        else
                        {
                            Append('4');
                            Append('8');
                        }
                        break;
                    }
                    case 'l':
                    {
                        Append('5');
                        break;
                    }
                    case 'm':
                    case 'n':
                    {
                        Append('6');
                        break;
                    }
                    case 'r':
                    {
                        Append('7');
                        break;
                    }
                    case 's':
                    case 'z':
                    case 'ß':
                    {
                        Append('8');
                        break;
                    }
                }
            }

            return sb.ToString();
        }
    }
}
