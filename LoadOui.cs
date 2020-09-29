using IEEEOUIparser.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace IEEEOUIparser
{
    public class LoadOui
    {
        public ILiteDbContext NetworkContext { get; set; }

        private readonly MacVenderLookup OuiImporter;
        private List<OuiLookup>[] pages;

        public LoadOui(ILiteDbContext database)
        {
            NetworkContext = database;
            OuiImporter = new MacVenderLookup();
        }

        public void RunService()
        {
            try
            {
                var Items = OuiImporter.ParseFile();
                Console.WriteLine("");
                StoreNewResults(Items);
                DisplayResults(Items, 59);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
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
                pages = SplitList(Items, PageSize).ToArray();
                var cnt = 1;

                while (cnt != pages.Length - 1)
                {
                    if (cnt > 0 && cnt <= pages.Length - 1)
                    {
                        PrintDisplay(cnt);
                        Console.WriteLine($"Enter for more results. page: {cnt} of {pages.Length - 1}");

                        switch (Console.ReadKey().Key)
                        {
                            case ConsoleKey.LeftArrow:
                                cnt--;
                                break;
                            case ConsoleKey.RightArrow:
                                cnt++;
                                break;
                            case ConsoleKey.Enter:
                                cnt += 10;
                                break;
                            case ConsoleKey.Home:
                                cnt = 1;
                                break;
                            case ConsoleKey.End:
                                cnt = pages.Length - 1;
                                break;
                            default:
                                cnt++;
                                break;
                        }
                        PrintDisplay(cnt);
                    }
                    else
                    {
                        //var x = 0;
                    }
                    Thread.Sleep(500);
                }
            }
            else
            {
                throw new Exception("Page Size is invalid, It must be greater than Zero or less than the size of the collection.");
            }
        }

        private void PrintDisplay(int cnt)
        {
            var page = pages[cnt];

            var table = page.ToStringTable(
            new[] { "ID", "HEX VALUE", "BASE 16 VALUE", "MANUFACTURER" },
            a => a.Id, a => a.HexValue, a => a.Base16Value, a => a.GetManufacturer());

            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(table);
        }

        private void StoreAllResults(List<OuiLookup> Items)
        {
            ProcessRecords(Items);
        }

        private void StoreNewResults(List<OuiLookup> Items)
        {
            var missing = FindMissingValues(Items);
            ProcessRecords(missing);
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

        private List<OuiLookup> FindMissingValues(List<OuiLookup> ouiLookups)
        {
            var stored = NetworkContext.LoadAll<OuiLookup>();
            return ouiLookups.Where(x => stored.All(x2 => x2.HexValue != x.HexValue)).ToList();
        }

        public static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }
    }
}