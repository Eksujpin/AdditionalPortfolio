using Xunit;
using System;
using System.Collections.Generic;

namespace BDSA2019.Assignment01.Tests
{

  public class IteratorsTests
  {

    public static IEnumerable<IEnumerable<int>> getStreamsOfWholeNumbers()
    {
      yield return getOddNumbers();
      yield return getEvenNumbers();
      yield break;
    }


    public static IEnumerable<int> getNumbers()
    {
        int i = 0;

        while(i<10)
        {
            yield return i;
            i++;
        }
    }
    public static IEnumerable<int> getEvenNumbers()
    {
      int i = 2;
      while (i < 10)
      {
        yield return i;
        i += 2;
      }
    }

    public static IEnumerable<int> getOddNumbers()
    {
      int i = 1;
      while (i < 10)
      {
        yield return i;
        i += 2;
      }
    }

    [Fact]
    public void FlattenTest()
    {
      // Arrange
      IEnumerable<IEnumerable<int>> streamofStreams = getStreamsOfWholeNumbers();

      // Act
      List<int> numbers = new List<int>();
      IEnumerable<int> flattenedStream = Iterators.Flatten(streamofStreams);

      foreach (int n in flattenedStream)
      {
        numbers.Add(n);
      }

      // Assert
      int[] expectedNumbers = {
                1,3,5,7,9,
                2,4,6,8
            };
      Assert.Equal(expectedNumbers, numbers.ToArray());
    }


    [Fact]
    public void FilterTest()
    {
      //Arrange
        Predicate<int> even = Even;
        IEnumerable<int> numberStream = getNumbers();
        List<int> numbers = new List<int>();


      //Act
        IEnumerable<int> filteredNumbers = Iterators.Filter(numberStream, even);

        foreach (int n in filteredNumbers)
        {
        numbers.Add(n);
        }


      //Assert
        int[] expectedNumbers = {0,2,4,6,8};
        Assert.Equal(expectedNumbers, numbers.ToArray());


    }

    public static bool Even(int i)
    {
      return i % 2 == 0;
    }
  }
}
