using IEEEOUIparser;
using LiteDB;
using System;
using System.Collections.Generic;

namespace OuiIeeeParser
{
    public class NetworkContext
    {
        protected string DbName { get; }
        protected string Coll { get; set; }

        public NetworkContext()
        {
            DbName = @"Filename=F:\Temp\NetworkData.db;Connection=shared;ReadOnly=false";
            Coll = "Oui";
        }

        public int Insert<T>(T item)
        {
            var x = item as OuiLookup;
            using (var db = new LiteDatabase(DbName))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<OuiLookup>(Coll);

                var result = col.Insert(x);

                return Convert.ToInt32(result.RawValue);
            }
        }

        public int Update<T>(T item)
        {
            var x = item as OuiLookup;
            using var db = new LiteDatabase(DbName);
            var col = db.GetCollection<OuiLookup>(Coll);
            var result = col.Update(x);
            return Convert.ToInt32(result);
        }


        public IList<OuiLookup> LoadAllOui()
        {
            using var db = new LiteDatabase(DbName);
            var col = db.GetCollection<OuiLookup>(Coll);
            var results = col.FindAll();
            return (IList<OuiLookup>)results;
        }


        public OuiLookup Find(string x)
        {
            using var db = new LiteDatabase(DbName);
            var col = db.GetCollection<OuiLookup>(Coll);

            var result = col.FindOne($"$.HexValue = '{x}'");
            return result;
        }

    }
}