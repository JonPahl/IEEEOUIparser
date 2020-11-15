using IEEEOUIparser.Exceptions;
using IEEEOUIparser.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace IEEEOUIparser
{
    public class MacVenderLookup : IMacVenderLookup
    {
        private string OiuUrl { get; }
        private readonly RegexOptions options = RegexOptions.Multiline;
        private string HexPattern { get; }
        private string Base16Pattern { get; }

        public MacVenderLookup(IOptions<RegExOptions> Option,
            IOptions<SettingOptions> Settings)
        {
            OiuUrl = Settings.Value.Url;
            HexPattern = Option.Value.Hex;
            Base16Pattern = Option.Value.Base16;
        }

        public List<OuiLookup> ParseFile()
        {
            var results = new List<OuiLookup>();
            //try {
            var contents = LoadRemoteFile(OiuUrl);

            var hexValues = ParseLine(contents, HexPattern, "Hex");
            var baseValues = ParseLine(contents, Base16Pattern, "Base16");

            foreach (var hex in hexValues)
            {
                var base16Value = baseValues
                    .Find(x => x.Item2.Equals(hex.Item2.Replace("-", "")) &&
                    x.Item3.Equals(hex.Item3));

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
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }

        private string LoadRemoteFile(string uri)
        {
            try
            {
                using var wc = new WebClient();
                return wc.DownloadString(uri);
            }
            catch (Exception ex)
            {
                throw new GeneralException(ex.Message, ex);
            }
        }

        private List<Tuple<string, string, string>> ParseLine(string line, string pattern, string type)
        {
            var matches = new List<Tuple<string, string, string>>();

            foreach (Match m in Regex.Matches(line, pattern, options))
            {
                var mac = m.Groups[1].ToString();
                var org = m.Groups[2].ToString();

                var manufacturer = org.Replace("\r\n", "  ").Replace("\r", "").Replace("\n", "");

                matches.Add(new Tuple<string, string, string>(type, mac, manufacturer));
            }

            return matches;
        }
    }
}
