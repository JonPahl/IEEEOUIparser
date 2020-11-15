using LiteDB;
using System.Collections.Generic;

namespace IEEEOUIparser.Abstract
{
    public interface ILiteDbContext
    {
        public ILiteDatabase Database { get; set; }
        public List<T> LoadAll<T>();
        public bool Merge<T>(T item);
    }
}