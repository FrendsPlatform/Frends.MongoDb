using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Frends.Mongo.Helpers;
using MongoDB.Bson;

namespace Frends.Mongo.GridFS
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
            /// The ID of the file to download
            /// </summary>
            [DisplayName("GridFS Id")]
            [DefaultValue("")]
            public string Id { get; set; }
        }

        /// <summary>
        /// Deletes one document from mongoDB
        /// </summary>
        /// <param name="parameters">the parameters</param>
        /// <returns>A boolean representing the result of the delete operation: TRUE on success, FALSE on failure</returns>
        public static async Task<bool> DeleteFromMongoGridFS(DeleteParameters parameters)
        {
            var helper = new DatabaseConnectionHelper();

            var bucket = helper.GetGridFSBucket(parameters.DbConnection.ServerAddress,
                parameters.DbConnection.ServerPort,
                parameters.DbConnection.Database,
                parameters.DbConnection.CollectionName,
                parameters.DbConnection.UserName,
                parameters.DbConnection.Password);

            ObjectId id = new ObjectId(parameters.Id);

            try
            {
                await bucket.DeleteAsync(id);
            }
            catch (Exception e)
            {
                if (e.Message == "GridFS file not found: file id " + parameters.Id + ".") return false; // Id not found - return false

                throw; // Some other error - throw it
            }

            return true;
        }
    }
}
