using System;
using System.Text.RegularExpressions;

namespace BDSA2020.Assignment02
{
    public static class Extensions
    {
        public static bool IsSecure( this Uri stuff)
        {
            Console.WriteLine("Started with " + stuff);
            if(stuff.Scheme.Equals("https")) return true;
            
            else return false;
        }

        public static int WordCount(this String input)
        {
            int counter = 0;
            string pattern = "\\b[^\\d\\W]+\\b";
            var matches = Regex.Matches(input,pattern);
            foreach(Match word in matches) counter++;
            return counter;
        }
        
    }
}
