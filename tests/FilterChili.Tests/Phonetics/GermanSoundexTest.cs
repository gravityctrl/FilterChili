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
using GravityCTRL.FilterChili.Phonetics;
using Xunit;

namespace GravityCTRL.FilterChili.Tests.Phonetics
{
    public sealed class GermanSoundexTest
    {
        [Fact]
        public void Test_German_Soundex_For_Breschnew()
        {
            "Breschnew".ToGermanSoundex().Should().Be("17863");
        }

        [Fact]
        public void Test_German_Soundex_For_Luedenscheid()
        {
            "Müller-Lüdenscheidt".ToGermanSoundex().Should().Be("657 52682");
        }

        [Fact]
        public void Test_German_Soundex_For_Heinz_Classen()
        {
            "Heinz Classen".ToGermanSoundex().Should().Be("068 4586");
        }

        [Fact]
        public void Test_German_Soundex_For_Wakapodia()
        {
            "Wikipedia".ToGermanSoundex().Should().Be("3412");
        }

        [Fact]
        public void Test_German_Soundex_For_Tuna()
        {
            "Tuna".ToGermanSoundex().Should().Be("26");
        }

        [Fact]
        public void Test_German_Soundex_For_Wortwitz()
        {
            "Wortwitz".ToGermanSoundex().Should().Be("37238");
        }

        [Fact]
        public void Test_German_Soundex_For_Lack()
        {
            "Lack".ToGermanSoundex().Should().Be("54");
        }

        [Fact]
        public void Test_German_Soundex_For_Xylophon()
        {
            "Xylophon".ToGermanSoundex().Should().Be("48536");
        }

        [Fact]
        public void Test_German_Soundex_For_Lockx()
        {
            "Lockx".ToGermanSoundex().Should().Be("548");
        }
    }
}
