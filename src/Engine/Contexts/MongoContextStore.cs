using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
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
            var host = ConfigurationManager.AppSettings["MongoDbHost"];
            var databaseName = ConfigurationManager.AppSettings["MongoDbDatabase"];
            var collection = ConfigurationManager.AppSettings["MongoDbContextCollection"];

            var clientSettings = new MongoClientSettings
            {
                Server = new MongoServerAddress(host),
                WaitQueueSize = 10000,
                MaxConnectionPoolSize = 1000,
                ConnectTimeout = new TimeSpan(0, 2, 0),
                SocketTimeout = new TimeSpan(0, 2, 0),
                WaitQueueTimeout = new TimeSpan(0, 2, 0),
                MaxConnectionIdleTime = new TimeSpan(0, 2, 0),
                MaxConnectionLifeTime = new TimeSpan(0, 2, 0),
                ServerSelectionTimeout = new TimeSpan(0, 2, 0)
            };
            var client = new MongoClient(clientSettings);
            var database = client.GetDatabase(databaseName);
            _contextCollection = database.GetCollection<Context>(collection);
        }

        public async Task Insert(Context context)
        {
            await _contextCollection.InsertOneAsync(context);
        }

        public async Task AddExecutedRule(string contextId, Rule rule)
        {
            var filter = Builders<Context>.Filter.Eq(c => c.Id, contextId);
            var push = Builders<Context>.Update.Push(c => c.ExecutedRules, rule);

            await _contextCollection.UpdateOneAsync(filter, push);
        }

        public async Task UpdateFinishedAt(string contextId, DateTime dateTime)
        {
            var filter = Builders<Context>.Filter.Eq(c => c.Id, contextId);
            var set = Builders<Context>.Update.Set(c => c.FinishedAt, dateTime);

            await _contextCollection.UpdateOneAsync(filter, set);
        }

        public async Task<Rule> GetLastRuleExecuted(string contextId)
        {
            return await _contextCollection
                .Find(c => c.Id == contextId)
                .Project(c => c.ExecutedRules.Last())
                .SingleAsync();
        }

        public async Task<IEnumerable<Context>> All()
        {
            return await _contextCollection.AsQueryable().ToListAsync();
        }

        public async Task Clear()
        {
            await _contextCollection.DeleteManyAsync(c => true);
        }
    }
}