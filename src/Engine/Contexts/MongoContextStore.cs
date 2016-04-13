using System;
using System.Collections.Generic;
using System.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Engine.Contexts
{
    public class MongoContextStore : IContextStore
    {
        private IMongoCollection<Context> _contextCollection;

        public MongoContextStore()
        {
            InitConnection();
        }

        private void InitConnection()
        {
            var url = ConfigurationManager.AppSettings["MongoDbUrl"];
            var databaseName = ConfigurationManager.AppSettings["MongoDbDatabase"];
            var collection = ConfigurationManager.AppSettings["MongoDbContextCollection"];

            var client = new MongoClient(new MongoUrl(url));
            var database = client.GetDatabase(databaseName);
            _contextCollection = database.GetCollection<Context>(collection);
        }

        public void Insert(Context context)
        {
            _contextCollection.InsertOne(context);
        }

        public void Update(Context context)
        {
            var filter = Builders<Context>.Filter.Eq(c => c.Id, context.Id);
            _contextCollection.ReplaceOne(filter, context);
        }

        public Context Get(string contextId)
        {
            var filter = Builders<Context>.Filter.Eq(c => c.Id, contextId);
            return _contextCollection.FindAsync(filter).Result.First();
        }

        public IEnumerable<Context> All()
        {
            return _contextCollection.AsQueryable().ToList();
        }

        public void Clear()
        {
            _contextCollection.DeleteMany(FilterDefinition<Context>.Empty);
        }
    }
}