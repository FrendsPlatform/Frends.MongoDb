using Frends.Community.MongoDB.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System.Collections.Generic;
using System.ComponentModel;

namespace Frends.Community.MongoDB
{
    public class Query
    {
        public class QueryParameters
        {
            /// <summary>
            /// The database connection
            /// </summary>
            [DisplayName("MongoDB Database Connection")]
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
        public static List<string> QueryDocuments(QueryParameters parameters)
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

            List<string> documentList = new List<string>();
            var jsonSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            foreach (var document in cursor.ToEnumerable())
            {
                documentList.Add(document.ToJson(jsonSettings));
            }

            return documentList;
        }
    }
}
