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

using FluentAssertions;
using GravityCTRL.FilterChili.Search;
using GravityCTRL.FilterChili.Search.Fragments;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Search
{
    public sealed class FragmentedSearchTest
    {
        [Fact]
        public void Should_Exclude_Ignore_Whitespaces_Items()
        {
            var result = new FragmentedSearch("  ABC  def  ");
            result.Should().BeEquivalentTo
            (
                new IncludeFragment(FragmentType.Word, "ABC"), 
                new IncludeFragment(FragmentType.Word, "def")
            );
        }

        [Fact]
        public void Should_Ignore_Non_AlphaNumeric_Items()
        {
            var result = new FragmentedSearch(" name.category! ?Foobert_ ");
            result.Should().BeEquivalentTo
            (
                new IncludeFragment(FragmentType.Word, "name"),
                new IncludeFragment(FragmentType.Word, "category"),
                new IncludeFragment(FragmentType.Word, "Foobert")
            );
        }

        [Fact]
        public void Should_Respect_Double_Quotes()
        {
            var result = new FragmentedSearch(@" ""name.category!"" ?Foobert_ ");
            result.Should().BeEquivalentTo
            (
                new IncludeFragment(FragmentType.Phrase, "name.category!"),
                new IncludeFragment(FragmentType.Word, "Foobert")
            );
        }

        [Fact]
        public void Should_Create_Phrases_If_Number_Of_Double_Quotes_Is_Odd()
        {
            var result = new FragmentedSearch(@" ""name.category!"" ""?Foobert_ ");
            result.Should().BeEquivalentTo
            (
                new IncludeFragment(FragmentType.Phrase, "name.category!"),
                new IncludeFragment(FragmentType.Phrase, "?Foobert_")
            );
        }

        [Fact]
        public void Should_Create_Phrases_If_Double_Quotes_Are_Not_Delimited()
        {
            var result = new FragmentedSearch(@" ""name.category!""""?Foobert_"" ");
            result.Should().BeEquivalentTo
            (
                new IncludeFragment(FragmentType.Phrase, "name.category!"),
                new IncludeFragment(FragmentType.Phrase, "?Foobert_")
            );
        }

        [Fact]
        public void Should_Create_Phrases_If_Double_Quotes_Are_Followed_By_Exclude_Statement()
        {
            var result = new FragmentedSearch(@" ""name.category!""-Foobert ");
            result.Should().BeEquivalentTo
            (
                new IncludeFragment(FragmentType.Phrase, "name.category!"),
                new ExcludeFragment(FragmentType.Word, "Foobert")
            );
        }

        [Fact]
        public void Should_Allow_Marking_Word_As_Unwanted()
        {
            var result = new FragmentedSearch(@" -name.category! ?Foobert_ ");
            result.Should().BeEquivalentTo
            (
                new ExcludeFragment(FragmentType.Word, "name"),
                new IncludeFragment(FragmentType.Word, "category"),
                new IncludeFragment(FragmentType.Word, "Foobert")
            );
        }

        [Fact]
        public void Should_Allow_Marking_Phrase_As_Unwanted()
        {
            var result = new FragmentedSearch(@" -""name.category!"" ?Foobert_ ");
            result.Should().BeEquivalentTo
            (
                new ExcludeFragment(FragmentType.Phrase, "name.category!"),
                new IncludeFragment(FragmentType.Word, "Foobert")
            );
        }

        [Fact]
        public void Should_Create_Constrained_Include_Fragment_If_Colon_Is_Used_As_Separator()
        {
            var result = new FragmentedSearch(@" name:category! ?Foobert_ ");
            result.Should().BeEquivalentTo
            (
                new ConstrainedIncludeFragment(FragmentType.Word, "category", "name"),
                new IncludeFragment(FragmentType.Word, "Foobert")
            );
        }

        [Fact]
        public void Should_Not_Create_Constrained_Include_Fragment_If_Constraint_Name_Is_Empty()
        {
            var result = new FragmentedSearch(@" :category! ?Foobert_ ");
            result.Should().BeEquivalentTo
            (
                new IncludeFragment(FragmentType.Word, "Foobert")
            );
        }

        [Fact]
        public void Should_Create_Constrained_Exclude_Fragment_If_Colon_Is_Used_As_Separator()
        {
            var result = new FragmentedSearch(@" -name:category! ?Foobert_ ");
            result.Should().BeEquivalentTo
            (
                new ConstrainedExcludeFragment(FragmentType.Word, "category", "name"),
                new IncludeFragment(FragmentType.Word, "Foobert")
            );
        }

        [Fact]
        public void Should_Not_Create_Constrained_Exclude_Fragment_If_Constraint_Name_Is_Empty()
        {
            var result = new FragmentedSearch(@" -:category! ?Foobert_ ");
            result.Should().BeEquivalentTo
            (
                new IncludeFragment(FragmentType.Word, "Foobert")
            );
        }

        [Fact]
        public void Should_Create_Constrained_Include_Fragment_Using_Quotes_For_Constraint()
        {
            var result = new FragmentedSearch(@" ""specific name"":category! ?Foobert_ ");
            result.Should().BeEquivalentTo
            (
                new ConstrainedIncludeFragment(FragmentType.Word, "category", "specific name"),
                new IncludeFragment(FragmentType.Word, "Foobert")
            );
        }

        [Fact]
        public void Should_Create_Constrained_Exclude_Fragment_Using_Quotes_For_Constraint()
        {
            var result = new FragmentedSearch(@" -""specific name"":category! ?Foobert_ ");
            result.Should().BeEquivalentTo
            (
                new ConstrainedExcludeFragment(FragmentType.Word, "category", "specific name"),
                new IncludeFragment(FragmentType.Word, "Foobert")
            );
        }

        [Fact]
        public void Should_Create_Constrained_Include_Fragment_Using_Quotes_For_Text()
        {
            var result = new FragmentedSearch(@" name:""specific category!"" ?Foobert_ ");
            result.Should().BeEquivalentTo
            (
                new ConstrainedIncludeFragment(FragmentType.Phrase, "specific category!", "name"),
                new IncludeFragment(FragmentType.Word, "Foobert")
            );
        }

        [Fact]
        public void Should_Create_Constrained_Exclude_Fragment_Using_Quotes_For_Text()
        {
            var result = new FragmentedSearch(@" -name:""specific category!"" ?Foobert_ ");
            result.Should().BeEquivalentTo
            (
                new ConstrainedExcludeFragment(FragmentType.Phrase, "specific category!", "name"),
                new IncludeFragment(FragmentType.Word, "Foobert")
            );
        }
    }
}
