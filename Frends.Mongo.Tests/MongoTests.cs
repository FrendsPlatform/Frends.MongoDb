using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Frends.Mongo.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using NUnit.Framework.Internal;

namespace Frends.Mongo.Tests
{
    [TestClass]
    //[Ignore("The tests in this file require a live MongoDB instance")]
    public class MongoTests
    {

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
        static readonly DatabaseConnection DatabaseConnectionInstance = new DatabaseConnection
        {
            ServerAddress = "localhost",
            ServerPort = "27017",
            Database = "mongo_tests",
            CollectionName = "test_collection",
            UserName = "",
            Password = ""
        };

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

            JArray documents = Query.QueryDocuments(upParams);

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

            Assert.IsTrue(result.count> 0);
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

            Assert.IsTrue(result.count > 0);
        }

        [TestMethod]
        public void TestUploadDocumentToGridFs()
        {
            GridFS.Upload.UploadParameters parameters = new GridFS.Upload.UploadParameters
            {
                DbConnection = DatabaseConnectionInstance,
                Document = "{ 'foo':'barf', 'bar': 'foo' }",
                FileName = "Test" + DateTime.Now
            };

            CancellationToken token = new CancellationToken();
            var id = GridFS.Upload.UploadToMongoGridFS(parameters, token);

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
            CancellationToken token = new CancellationToken();
            var id = GridFS.UploadFile.UploadFileToMongoGridFS(parameters, token);

            Assert.AreNotEqual("", id);
        }

        [TestMethod]
        public void TestFileDownloadFromGridFs()
        {
            CancellationToken token = new CancellationToken();
            var fileName = Path.GetFileName(Path.GetTempFileName());
            var fileId = GridFS.Upload.UploadToMongoGridFS(
                new GridFS.Upload.UploadParameters { DbConnection = DatabaseConnectionInstance, Document = "{ \"a\": 123 }", FileName = fileName }, token);
            GridFS.Download.DownloadParameters parameters = new GridFS.Download.DownloadParameters
            {
                DbConnection = DatabaseConnectionInstance,
                Id = fileId.ToString()
            };

            var result = GridFS.Download.DownloadDocumentFromMongoGridFS(parameters, token);

            Assert.AreNotEqual("", result);
        }

        [TestMethod]
        public void TestFindFromGridFs()
        {
            var fileName = Path.GetFileName(Path.GetTempFileName());
            CancellationToken token = new CancellationToken();
            var fileId = GridFS.Upload.UploadToMongoGridFS(
                new GridFS.Upload.UploadParameters { DbConnection = DatabaseConnectionInstance, Document = "{ \"a\": 123 }", FileName = fileName }, token);
            GridFS.Find.FindParameters parameters = new GridFS.Find.FindParameters
            {
                DbConnection = DatabaseConnectionInstance,
                Filter = "{ 'filename': '" + fileName + "' }"
            };

            var r = GridFS.Find.FindFromMongoGridFS(parameters, token);

            Assert.AreNotEqual(0, r.Result.Count);
        }

        [TestMethod]
        public void TestDeleteFromGridFs()
        {
            var fileName = Path.GetFileName(Path.GetTempFileName());
            CancellationToken token = new CancellationToken();
            var fileId = GridFS.Upload.UploadToMongoGridFS(
                new GridFS.Upload.UploadParameters { DbConnection = DatabaseConnectionInstance, Document = "{ \"a\": 123 }", FileName = fileName }, token);
            GridFS.Delete.DeleteParameters parameters = new GridFS.Delete.DeleteParameters
            {
                DbConnection = DatabaseConnectionInstance,
                Id = fileId.ToString()
            };

            var result = GridFS.Delete.DeleteFromMongoGridFS(parameters);

            Assert.AreEqual(true, result);
        }
    }
}