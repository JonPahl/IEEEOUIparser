using IEEEOUIparser.Abstract;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IEEEOUIparser.Context
{
    public class MongoDbContext : IContext
    {
        public string DbName {get; }

        public string Table { get; set; }
        public object DbContext { get; set; }
        public string Connection { get; }

        public IMongoClient Database { get; }

        public MongoDbContext()
        {
            Connection = "mongodb://192.168.1.28:27017";
            DbName = "NetworkScanner";
            Table = "OUI";
            Database = new MongoClient(Connection);
            // Database as IMongoClient;
        }

        public async Task<List<T>> LoadAll<T>() where T : BaseLookup
        {
            //try
            //{
                var collection = Database.GetDatabase(DbName).GetCollection<T>(typeof(T).Name);
                return await collection.Find(_ => true).ToListAsync().ConfigureAwait(false);
            //}
            //catch (MongoException ex)
            //{
            //    Console.WriteLine(ex.Message, ex);
            //    return (T)Convert.ChangeType(-1, typeof(T));
            //}
        }

        public async Task<bool> Merge<T>(T item) where T : BaseLookup
        {
            var collection = Database.GetDatabase(DbName).GetCollection<T>(typeof(T).Name);

            //var id = item.GetType().GetProperty("Id").GetValue(item);
            var filter = Builders<T>.Filter.Where(x => x.Id == item.Id);

            ReplaceOptions options = new ReplaceOptions { IsUpsert = true };

            var result = await collection.ReplaceOneAsync(filter, item, options).ConfigureAwait(false);
            return result.IsAcknowledged;
        }

        /*
        BsonDocument doc = new BsonDocument();
        public string HexValue { get; set; }
    public string Base16Value { get; set; }
    public string Manufacturer { get; set; }
    doc["firstName"] = customer.FirstName;
        doc["lastName"] = customer.LastName;
        doc["email"] = customer.Email;
        */

        //error thrown here on the ReplaceOneAsync params..
        //try
        //{
        //    using LiteDatabase liteDatabase = (LiteDatabase)_dbContext.Database;
        //    var col = liteDatabase.GetCollection<T>(Table);
        //    col.EnsureIndex(x => x.Id, true);

        //    var result = await Task.Run(() => col.Insert(entity)).ConfigureAwait(false);
        //    return (T)Convert.ChangeType(result.RawValue, typeof(T));
        //}
        //catch (LiteException ex)
        //{
        //    Console.WriteLine(ex.Message, ex);
        //    return (T)Convert.ChangeType(-1, typeof(T));
        //}
    }
}
