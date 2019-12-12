using System;
using System.Collections.Generic;
using System.IO;
using Frends.Community.MongoDB.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Frends.Community.MongoDB.Tests
{
    [TestClass]
    //[Ignore("The tests in this file require a live MongoDB instance")]
    public class MongoDbTests
    {

        #region TestVariables
        public string JSON = @"{
  'Stores': [
    'Lambton Quay',
    'Willis Street'
  ],
  'Manufacturers': [
    {
      'Name': 'Acme Co',
      'Products': [
        {
          'Name': 'Anvil',
          'Price': 50
        }
      ]
    },
    {
      'Name': 'Contoso',
      'Products': [
        {
          'Name': 'Elbow Grease',
          'Price': 99.95
        },
        {
          'Name': 'Headlight Fluid',
          'Price': 4
        }
      ]
    }
  ]
}";
        #endregion

        static readonly DatabaseConnection DatabaseConnectionInstance = new DatabaseConnection
        {
            ServerAddress = "localhost",
            ServerPort = "27017",
            Database = "mongo_tests",
            CollectionName = "test_collection",
            UserName = "",
            Password = ""
        };

    #region Mongo
    [TestMethod]
    public void TestInsertDocumentToMongoDb()
        {
            Insert.InsertParameters parameters = new Insert.InsertParameters
            {
                DbConnection = DatabaseConnectionInstance,
                Document = "{ 'foo':'barf', 'bar': 'foo' }"
            };

            string s = Insert.InsertDocument(parameters);

            Assert.AreNotEqual("", s);
        }

        [TestMethod]
        public void TestQueryDocumentsFromMongoDb()
        {
            Query.QueryParameters upParams = new Query.QueryParameters
            {
                DbConnection = DatabaseConnectionInstance,
                FilterString = "{'bar':'foo'}"
            };

            List<string> documents = Query.QueryDocuments(upParams);

            Assert.IsTrue(documents.Count > 0);
        }

        [TestMethod]
        public void TestUpdateDocumentsInMongoDb()
        {
            Update.UpdateParameters parameters = new Update.UpdateParameters
            {
                DbConnection = DatabaseConnectionInstance,
                FilterString = "{'foo':'barf'}",
                UpdateString = "{$set: {bar:'foobar'}}"
            };

            var result = Update.UpdateDocuments(parameters);

            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public void TestDeleteDocumentsInMongoDb()
        {
            Delete.DeleteParameters parameters = new Delete.DeleteParameters
            {
                DbConnection = DatabaseConnectionInstance,
                FilterString = "{'bar':'foobar'}"
            };

            var result = Delete.DeleteDocuments(parameters);

            Assert.IsTrue(result > 0);
        }
        #endregion Mongo

        #region GridFS
        [TestMethod]
        public void TestUploadDocumentToGridFs()
        {
            GridFS.Upload.UploadParameters parameters = new GridFS.Upload.UploadParameters
            {
                DbConnection = DatabaseConnectionInstance,
                Document = "{ 'foo':'barf', 'bar': 'foo' }",
                FileName = "Test" + DateTime.Now
            };

            var id = GridFS.Upload.UploadToMongoGridFS(parameters);

            Assert.AreNotEqual("", id);
        }

        [TestMethod]
        public void TestFileUploadToGridFs()
        {
            var tmpFilePath = Path.GetTempFileName();
            File.WriteAllText("test", tmpFilePath);

            GridFS.UploadFile.UploadParameters parameters = new GridFS.UploadFile.UploadParameters
            {
                DbConnection = DatabaseConnectionInstance,
                FilePath = tmpFilePath
            };

            var id = GridFS.UploadFile.UploadFileToMongoGridFS(parameters);

            Assert.AreNotEqual("", id);
        }

        [TestMethod]
        public void TestFileDownloadFromGridFs()
        {
            var fileName = Path.GetFileName(Path.GetTempFileName());
            var fileId = GridFS.Upload.UploadToMongoGridFS(
                new GridFS.Upload.UploadParameters { DbConnection = DatabaseConnectionInstance, Document = "{ \"a\": 123 }", FileName = fileName });
            GridFS.Download.DownloadParameters parameters = new GridFS.Download.DownloadParameters
            {
                DbConnection = DatabaseConnectionInstance,
                Id = fileId
            };

            var result = GridFS.Download.DownloadFromMongoGridFS(parameters);

            Assert.AreNotEqual("", result);
        }

        [TestMethod]
        public void TestFindFromGridFs()
        {
            var fileName = Path.GetFileName(Path.GetTempFileName());
            var fileId = GridFS.Upload.UploadToMongoGridFS(
                new GridFS.Upload.UploadParameters { DbConnection = DatabaseConnectionInstance, Document = "{ \"a\": 123 }", FileName = fileName });
            GridFS.Find.FindParameters parameters = new GridFS.Find.FindParameters
            {
                DbConnection = DatabaseConnectionInstance,
                Filter = "{ 'filename': '" + fileName + "' }"
            };

            var r = GridFS.Find.FindFromMongoGridFS(parameters);

            Assert.AreNotEqual(0, r.Count);
        }

        [TestMethod]
        public void TestDeleteFromGridFs()
        {
            var fileName = Path.GetFileName(Path.GetTempFileName());
            var fileId = GridFS.Upload.UploadToMongoGridFS(
                new GridFS.Upload.UploadParameters { DbConnection = DatabaseConnectionInstance, Document = "{ \"a\": 123 }", FileName = fileName });
            GridFS.Delete.DeleteParameters parameters = new GridFS.Delete.DeleteParameters
            {
                DbConnection = DatabaseConnectionInstance,
                Id = fileId
            };

            var result = GridFS.Delete.DeleteFromMongoGridFS(parameters);

            Assert.AreEqual(true, result);
        }
        #endregion GridFS
    }
}
