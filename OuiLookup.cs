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
    }
}
