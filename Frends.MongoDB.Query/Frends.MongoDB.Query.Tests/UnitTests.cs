using Microsoft.VisualStudio.TestTools.UnitTesting;
using Frends.MongoDB.Query.Definitions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Frends.MongoDB.Query.Tests;

[TestClass]
public class UnitTests
{
    /* 
        Run command 'docker-compose up -d' in \Frends.MongoDB.Query.Tests\Files\
    */

    private static readonly Connection _connection = new()
    {
		ConnectionString = "mongodb://admin:Salakala@localhost:27017/?authSource=admin",
		Database = "testdb",
        CollectionName = "testcoll",
    };

    private readonly string _doc1 = "{ \"foo\":\"bar\", \"bar\": \"foo\" }";
    private readonly string _doc2 = "{ \"foo\":\"bar\", \"bar\": \"foo\" }";
    private readonly string _doc3 = "{ \"qwe\":\"rty\", \"asd\": \"fgh\" }";


    [TestInitialize]
    public void StartUp()
    {
        InsertTestData();
    }

    [TestCleanup]
    public void CleanUp()
    {
        DeleteTestData();
    }

    [TestMethod]
    public async Task Test_Query_TwoResults()
    {
        var _input = new Input()
        {
            Filter = "{'foo':'bar'}"
        };

        var result = await MongoDB.Query(_input, _connection, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(2, result.Data.Count);
    }

    [TestMethod]
    public async Task Test_Query_OneResults()
    {
        var _input = new Input()
        {
            Filter = "{'qwe':'rty'}"
        };

        var result =await  MongoDB.Query(_input, _connection, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(1, result.Data.Count);
    }

	[TestMethod]
	public async Task Test_Query_QueryOne_Options()
	{
		var _input = new Input()
		{
			Filter = "{'foo':'bar'}",
			QueryOptions = QueryOptions.QueryOne
		};

		var result = await MongoDB.Query(_input, _connection, default);
		Assert.IsTrue(result.Success);
		Assert.AreEqual(1, result.Data.Count);
	}

	[TestMethod]
	public async Task Test_Query_QuerMany_Options()
	{
		var _input = new Input()
		{
			Filter = "{'foo':'bar'}",
			QueryOptions = QueryOptions.QueryMany
		};

		var result = await MongoDB.Query(_input, _connection, default);
		Assert.IsTrue(result.Success);
		Assert.AreEqual(2, result.Data.Count);
	}

	[TestMethod]
    public async Task Test_Query_NotFoundFilter()
    {
        var _input = new Input()
        {
            Filter = "{'not':'found'}",
        };

        var result = await MongoDB.Query(_input, _connection, default);
        Assert.IsTrue(result.Success);
        Assert.AreEqual(0, result.Data.Count);
    }

    [TestMethod]
    public async Task Test_EmptyQuery()
    {
		var ex = await Assert.ThrowsExceptionAsync<Exception>(async () => await MongoDB.Query(new Input { Filter = "" }, _connection, default));
        Assert.AreEqual("Query error: Filter can't be null.", ex.Message);
    }

    [TestMethod]
    public async Task Test_InvalidConnectionString()
    {
        var input = new Input()
        {
            Filter = "{'not':'found'}",
        };

        var connection = new Connection
        {
            ConnectionString = "mongodb://admin:Salakala@192.168.10.113:27017/?authSource=?authSource=invalid",
            CollectionName = _connection.CollectionName,
            Database = _connection.Database,
        };

        var ex = await Assert.ThrowsExceptionAsync<Exception>(async () => await MongoDB.Query(input, connection, default));
        Assert.IsTrue(ex.Message.StartsWith("Query error: MongoDB.Driver.MongoAuthenticationException: Unable to authenticate using sasl protocol mechanism SCRAM-SHA-1."));
    }

    private void InsertTestData()
    {
        try
        {
            var collection = GetMongoCollection(_connection.ConnectionString, _connection.Database, _connection.CollectionName);

            var doc1 = BsonDocument.Parse(_doc1);
            var doc2 = BsonDocument.Parse(_doc2);
            var doc3 = BsonDocument.Parse(_doc3);

            collection.InsertOne(doc1);
            collection.InsertOne(doc2);
            collection.InsertOne(doc3);
        }
        catch (Exception ex)
        {

            throw new Exception(ex.Message);
        }
    }

    private static IMongoCollection<BsonDocument> GetMongoCollection(string connectionString, string database, string collectionName)
    {
        var dataBase = GetMongoDatabase(connectionString, database);
        var collection = dataBase.GetCollection<BsonDocument>(collectionName);
        return collection;
    }

    private static IMongoDatabase GetMongoDatabase(string connectionString, string database)
    {
        var mongoClient = new MongoClient(connectionString);
        var dataBase = mongoClient.GetDatabase(database);
        return dataBase;
    }

    private static void DeleteTestData()
    {
        var collection = GetMongoCollection(_connection.ConnectionString, _connection.Database, _connection.CollectionName);

        var filter1 = "{'bar':'foo'}";
        var filter2 = "{'qwe':'rty'}";
        var filter3 = "{'asd':'fgh'}";
        collection.DeleteMany(filter1);
        collection.DeleteMany(filter2);
        collection.DeleteMany(filter3);
    }
}