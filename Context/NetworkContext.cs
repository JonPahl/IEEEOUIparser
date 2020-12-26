/*
using IEEEOUIparser.Abstract;
using IEEEOUIparser.Options;
using LiteDB;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace IEEEOUIparser.Context
{
    public class NetworkContext : IContext
    {
        public ILiteDatabase Database { get; set; }
        private string CollectionName { get; }

        public string DbName => throw new System.NotImplementedException();

        public string Table { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public object DbContext { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public NetworkContext(IOptions<LiteDbOptions> options)
        {
            var location = options.Value.DatabaseLocation;
            Database = new LiteDatabase(location);
            CollectionName = "Oui";
        }

        public List<T> LoadAll<T>() => Database.GetCollection<T>(CollectionName).FindAll().ToList();

        public bool Merge<T>(T item)
        {
            var collection = Database.GetCollection<T>(CollectionName);
            return collection.Upsert(item);
        }
    }
}
*/