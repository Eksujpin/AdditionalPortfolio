using System;
using System.Linq;
using Xunit;

namespace Assignment05.App.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void ParallelOperations_square_lower_1_upper_5()
        {
            Array expected = new long[1, 4, 9, 16, 25];

            var t1 = ParallelOperations.Squares(1, 5);

            Assert.True(t1.Contains(1));
            Assert.True(t1.Contains(4));
            Assert.True(t1.Contains(9));
            Assert.True(t1.Contains(16));
            Assert.True(t1.Contains(25));

        }


        [Fact]
        public void ParallelOperations_SquareLinq_lower_1_upper_5()
        {
            Array expected = new long[1, 4, 9, 16, 25];

            var t1 = ParallelOperations.SquaresLinq(1, 5);
            var res = t1.ToList();
            
            Assert.True(res.Contains(1));
            Assert.True(res.Contains(4));
            Assert.True(res.Contains(9));
            Assert.True(res.Contains(16));
            Assert.True(res.Contains(25));
        }
    }
}
