using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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

        public void AddExecutedRule(string contextId, Rule rule)
        {
            var filter = Builders<Context>.Filter.Eq(c => c.Id, contextId);
            var push = Builders<Context>.Update.Push(c => c.ExecutedRules, rule);

            _contextCollection.UpdateOne(filter, push);
        }

        public void UpdateFinishedAt(string contextId, DateTime dateTime)
        {
            var filter = Builders<Context>.Filter.Eq(c => c.Id, contextId);
            var set = Builders<Context>.Update.Set(c => c.FinishedAt, dateTime);

            _contextCollection.UpdateOne(filter, set);
        }

        public Context Get(string contextId)
        {
            return _contextCollection.Find(c => c.Id == contextId).Single();
        }

        public Rule GetLastRuleExecuted(string contextId)
        {
            return _contextCollection
                .Find(c => c.Id == contextId)
                .Project(c => c.ExecutedRules.Last())
                .Single();
        }

        public IEnumerable<Context> All()
        {
            return IAsyncCursorSourceExtensions.ToList(_contextCollection.AsQueryable());
        }

        public void Clear()
        {
            _contextCollection.DeleteMany(FilterDefinition<Context>.Empty);
        }
    }
}