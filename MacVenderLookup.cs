using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace IEEEOUIparser
{
    public class MacVenderLookup
    {
        private readonly string OiuUrl = "http://standards-oui.ieee.org/oui/oui.txt";
        private readonly RegexOptions options = RegexOptions.Multiline;

        private readonly string HexPattern = @"([0-9A-F]{2}-[0-9A-F]{2}-[0-9A-F]{2})\s+\(hex\)\s+(.+)";
        private readonly string Base16Pattern = @"([0-9A-F]{2}[0-9A-F]{2}[0-9A-F]{2})\s+\(base 16\)\s+(.+)";

        public List<OuiLookup> ParseFile()
        {
            var results = new List<OuiLookup>();

            var contents = "";
            using (var wc = new WebClient())
            {
                contents = wc.DownloadString(OiuUrl);
            }

            var hexValues = ParseLine(contents, HexPattern, "Hex");
            var baseValues = ParseLine(contents, Base16Pattern, "Base16");

            foreach (var hex in hexValues)
            {
                var base16Value = baseValues
                    .Find(x => x.Item2 == hex.Item2.Replace("-", "") && x.Item3 == hex.Item3);

                var item = new OuiLookup();
                item.Id = item.GetHashCode();
                item.HexValue = hex.Item2;
                item.Base16Value = base16Value.Item2;
                item.Manufacturer = hex.Item3;

                results.Add(item);
                var pct = Math.Round(Convert.ToDecimal(results.Count) / hexValues.Count * 100, 2);
                Console.Write($"Percent Processed {pct:N} %");
                Console.SetCursorPosition(0, 0);
            }
            return results;
        }

        private List<Tuple<string, string, string>> ParseLine(string line, string pattern, string type)
        {
            var matches = new List<Tuple<string, string, string>>();

            foreach (Match m in Regex.Matches(line, pattern, options))
            {
                var mac = m.Groups[1].ToString();
                var Organization = m.Groups[2].ToString();

                var manufacturer = Organization.Replace("\r\n", "  ").Replace("\r", "").Replace("\n", "");

                matches.Add(new Tuple<string, string, string>(type, mac, manufacturer));
            }

            return matches;
        }
    }
}
