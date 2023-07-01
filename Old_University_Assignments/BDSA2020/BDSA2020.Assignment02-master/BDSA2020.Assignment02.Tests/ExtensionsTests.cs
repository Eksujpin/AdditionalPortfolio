using System;
using Xunit;

namespace BDSA2020.Assignment02.Tests
{
    public class ExtensionsTests
    {
        [Fact]
        public void Test_if_secure()
        {
            //arrange
            Uri http = new Uri("http://www.contoso.com/");
            Uri https = new Uri("https://www.youtube.com/");

            //assert
            Assert.Equal(false, http.IsSecure());
            Assert.Equal(true, https.IsSecure());

        }
        
        [Fact]
        public void Test_number_count()
        {
            //arrange
            String word0 = "this is string number 1, so hOw many words is it";
            String word1 = "12 13 14 15 16 !!!!! test my string ## Stupid 111";

            //assert
            Assert.Equal(10,word0.WordCount());
            Assert.Equal(4,word1.WordCount());

        }
    }
}
