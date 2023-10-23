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
    
    public async Task<AuthResponse<PreUser>> CreatePreUser(PreUser request)
    {
        var user = await GetDataFromQuery(query: "Email", value: request.Email);

        if (user.Count != 0) return new AuthResponse<PreUser>() { Error = true, Message = "Confirm your e-mail", Code = 403 };
        
        await _collection.InsertOneAsync(request);

        return new AuthResponse<PreUser>() { Error = false };
    }

    private async Task<ICollection<PreUser>> GetDataFromQuery(string query, string value) 
    {
        FilterDefinition<PreUser> filter = Builders<PreUser>.Filter.Eq(query, value);
        
        var user = await _collection.FindAsync<PreUser>(filter);

        return user.ToList();
    }

}