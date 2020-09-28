using OuiIeeeParser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IEEEOUIparser
{
    public static class Program
    {
        private static MacVenderLookup OuiImporter;

        public static void Main()
        {
            //var r = ctx.Find("6C-2B-59");
            //Console.WriteLine(r.Manufacturer);

            OuiImporter = new MacVenderLookup();
            var Items = OuiImporter.ParseFile();
            //StoreResults(Items); Make be a backgroundworker.
            DisplayResults(Items);
        }

        private static void DisplayResults(List<OuiLookup> Items)
        {
            //todo: Add in Paging of results.
            var table = Items.ToStringTable(
                new[] { "ID", "HEX VALUE", "BASE 16 VALUE", "MANUFACTURER"},
                a => a.Id, a => a.HexValue, a => a.Base16Value, a=>a.GetManufacturer()); //a => a.Manufacturer
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine(table);
        }

        private static void StoreResults(List<OuiLookup> Items)
        {
            var ctx = new NetworkContext();
            foreach (var item in Items)
            {
                try
                {
                    //Console.WriteLine($"{item.Id}: {item.HexValue}\t{item.Base16Value}\t{item.Manufacturer}");
                    var r = ctx.Insert(item);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
