using Frends.Mongo.Helpers;
using MongoDB.Bson;
using MongoDB.Driver;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Mongo
{
    public class Update
    {
        public class UpdateParameters
        {
            /// <summary>
            /// The database connection
            /// </summary>
            [DisplayName("MongoDB Database Connection")]
            public DatabaseConnection DbConnection { get; set; }

            /// <summary>
            /// The filter to use for the search, in JSON
            /// </summary>
            [DisplayFormat(DataFormatString = "Jsont")]
            [DisplayName("Filter")]
            [DefaultValue("{ 'foo':'bar', 'bar': 'foo' }")]
            public string FilterString { get; set; }

            /// <summary>
            /// The values to update in the document, as JSON
            /// </summary>
            [DisplayFormat(DataFormatString = "Json")]
            [DisplayName("Filter")]
            [DefaultValue("{$set: {bar:'foobar'}}")]
            public string UpdateString { get; set; }
        }

        public class UpdateResult
        {
            public long count;
        }

        /// <summary>
        /// Updates all files in MongoDB that match the search criteria
        /// </summary>
        /// <param name="parameters">The parameters</param>
        /// <returns>The count of updated documents</returns>
        public static UpdateResult UpdateDocuments(UpdateParameters parameters)
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

            UpdateDefinition<BsonDocument> update = parameters.UpdateString;
            var result = new UpdateResult();

            result.count = collection.UpdateMany(filter, update).ModifiedCount;

            return result;
        }
    }
}
