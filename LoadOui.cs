using IEEEOUIparser.Abstract;
using IEEEOUIparser.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace IEEEOUIparser
{
    public class LoadOui
    {
        public IContext NetworkContext { get; set; }

        private readonly IMacVenderLookup OuiImporter;
        private List<OuiLookup>[] Pages { get; set; }
        private SettingOptions Setting { get; }

        public LoadOui(IContext database,
            IMacVenderLookup macVender,
            IOptions<SettingOptions> setting)
        {
            NetworkContext = database;
            OuiImporter = macVender;
            Setting = setting.Value;
        }

        public void RunService()
        {
            var Items = OuiImporter.ParseFile();
            Console.WriteLine("");
            StoreNewResults(Items);
            DisplayResults(Items, Setting.PageSize);
        }

        private void DisplayResults(List<OuiLookup> Items, int PageSize = 0)
        {
            string table = "";

            if (PageSize.Equals(0))
            {
                table = Items.ToStringTable(
                    new[] { "ID", "HEX VALUE", "BASE 16 VALUE", "MANUFACTURER" },
                    a => a.Id, a => a.HexValue, a => a.Base16Value, a => a.GetManufacturer());

                Console.Clear();
                Console.SetCursorPosition(0, 0);
                Console.WriteLine(table);
            }
            else if (PageSize > 0 && PageSize <= Items.Count)
            {
                Pages = SplitList(Items, PageSize).ToArray();
                var cnt = 1;

                while (cnt != Pages.Length - 1)
                {
                    if (cnt > 0 && cnt <= Pages.Length - 1)
                    {
                        PrintDisplay(cnt);
                        Console.WriteLine($"Enter for more results. page: {cnt} of {Pages.Length - 1}");

                        var key = Console.ReadKey().Key;
                        cnt = KeyPaging(key, cnt);
                        PrintDisplay(cnt);
                    }
                    Thread.Sleep(500);
                }
            }
            else
            {
                throw new Exception("Page Size is invalid, It must be greater than Zero or less than the size of the collection.");
            }
        }

        private int KeyPaging(ConsoleKey key, int cnt)
        {
            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    cnt--;
                    break;
                case ConsoleKey.RightArrow:
                    cnt++;
                    break;
                case ConsoleKey.Enter:
                    cnt += Setting.PageSize; //change to pageSize
                    break;
                case ConsoleKey.Home:
                    cnt = 1;
                    break;
                case ConsoleKey.Escape:
                case ConsoleKey.End:
                    cnt = Pages.Length - 1;
                    break;
                default:
                    cnt++;
                    break;
            }
            return cnt;
        }

        private void PrintDisplay(int cnt)
        {
            var page = Pages[cnt];

            var table = page.ToStringTable(
            new[] { "ID", "HEX VALUE", "BASE 16 VALUE", "MANUFACTURER" },
            a => a.Id, a => a.HexValue, a => a.Base16Value, a => a.GetManufacturer());

            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(table);
        }

        private void StoreNewResults(List<OuiLookup> Items)
        {
            //var missing = FindMissingValues(Items);
            ProcessRecords(Items);
        }

        private void ProcessRecords(List<OuiLookup> Items)
        {
            var added = 0;
            var existing = 0;

            foreach (var item in Items)
            {
                try
                {
                    Console.SetCursorPosition(0, 2);
                    var result = NetworkContext.Merge(item);
                    Console.Write($"Records Added: {added:N0}");
                    added++;
                }
                catch (LiteDB.LiteException ex)
                {
                    if (ex.Message.StartsWith("Cannot insert duplicate key in unique index '_id'."))
                    {
                        Console.SetCursorPosition(0, 3);
                        Console.WriteLine($"Existing Record: {existing:N0}");
                        existing++;
                    }
                    else
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    var totalProcessed = Convert.ToDecimal(added + existing);
                    var pct = Math.Round((totalProcessed / Items.Count) * 100, 2);
                    Console.SetCursorPosition(0, 4);
                    Console.Write($"Percent Processed: {totalProcessed:N0}/{Items.Count:N0} \t\t {pct:N} %");
                }
            }
        }

        /*
        private List<OuiLookup> FindMissingValues(List<OuiLookup> ouiLookups)
        {
            var stored = NetworkContext.LoadAll<OuiLookup>();
            return ouiLookups.Where(x => stored.All(x2 => x2.HexValue != x.HexValue)).ToList();
        }*/

        public static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }
    }
}