using System.ComponentModel.DataAnnotations;

namespace IEEEOUIparser
{
    public class OuiLookup
    {
        [Key]
        public int Id { get; set; }
        public string HexValue { get; set; }
        public string Base16Value { get; set; }
        public string Manufacturer { get; set; }

        internal string GetManufacturer()
        {
            if (this.Manufacturer != null)
            {
                var nw = Manufacturer.Replace("\r\n", "  ").Replace("\r", "").Replace("\n", "");
                return nw;
            } else
            {
                return "";
            }
        }
    }
}
