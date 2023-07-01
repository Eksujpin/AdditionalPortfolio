using Xunit;

namespace BDSA2020.Assignment02.Tests
{
    public class WizardTests
    {
        [Fact]
        public void Wizards_contains_2_wizards()
        {
            var wizards = Wizard.Wizards.Value;

            Assert.Equal(10, wizards.Count);
        }

        [Theory]
        [InlineData("Darth Vader", "Star Wars", 1977, "George Lucas")]
        [InlineData("Sauron", "The Fellowship of the Ring", 1954, "J.R.R. Tolkien")]
        [InlineData("Yoda","Star Wars",1977,"George Lucas")]
        [InlineData("Albus Dumbledore","Harry Potter",2001,"J.K. Rowling")]
        [InlineData("Gargamel","Smurfs",1959,"Peyo")]
        [InlineData("Jafar","Aladin",1992,"Andreas Deja")]
        [InlineData("Lord Voldemort","Harry Potter",2001,"J.K. Rowling")]
        [InlineData("Glinda the Good","The Wonderful Wizard of Oz",1900,"L. Frank Baum")]
        [InlineData("Ursula","The Little Mermaid",1989,"H.C Andersen")]
        [InlineData("Tinkerbell","Peter Pan",1953,"J.M. Barrie")]
 
        public void Spot_check_wizards(string name, string medium, int year, string creator)
        {
            var wizards = Wizard.Wizards.Value;

            Assert.Contains(wizards, w =>
                w.Name == name &&
                w.Medium == medium &&
                w.Year == year &&
                w.Creator == creator
            );
        }
    }
}

