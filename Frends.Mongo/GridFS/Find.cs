using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Frends.Mongo.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Frends.Mongo.GridFS
{
    public class Find
    {
        public class FindParameters
        {
            /// <summary>
            /// The database connection
            /// </summary>
            [DisplayName("MongoDB Database Connection")]
            public DatabaseConnection DbConnection { get; set; }

            /// <summary>
            /// The filter with which to search files
            /// </summary>
            [DisplayName("GridFS Id")]
            [DefaultValue("\"{ 'foo':'bar', 'bar': 'foo' }\"")]
            public string Filter { get; set; }
        }

        /// <summary>
        /// Searches documents from MongoDB GridFS
        /// </summary>
        /// <param name="parameters">the parameters</param>
        /// <returns>The metadata of the documents in MongoDB - eg. not the chunks</returns>
        public static async Task<List<string>> FindFromMongoGridFS(FindParameters parameters, CancellationToken cancellationtoken)
        {
            var helper = new DatabaseConnectionHelper();

            var bucket = helper.GetGridFSBucket(parameters.DbConnection.ServerAddress,
                                                parameters.DbConnection.ServerPort,
                                                parameters.DbConnection.Database,
                                                parameters.DbConnection.CollectionName,
                                                parameters.DbConnection.UserName,
                                                parameters.DbConnection.Password);

            var filter = parameters.Filter;
            List<GridFSFileInfo> fileInfo;
            using (var cursor = await bucket.FindAsync(filter))
            {
                fileInfo = cursor.ToList();
            }

            List<string> returnList = new List<string>();
            var jsonSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            

            return fileInfo.Select(fInfo => fInfo.ToBsonDocument().ToJson(jsonSettings)).ToList(); 
            
        }
    }
}
