using System.ComponentModel;
using System.IO;
using Frends.Community.MongoDB.Helpers;

namespace Frends.Community.MongoDB.GridFS
{
    public class UploadFile
    {
        public class UploadParameters
        {
            /// <summary>
            /// The database connection
            /// </summary>
            [DisplayName("MongoDB Database Connection")]
            public DatabaseConnection DbConnection { get; set; }

            /// <summary>
            /// The name of the file to upload to GridFS
            /// </summary>
            [DisplayName("Filepath")]
            [DefaultValue("\"path\\to\\file.txt\"")]
            public string FilePath { get; set; }
        }

        /// <summary>
        /// Uploads a document from a file location to MongoDB/GridFS
        /// </summary>
        /// <param name="parameters">The parameters</param>
        /// <returns>The GridFS ID of the uploaded document</returns>
        public static string UploadFileToMongoGridFS(UploadParameters parameters)
        {
            var helper = new DatabaseConnectionHelper();

            var bucket = helper.GetGridFSBucket(parameters.DbConnection.ServerAddress,
                parameters.DbConnection.ServerPort,
                parameters.DbConnection.Database,
                parameters.DbConnection.CollectionName,
                parameters.DbConnection.UserName,
                parameters.DbConnection.Password);

            var fileName = Path.GetFileName(parameters.FilePath);
            string id;

            using (FileStream fs = File.OpenRead(parameters.FilePath))
            {
                id = bucket.UploadFromStream(fileName, fs).ToString();
            }

            return id;
        }
    }
}
