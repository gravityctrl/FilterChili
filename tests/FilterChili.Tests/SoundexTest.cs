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

namespace GravityCTRL.FilterChili.Tests
{
    public class SoundexTest
    {
        [Fact]
        public void Test_Soundex_For_Breschnew()
        {
            "Breschnew".ToSoundex().Should().Be("B625");
        }

        [Fact]
        public void Test_Soundex_For_Luedenscheid()
        {
            "Müller-Lüdenscheidt".ToSoundex().Should().Be("M460 L352");
        }

        [Fact]
        public void Test_Soundex_For_Heinz_Classen()
        {
            "Heinz Classen".ToSoundex().Should().Be("H520 C425");
        }

        [Fact]
        public void Test_Soundex_For_Wakapodia()
        {
            "Wikipedia".ToSoundex().Should().Be("W213");
        }

        [Fact]
        public void Test_Soundex_For_Tuna()
        {
            "Tun".ToSoundex().Should().Be("T500");
            "Tuna".ToSoundex().Should().Be("T500");
        }

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
            "Tun".ToGermanSoundex().Should().Be("26");
            "Tuna".ToGermanSoundex().Should().Be("26");
        }
    }
}
