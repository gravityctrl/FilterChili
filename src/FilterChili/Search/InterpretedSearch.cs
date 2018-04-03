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
using GravityCTRL.FilterChili.Models;
using GravityCTRL.FilterChili.Search.Fragments;
using JetBrains.Annotations;

namespace GravityCTRL.FilterChili.Search
{
    internal sealed class InterpretedSearch : List<Fragment>
    {
        private const char DOUBLE_QUOTE = '"';
        private const char ACTION_CHARACTER = ':';
        private const char EXCLUDE_CHARACTER = '-';
        private const char SEPARATOR_CHARACTER = ',';

        private readonly StringBuilder _stringBuilder;

        private bool _shallExclude;
        private bool _foundSeparator;
        private string _propertyName;
        private int _quoteCount;
        private Guid _groupId;


        public InterpretedSearch([NotNull] string searchString)
        {
            _stringBuilder = new StringBuilder();

            _groupId = Guid.NewGuid();
            _shallExclude = false;
            _foundSeparator = false;
            _propertyName = null;
            _quoteCount = 0;

            var phrases = CreateClassifiedFragments(searchString);
            AddRange(phrases);
        }

        private IEnumerable<Fragment> CreateClassifiedFragments([NotNull] string text)
        {
            using (var reader = new StringReader(text))
            {
                int readCharacter;
                while ((readCharacter = reader.Read()) != -1)
                {
                    var character = Convert.ToChar(readCharacter);
                    if (character == DOUBLE_QUOTE)
                    {
                        if (_quoteCount == 2)
                        {
                            var fragment = CreateClassifiedFragment();
                            if (fragment.TryGetValue(out var value))
                            {
                                yield return value;
                            }
                        }

                        _quoteCount++;
                        continue;
                    }

                    if (_quoteCount == 1)
                    {
                        _stringBuilder.Append(character);
                        continue;
                    }

                    // ReSharper disable once SwitchStatementMissingSomeCases
                    switch (character)
                    {
                        case EXCLUDE_CHARACTER:
                        {
                            if (_quoteCount == 2)
                            {
                                var fragment = CreateClassifiedFragment();
                                if (fragment.TryGetValue(out var value))
                                {
                                    yield return value;
                                }
                            }

                            _shallExclude = true;
                            continue;
                        }
                        case ACTION_CHARACTER:
                        {
                            _propertyName = ExtractPhrase();
                            _quoteCount = 0;
                            continue;
                        }
                        case SEPARATOR_CHARACTER:
                        {
                            _foundSeparator = true;
                            var fragment = CreateClassifiedFragment();
                            if (fragment.TryGetValue(out var value))
                            {
                                yield return value;
                            }
                            continue;
                        }
                    }

                    if (!char.IsLetterOrDigit(character))
                    {
                        var fragment = CreateClassifiedFragment();
                        if (fragment.TryGetValue(out var value))
                        {
                            yield return value;
                        }
                        continue;
                    }

                    _stringBuilder.Append(character);
                }

                var lastFragment = CreateClassifiedFragment();
                if (lastFragment.TryGetValue(out var lastValue))
                {
                    yield return lastValue;
                }
            }
        }

        [NotNull]
        private Option<Fragment> CreateClassifiedFragment()
        {
            var phrase = ExtractPhrase();
            if (string.IsNullOrWhiteSpace(phrase))
            {
                return Option.None<Fragment>();
            }

            Fragment fragment;
            if (_propertyName != null)
            {
                if (_propertyName == string.Empty)
                {
                    _propertyName = null;
                    return Option.None<Fragment>();
                }

                if (_shallExclude)
                {
                    if (_quoteCount > 0)
                    {
                        fragment = new ConstrainedExcludeFragment(FragmentType.Phrase, phrase, _propertyName);
                        _quoteCount = 0;
                    }
                    else
                    {
                        fragment = new ConstrainedExcludeFragment(FragmentType.Word, phrase, _propertyName);
                    }
                }
                else
                {
                    if (_quoteCount > 0)
                    {
                        fragment = new ConstrainedIncludeFragment(FragmentType.Phrase, phrase, _propertyName);
                        _quoteCount = 0;
                    }
                    else
                    {
                        fragment = new ConstrainedIncludeFragment(FragmentType.Word, phrase, _propertyName);
                    }
                }

                if (_foundSeparator)
                {
                    _foundSeparator = false;
                }
                else
                {
                    _shallExclude = false;
                    _propertyName = null;
                }
            }
            else
            {
                if (_shallExclude)
                {
                    if (_quoteCount > 0)
                    {
                        fragment = new ExcludeFragment(FragmentType.Phrase, phrase) { GroupId = _groupId };
                        _quoteCount = 0;
                    }
                    else
                    {
                        fragment = new ExcludeFragment(FragmentType.Word, phrase) { GroupId = _groupId };
                    }
                }
                else
                {
                    if (_quoteCount > 0)
                    {
                        fragment = new IncludeFragment(FragmentType.Phrase, phrase) { GroupId = _groupId };
                        _quoteCount = 0;
                    }
                    else
                    {
                        fragment = new IncludeFragment(FragmentType.Word, phrase) { GroupId = _groupId };
                    }
                }

                if (_foundSeparator)
                {
                    _foundSeparator = false;
                }
                else
                {
                    _groupId = Guid.NewGuid();
                    _shallExclude = false;
                }
            }

            return fragment;
        }

        private string ExtractPhrase()
        {
            var phrase = _stringBuilder.ToString();
            _stringBuilder.Clear();
            return phrase.Trim();
        }
    }
}