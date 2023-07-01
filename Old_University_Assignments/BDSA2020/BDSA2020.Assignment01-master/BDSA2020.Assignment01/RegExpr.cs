using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BDSA2019.Assignment01
{
  public static class RegExpr
  {

    static string wordPattern = @"[A-Za-z0-9]+";
    public static IEnumerable<string> SplitLine(IEnumerable<string> lines)
    {
      foreach(string line in lines)
      {
          var matches = Regex.Matches(line, wordPattern);

          foreach(Match m in matches) {
            yield return m.Value;
          }
      }
    }

    static string resolutionPattern = @"(?<width>\d+)x(?<height>\d+)";

    public static IEnumerable<(int width, int height)> Resolutions(IEnumerable<string> resolutions)
    {
      foreach (string input in resolutions ) {
        var matches = Regex.Matches(input.Trim(), resolutionPattern);

        foreach(Match match in matches) {
          bool parsedWidth = int.TryParse(match.Groups["width"].Value, out int width);
          bool parsedHeight = int.TryParse(match.Groups["height"].Value, out int height);
          if (parsedWidth && parsedHeight) yield return (width, height);
        }
      }
    }

    public static IEnumerable<string> InnerText(string html, string tag)
    {
      string innerTextPattern = $@"<{tag}.*?>(?<innerText>.*?)<\/{tag}>";
      string tagPattern = "<.*?>";
      var matches = Regex.Matches(html, innerTextPattern);
      foreach (Match m in matches)
      {
        yield return Regex.Replace(m.Groups["innerText"].Value, tagPattern, "");
      }
    }
  }
}
