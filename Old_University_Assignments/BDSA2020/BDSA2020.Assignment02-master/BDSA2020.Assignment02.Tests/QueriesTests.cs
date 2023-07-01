using System;
using System.Collections.Generic;
using Xunit;

namespace BDSA2020.Assignment02.Tests
{
    public class QueriesTests
    {
        [Fact]
        public void Return_Wizz_By_Rowling_With_LINQ()
        {
            var wizards = Wizard.Wizards.Value;

            var output = Queries.checkRowlingLinq();
            var expected = new List<string> {"Albus Dumbledore", "Lord Voldemort"};

            Assert.Equal(expected, output);
        }

        [Fact]
        public void Return_Wizz_By_Rowling_With_Extensions()
        {
            var wizards = Wizard.Wizards.Value;

            var output = Queries.checkRowlingExtension();
            var expected = new List<string> {"Albus Dumbledore", "Lord Voldemort"};

            Assert.Equal(expected, output);
        }
        
        [Fact]
        public void Return_Year_Of_First_SithLord_With_LINQ()
        {
            var wizards = Wizard.Wizards.Value;

            var output = Queries.firstSithLordLinq();
            var expected = 1977;

            Assert.Equal(expected, output);
        }

        [Fact]
        public void Return_Year_Of_First_SithLord_With_Extention()
        {
            var wizards = Wizard.Wizards.Value;

            var output = Queries.firstSithLordExtension();
            var expected = 1977;

            Assert.Equal(expected, output);
        }
    }
}
