using Frends.Community.MongoDB.Helpers;
using MongoDB.Bson;
using System.ComponentModel;

namespace Frends.Community.MongoDB
{
    public class Insert
    {
        public class InsertParameters
        {
            /// <summary>
            /// The database connection
            /// </summary>
            [DisplayName("MongoDB Database Connection")]
            public DatabaseConnection DbConnection { get; set; }

            /// <summary>
            /// The document to insert to the collection, as a JSON string
            /// </summary>
            [DisplayName("Document")]
            [DefaultValue("{ 'foo':'bar', 'bar': 'foo' }")]
            public string Document { get; set; }
        }

        /// <summary>
        /// Inserts a document to MongoDB
        /// </summary>
        /// <param name="parameters">The parameters</param>
        /// <returns>The MongoDB ID of the added document</returns>
        public static string InsertDocument(InsertParameters parameters)
        {
            var helper = new DatabaseConnectionHelper();
            var collection = helper.GetMongoCollection(parameters.DbConnection.ServerAddress,
                                                parameters.DbConnection.ServerPort,
                                                parameters.DbConnection.Database,
                                                parameters.DbConnection.CollectionName,
                                                parameters.DbConnection.UserName,
                                                parameters.DbConnection.Password);

            // Insert document
            var bsonDocument = BsonDocument.Parse(parameters.Document);
            collection.InsertOne(bsonDocument);
            return bsonDocument["_id"].ToString();
        }
    }
}
