using IEEEOUIparser.Options;
using LiteDB;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace IEEEOUIparser
{
    public class NetworkContext : Abstract.ILiteDbContext
    {
        public ILiteDatabase Database { get; set; }
        private string CollectionName { get; }

        public NetworkContext(IOptions<LiteDbOptions> options)
        {
            var location = options.Value.DatabaseLocation;
            Database = new LiteDatabase(location);
            CollectionName = "Oui";
        }

        public List<T> LoadAll<T>()
        {
            return Database.GetCollection<T>(CollectionName).FindAll().ToList();
        }

        public bool Merge<T>(T item)
        {
            var collection = Database.GetCollection<T>(CollectionName);
            return collection.Upsert(item);
        }
    }
}