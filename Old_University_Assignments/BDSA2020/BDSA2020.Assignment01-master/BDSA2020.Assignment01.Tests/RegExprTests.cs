using Xunit;
using System.Collections.Generic;

namespace BDSA2019.Assignment01.Tests
{
  public class RegExprTests
  {
    [Fact]
    public static void SplitLineTest()
    {
      // Arrange
      IEnumerable<string> lines = new string[] { "hello world", "lorem ipsum" };

      IEnumerable<string> expectedWords = new string[] { "hello", "world", "lorem", "ipsum" };

      // Act
      IEnumerable<string> words = RegExpr.SplitLine(lines);

      // Assert

      Assert.Equal(expectedWords, words);
    }

    [Fact]
    public static void ResolutionsTest()
    {
      // Arrange
      IEnumerable<string> inputs = new string[]{
        "1920x1080",
        "1024x768, 800x600, 640x480",
        "320x200, 320x240, 800x600",
        "1280x960"
      };
      IEnumerable<(int, int)> expectedOutputs = new (int, int)[]{
        (1920, 1080),
        (1024, 768),
        (800, 600),
        (640, 480),
        (320, 200),
        (320, 240),
        (800, 600),
        (1280, 960)
      };
      // Act
      IEnumerable<(int, int)> outputs = RegExpr.Resolutions(inputs);
      // Assert
      Assert.Equal(expectedOutputs, outputs);
    }

    [Theory]
    [InlineData(@"<a>hej</a>", "a", new string[] { "hej" })]
    [InlineData(@"<a>hej</a>", "a", new string[] { "hej" })]
    [InlineData(@"<div>
    <p>A <b>regular expression</b>, <b>regex</b> or <b>regexp</b> (sometimes called a <b>rational expression</b>) is, in <a href=""/wiki/Theoretical_computer_science"" title=""Theoretical computer science"">theoretical computer science</a> and <a href=""/wiki/Formal_language"" title=""Formal language"">formal language</a> theory, a sequence of <a href=""/wiki/Character_(computing)"" title=""Character (computing)"">characters</a> that define a <i>search <a href=""/wiki/Pattern_matching"" title=""Pattern matching"">pattern</a></i>. Usually this pattern is then used by <a href=""/wiki/String_searching_algorithm"" title=""String searching algorithm"">string searching algorithms</a> for ""find"" or ""find and replace"" operations on <a href=""/wiki/String_(computer_science)"" title=""String (computer science)"">strings</a>.</p>
</div>", "a", new string[]{
        "theoretical computer science",
        "formal language",
        "characters",
        "pattern",
        "string searching algorithms",
        "strings",
      })]
    [InlineData(@"<div><p>The phrase <i>regular expressions</i> (and consequently, regexes) is often used to mean the specific, standard textual syntax for representing <u>patterns</u> that matching <em>text</em> need to conform to.</p>
</div>", "p", new string[]{"The phrase regular expressions (and consequently, regexes) is often used to mean the specific, standard textual syntax for representing patterns that matching text need to conform to."})]
    public static void InnerTextTest(string input, string tag, IEnumerable<string> expectedTexts)
    {
      // Act
      IEnumerable<string> texts = RegExpr.InnerText(input, tag);
      // Assert
      Assert.Equal(expectedTexts, texts);
    }

  }

}
