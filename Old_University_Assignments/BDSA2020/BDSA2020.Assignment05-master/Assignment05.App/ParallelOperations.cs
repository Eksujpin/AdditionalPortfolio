using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

namespace Assignment05
{
    public class ParallelOperations
    {
        public static ICollection<long> Squares(long lowerBound, long upperBound)
        {
            ConcurrentBag<long> output = new ConcurrentBag<long>();

            Parallel.For(lowerBound, upperBound + 1, i =>
            {
                output.Add(i * i);
            });

            return output.ToArray();
        }

        public static IEnumerable<long> SquaresLinq(int start, int count)
        {
            var query = from n in Enumerable.Range(start, count * count).AsParallel()
                        where Enumerable.Range(start, count).Any(i => n == i*i)
                        select (long)n;
            return query.AsEnumerable();
        }



        public static void CreateThumbnails(IPictureModule resizer, IEnumerable<string> imageFiles, string outputFolder, Size size)
        {
            throw new NotImplementedException();
        }
    }
}
