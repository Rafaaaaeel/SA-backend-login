using LoginApp.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace LoginApp.Settings;

public class PreUserService 
{
    private readonly IMongoCollection<PreUser> _tempUserCollection;

    public PreUserService(IOptions<MongoDBSettings> options)
    {
        MongoClient client = new MongoClient(options.Value.ConnectionURI);

        IMongoDatabase database = client.GetDatabase(options.Value.DatabaseName);

        _tempUserCollection = database.GetCollection<PreUser>(options.Value.CollectionName);
    }
}