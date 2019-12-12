using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Frends.Community.MongoDB.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;

namespace Frends.Community.MongoDB.GridFS
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
        public static List<string> FindFromMongoGridFS(FindParameters parameters)
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
            using (var cursor = bucket.Find(filter))
            {
                fileInfo = cursor.ToList();
            }

            List<string> returnList = new List<string>();
            var jsonSettings = new JsonWriterSettings { OutputMode = JsonOutputMode.Strict };

            foreach (var fInfo in fileInfo)
            {
                returnList.Add(fInfo.ToBsonDocument().ToJson(jsonSettings));
            }

            return returnList;
        }
    }
}
