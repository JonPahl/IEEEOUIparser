using System.Collections.Generic;
using System.Threading.Tasks;

namespace IEEEOUIparser.Abstract
{
    public interface IContext
    {
        public string Connection { get; }
        public string DbName { get; }
        public string Table { get; set; }
        public object DbContext { get; set; }
        public Task<List<T>> LoadAll<T>() where T : BaseLookup;
        public Task<bool> Merge<T>(T item) where T : BaseLookup;
    }
}