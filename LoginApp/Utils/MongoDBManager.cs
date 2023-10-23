using LoginApp.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LoginApp.Utils;

public class MongoDBManager
{
    private readonly IMongoCollection<PreUser> _collection;

    public MongoDBManager(IOptions<MongoDBSettings> options)
    {
        MongoClient client = new MongoClient(options.Value.ConnectionURI);

        IMongoDatabase database = client.GetDatabase(options.Value.DatabaseName);

        _collection = database.GetCollection<PreUser>(options.Value.CollectionName);
    }
    
    public async Task CreatePreUser(PreUser request)
    {
        FilterDefinition<PreUser> filter = Builders<PreUser>.Filter.Eq("Email", request.Email);
        
        var value = await _collection.FindAsync<PreUser>(filter);
        
        await _collection.InsertOneAsync(request);
    }

}