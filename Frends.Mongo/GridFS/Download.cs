using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Frends.Mongo.Helpers;
using MongoDB.Bson;

namespace Frends.Mongo.GridFS
{
    public class Download
    {
        public class DownloadParameters
        {
            /// <summary>
            /// The database connection
            /// </summary>
            [DisplayName("MongoDB Database Connection")]
            public DatabaseConnection DbConnection { get; set; }

            /// <summary>
            /// The ID of the file to download
            /// </summary>
            [DisplayName("GridFS Id")]
            [DefaultValue("")]
            public string Id { get; set; }
        }

        /// <summary>
        /// Downloads a document from MongoDB GridFS based on ID
        /// </summary>
        /// <param name="parameters">the parameters</param>
        /// <returns>The document as a string</returns>
        public static async Task<string> DownloadDocumentFromMongoGridFS(DownloadParameters parameters, CancellationToken cancellationtoken)
        {
            var helper = new DatabaseConnectionHelper();

            var bucket = helper.GetGridFSBucket(parameters.DbConnection.ServerAddress,
                parameters.DbConnection.ServerPort,
                parameters.DbConnection.Database,
                parameters.DbConnection.CollectionName,
                parameters.DbConnection.UserName,
                parameters.DbConnection.Password);

            ObjectId id = new ObjectId(parameters.Id);

            var bytes = await bucket.DownloadAsBytesAsync(id);
            var stringResult = System.Text.Encoding.UTF8.GetString(bytes);

            return stringResult;
        }

    }
}
