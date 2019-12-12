using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;

namespace Frends.Community.MongoDB.Helpers
{
    public class DatabaseConnectionHelper
    {
        /// <summary>
        /// Creates a connection to the MongoDB database
        /// </summary>
        /// <param name="serverAddress">Server address</param>
        /// <param name="serverPort">Server port</param>
        /// <param name="database">Database name</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>A IMongoDatabase instance with the database connection</returns>
        public IMongoDatabase GetMongoDatabase(string serverAddress, string serverPort, string database, string username, string password)
        {
            // Establish the connection:
            var credentials = MongoCredential.CreateMongoCRCredential(database, username, password);
            
            var clientSettings = new MongoClientSettings
            {
                Credential = string.IsNullOrEmpty(username) ? null : credentials,
                Server = new MongoServerAddress(serverAddress, Convert.ToInt32(serverPort))
            };

            MongoClient mongoClient = new MongoClient(clientSettings);
            var dataBase = mongoClient.GetDatabase(database);
            return dataBase;
        }

        /// <summary>
        /// Creates a connection to the MongoDB database and returns a connection to a MongoDB collection
        /// </summary>
        /// <param name="serverAddress">server name</param>
        /// <param name="serverPort">server port</param>
        /// <param name="database">database name</param>
        /// <param name="collectionName">collection name</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>A IMongoCollection instance with the connection to the collection</returns>
        public IMongoCollection<BsonDocument> GetMongoCollection(string serverAddress, string serverPort, string database, string collectionName, string username, string password)
        {
            var dataBase = GetMongoDatabase(serverAddress, serverPort, database, username, password);
            var collection = dataBase.GetCollection<BsonDocument>(collectionName);

            return collection;
        }

        /// <summary>
        /// Returns a GridFS bucket with an open connection to Mongo
        /// </summary>
        /// <param name="serverAddress">server address</param>
        /// <param name="serverPort">server port</param>
        /// <param name="database">database name</param>
        /// <param name="bucketName">the name of the bucket (collection) to operate on</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>a GridFSBucket with an active connection</returns>
        public GridFSBucket GetGridFSBucket(string serverAddress, string serverPort, string database, string bucketName, string username, string password)
        {
            var mongoDatabase = GetMongoDatabase(serverAddress, serverPort, database, username, password);

            var bucket = new GridFSBucket(mongoDatabase, new GridFSBucketOptions
            {
                BucketName = bucketName,
                ChunkSizeBytes = 15 * 1024 * 1024, // Set one chunk to 15 MB
                WriteConcern = WriteConcern.WMajority,
                ReadPreference = ReadPreference.Secondary
            });

            return bucket;
        }
    }
}
