using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBCRUD.Models;

public class BooksCollection
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("name")]
    [JsonPropertyName("name")]
    public string BookName { get; set; } = null!;

    [BsonElement("author")]
    [JsonPropertyName("author")]
    public List<string> Authors { get; set; } = new List<string>();
    
    [BsonElement("year")]
    [JsonPropertyName("year")]
    public int YearOfBook { get; set; }
    
    [BsonElement("programming_language")]
    [JsonPropertyName("programming_language")]
    public string ProgrammingLanguages { get; set; } = null!;
    
}
