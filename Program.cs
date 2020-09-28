using OuiIeeeParser;
using System;
using System.Collections.Generic;

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
            StoreResults(Items);
        }


        private static void StoreResults(List<OuiLookup> Items)
        {
            var ctx = new NetworkContext();
            foreach (var item in Items)
            {
                try
                {
                    Console.WriteLine($"{item.Id}: {item.HexValue}\t{item.Base16Value}\t{item.Manufacturer}");
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
