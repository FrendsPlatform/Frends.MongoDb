using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Frends.Mongo.Helpers
{
    public class DatabaseConnection
    {
        /// <summary>
        /// The address of the MongoDB server
        /// </summary>
        [DisplayName("Server Address")]
        [DisplayFormat(DataFormatString = "Text")]
        public string ServerAddress { get; set; }

        /// <summary>
        /// The port used to connect to the MongoDB server
        /// </summary>
        [DisplayName("Server Port")]
        [DisplayFormat(DataFormatString = "")]
        public string ServerPort { get; set; }

        /// <summary>
        /// The database to connect to
        /// </summary>
        [DisplayName("Database")]
        [DefaultValue("")]
        public string Database { get; set; }

        /// <summary>
        /// The name of the MongoDB collection to perform the operation on
        /// </summary>
        [DisplayName("Collection Name")]
        [DefaultValue("")]
        public string CollectionName { get; set; }

        /// <summary>
        /// The username to use when connecting to Mongo
        /// </summary>
        [DisplayName("Username")]
        public string UserName { get; set; }

        /// <summary>
        /// The password to use when connecting to Mongo
        /// </summary>
        [PasswordPropertyText]
        [DisplayName("Password")]
        public string Password { get; set; }
    }
}
