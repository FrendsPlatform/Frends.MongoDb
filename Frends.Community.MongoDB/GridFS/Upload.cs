using System.ComponentModel;
using System.Text;
using Frends.Community.MongoDB.Helpers;

namespace Frends.Community.MongoDB.GridFS
{
    public class Upload
    {
        public class UploadParameters
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

            /// <summary>
            /// The name of the file to upload to GridFS
            /// </summary>
            [DisplayName("Filename")]
            [DefaultValue("\"fileName\"")]
            public string FileName { get; set; }
        }

        /// <summary>
        /// Uploads a document to MongoDB/GridFS. In this method, the document is provided "inline" as JSON in the Document-field
        /// </summary>
        /// <param name="parameters">The parameters</param>
        /// <returns>The GridFS ID of the </returns>
        public static string UploadToMongoGridFS(UploadParameters parameters)
        {
            var helper = new DatabaseConnectionHelper();

            var bucket = helper.GetGridFSBucket(parameters.DbConnection.ServerAddress,
                                                parameters.DbConnection.ServerPort,
                                                parameters.DbConnection.Database,
                                                parameters.DbConnection.CollectionName,
                                                parameters.DbConnection.UserName,
                                                parameters.DbConnection.Password);

            // Convert the document to bytes so it can be saved to Mongo/GridFS
            byte[] documentAsBytes = Encoding.UTF8.GetBytes(parameters.Document);

            var id = bucket.UploadFromBytes(parameters.FileName, documentAsBytes);
            return id.ToString();
        }
    }
}
