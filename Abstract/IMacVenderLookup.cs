using System.Collections.Generic;

namespace IEEEOUIparser
{
    public interface IMacVenderLookup
    {
        List<OuiLookup> ParseFile();
    }
}