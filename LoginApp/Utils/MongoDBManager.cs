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
        var user = await GetDataFromQuery(query: "Email", value: (string) request.Email);

        if (user.Count != 0) return new AuthResponse<PreUser>() { Error = true, Message = "Confirm your e-mail", Code = 403 };
        
        await _collection.InsertOneAsync(request);

        return new AuthResponse<PreUser>() { Error = false };
    }

    public async Task<AuthResponse<PreUser>> DeleteUser(string id, int token)
    {
        var data = await _collection.FindAsync(d => d.Id == id);

        if (data == null) return new AuthResponse<PreUser>() { Error = true, Code = 404 };
        
        var user = data.FirstOrDefault();

        if (user.Token != token) return new AuthResponse<PreUser>() { Error = true, Code = 400 };

        var filter = Builders<PreUser>.Filter.Eq(u => u.Id, id);
            
        await _collection.DeleteOneAsync(filter);

        return new AuthResponse<PreUser>() { Data = user };
    }

    private async Task<ICollection<PreUser>> GetDataFromQuery<T>(string query, T value) 
    {
        FilterDefinition<PreUser> filter = Builders<PreUser>.Filter.Eq(query, value);
        
        var user = await _collection.FindAsync<PreUser>(filter);

        return user.ToList();
    }

}