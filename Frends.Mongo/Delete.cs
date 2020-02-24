using Frends.Mongo.Helpers;
using System.ComponentModel;

namespace Frends.Mongo
{
    public class Delete
    {
        public class DeleteParameters
        {
            /// <summary>
            /// The database connection
            /// </summary>
            [DisplayName("MongoDB Database Connection")]
            public DatabaseConnection DbConnection { get; set; }

            /// <summary>
            /// The filter to match documents to, in JSON
            /// </summary>
            [DisplayName("Filter")]
            [DefaultValue("{ 'foo':'bar', 'bar': 'foo' }")]
            public string FilterString { get; set; }
        }

        public class DeleteResult
        {
            public long count;
        }

        /// <summary>
        /// Deletes documents that match the filtering criteria
        /// </summary>
        /// <param name="parameters">The parameters</param>
        /// <returns>A long value with the amount of deleted documents</returns>
        public static DeleteResult DeleteDocuments(DeleteParameters parameters)
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
            var result = new DeleteResult();
            result.count = collection.DeleteMany(filter).DeletedCount;

            return result;
        }
    }
}
