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
using System.Collections.Generic;
using System.IO;
using System.Text;
using GravityCTRL.FilterChili.Search.Fragments;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Search
{
    internal sealed class FragmentedSearch : List<Fragment>
    {
        private const char DOUBLE_QUOTE = '"';
        private const char ACTION_CHARACTER = ':';
        private const char EXCLUDE_CHARACTER = '-';

        public FragmentedSearch(string searchString)
        {
            var phrases = CreateClassifiedFragments(searchString);
            AddRange(phrases);
        }

        private static IEnumerable<Fragment> CreateClassifiedFragments(string text)
        {
            var stringBuilder = new StringBuilder();

            var waitForEndQuote = false;
            var shallExclude = false;
            string propertyName = null;

            Fragment CreateClassifiedFragment()
            {
                var phrase = ExtractPhrase(stringBuilder);
                if (string.IsNullOrWhiteSpace(phrase))
                {
                    return null;
                }

                Fragment fragment;
                if (propertyName != null)
                {
                    if (propertyName == string.Empty)
                    {
                        propertyName = null;
                        return null;
                    }
                    
                    if (shallExclude)
                    {
                        if (waitForEndQuote)
                        {
                            fragment = new ConstrainedExcludeFragment(FragmentType.Phrase, phrase, propertyName);
                            waitForEndQuote = false;
                        }
                        else
                        {
                            fragment = new ConstrainedExcludeFragment(FragmentType.Word, phrase, propertyName);
                        }

                        shallExclude = false;
                    }
                    else
                    {
                        if (waitForEndQuote)
                        {
                            fragment = new ConstrainedIncludeFragment(FragmentType.Phrase, phrase, propertyName);
                            waitForEndQuote = false;
                        }
                        else
                        {
                            fragment = new ConstrainedIncludeFragment(FragmentType.Word, phrase, propertyName);
                        }
                    }

                    propertyName = null;
                }
                else
                {
                    if (shallExclude)
                    {
                        if (waitForEndQuote)
                        {
                            fragment = new ExcludeFragment(FragmentType.Phrase, phrase);
                            waitForEndQuote = false;
                        }
                        else
                        {
                            fragment = new ExcludeFragment(FragmentType.Word, phrase);
                        }

                        shallExclude = false;
                    }
                    else
                    {
                        if (waitForEndQuote)
                        {
                            fragment = new IncludeFragment(FragmentType.Phrase, phrase);
                            waitForEndQuote = false;
                        }
                        else
                        {
                            fragment = new IncludeFragment(FragmentType.Word, phrase);
                        }
                    }
                }

                return fragment;
            }

            using (var reader = new StringReader(text))
            {
                int readCharacter;
                while ((readCharacter = reader.Read()) != -1)
                {
                    var character = Convert.ToChar(readCharacter);
                    if (character == DOUBLE_QUOTE)
                    {
                        if (!waitForEndQuote)
                        {
                            waitForEndQuote = true;
                        }
                        else
                        {
                            var fragment = CreateClassifiedFragment();
                            if (fragment != null)
                            {
                                yield return fragment;
                            }
                        }
                        continue;
                    }

                    if (waitForEndQuote)
                    {
                        stringBuilder.Append(character);
                        continue;
                    }

                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (character)
                    {
                        case EXCLUDE_CHARACTER:
                        {
                            shallExclude = true;
                            continue;
                        }
                        case ACTION_CHARACTER:
                        { 
                            propertyName = ExtractPhrase(stringBuilder);
                            continue;
                        }
                    }

                    if (!char.IsLetterOrDigit(character))
                    {
                        var fragment = CreateClassifiedFragment();
                        if (fragment != null)
                        {
                            yield return fragment;
                        }
                        continue;
                    }

                    stringBuilder.Append(character);
                }

                var lastFragment = CreateClassifiedFragment();
                if (lastFragment != null)
                {
                    yield return lastFragment;
                }
            }
        }

        private static string ExtractPhrase([NotNull] StringBuilder stringBuilder)
        {
            var phrase = stringBuilder.ToString();
            stringBuilder.Clear();
            return phrase.Trim();
        }
    }
}