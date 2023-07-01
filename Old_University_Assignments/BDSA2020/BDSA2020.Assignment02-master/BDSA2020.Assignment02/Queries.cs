using System;
using System.Linq;
using System.Collections.Generic;
namespace BDSA2020.Assignment02
{
    public class Queries
    {
        //1 with LINQ
       public static IEnumerable<string> checkRowlingLinq()
        {
            var wizzByRowling = from w in Wizard.Wizards.Value
                                where w.Creator.Contains("J.K. Rowling")
                                select w.Name;
            
            return wizzByRowling;
        }

        //1 with Extensions
        public static IEnumerable<string> checkRowlingExtension()
        {
            var wizzByRowling2 = Wizard.Wizards.Value
                                .Where(w => w.Creator.Contains("J.K. Rowling"))
                                .Select(w => w.Name); 
            
            return wizzByRowling2;
        }

        //2 with LINQ
        public static int? firstSithLordLinq()
        {
            var darthWho = from w in Wizard.Wizards.Value
                            where w.Name.Contains("Darth")
                            orderby w.Year descending
                            select w.Year;

             return darthWho.First();
        }

        //2 with Extensions
        public static int? firstSithLordExtension()
        {
            var darthWho2 = Wizard.Wizards.Value
                            .Where(w => w.Name.Contains("Darth"))
                            .OrderByDescending(w => w.Year)
                            .Select(w => w.Year);

            return darthWho2.First();
        }

        //3 with Linq
        public static void hpWizardsLinq()
        {
            var wizz = from w in Wizard.Wizards.Value
                where w.Medium.Contains("Harry Potter")
                select w.Year + " " + w.Name;

                foreach (var w in wizz)
                {
                    Console.WriteLine(w);
                }
        }

        //3 with Extensions
        public static void hpWizardsExtensions()
        {
            var wizz = Wizard.Wizards.Value
                    .Where(w => w.Medium.Contains("Harry Potter"))
                    .Select(w => w.Name + " " + w.Year);

                foreach (var w in wizz)
                {
                    Console.WriteLine(w);
                }
        }


        //4 with LINQ
        public static void creatorSort()
        {
            var sorted = from w in Wizard.Wizards.Value
                         orderby w.Name
                         group w.Name by w.Creator
                            into creatorGroups
                         orderby creatorGroups.Key descending
                         select creatorGroups;

            //maybe bad print statement as this should return something for testing not print 
            foreach (var wiz in sorted)
            {
                Console.WriteLine("Creator: " + wiz.Key);
                foreach (var name in wiz) Console.WriteLine(name);
            }
        }

        //4.Extensions
        public static void creatorSort2()
        {
            var sorted = Wizard.Wizards.Value
                        .OrderByDescending(w => w.Creator)
                        .ThenBy(w => w.Name)
                        .GroupBy(w => w.Creator);
                      
            //maybe bad print statement as this should return something for testing not print 
            foreach (var wiz in sorted)
            {
                Console.WriteLine("Creator: " + wiz.Key);
                foreach (var name in wiz) Console.WriteLine(name.Name);
            }
        }
    }
}
