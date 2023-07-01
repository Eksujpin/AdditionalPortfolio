using System;
using System.Collections.Generic;
using System.Linq;

namespace BDSA2020.Assignment02
{
    class Program
    {

        static String revers(String input)
        {
            char[] temp = input.ToCharArray();
            Array.Reverse(temp);
            return new string (temp);
        }
            /*Func<String,String> test1 = revers;
            String result = test1("testing");
            Console.WriteLine(result); */

        static double product(double x, double y) => x*y;
            /*Func<double, double, double> mathTool = product;
            double test = mathTool(7.18,9.13);
            Console.WriteLine(test);*/
        static bool compare (String nr, int x)
        {
            int y;
            bool success = Int32.TryParse(nr,out y);
            if(success)
            {
                return y.Equals(x);
            }
            else return false;
        }
            /*Func<String, int, bool> testingTool = compare;
            bool result = testingTool(" 00042", 42);
            Console.WriteLine(result);*/

        static void Main(string[] args)
        {
         Queries.hpWizardsExtensions();
        }
    }
}
