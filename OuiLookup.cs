namespace IEEEOUIparser
{
    public class OuiLookup: BaseLookup
    {
        public string HexValue { get; set; }
        public string Base16Value { get; set; }
        public string Manufacturer { get; set; }

        internal string GetManufacturer()
        {
            return (Manufacturer != null) ? Manufacturer.Replace("\r\n","  ").Replace("\r","").Replace("\n","") : "";
        }
    }
}
