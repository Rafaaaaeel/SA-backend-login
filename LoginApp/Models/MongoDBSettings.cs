namespace LoginApp.Models;

public class MongoDBSettings 
{
    public required string ConnectionURI { get; set; }
    public required string DatabaseName { get; set; }
    public required string CollectionName { get; set; }
}