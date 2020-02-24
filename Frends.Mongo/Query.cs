using Frends.Mongo.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Frends.Mongo
{
    public class Query
    {
        public class QueryParameters
        {
            /// <summary>
            /// The database connection
            /// </summary>
            [DisplayName("CassandraDB Database Connection")]
            public DatabaseConnection DbConnection { get; set; }

            /// <summary>
            /// The filter to use for the search, in JSON
            /// </summary>
            [DisplayName("Filter")]
            [DefaultValue("{ 'foo':'bar', 'bar': 'foo' }")]
            public string FilterString { get; set; }
        }

        /// <summary>
        /// Searches for documents in MongoDB
        /// </summary>
        /// <param name="parameters">The parameters</param>
        /// <returns>A list with the documents matching the search criteria</returns>
        public static JArray QueryDocuments(QueryParameters parameters)
        {
            var helper = new DatabaseConnectionHelper();
            var collection = helper.GetMongoCollection(parameters.DbConnection.ServerAddress,
                                                parameters.DbConnection.ServerPort,
                                                parameters.DbConnection.Database,
                                                parameters.DbConnection.CollectionName,
                                                parameters.DbConnection.UserName,
                                                parameters.DbConnection.Password);

            // Initialize the filter
            var filter = parameters.FilterString;
            var cursor = collection.Find(filter).ToCursor();

            JArray documentList = new JArray();

            var jsonSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            var list = cursor.ToEnumerable().Select(document => document.ToJson(jsonSettings)).ToList();

            return JArray.FromObject(list);
        }
    }
}
