using System.Linq.Expressions;
using MongoDBCRUD.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoDBCRUD.Services;

public class AuthorService
{
    private readonly IMongoCollection<AuthorCollection> _authorCollection;
    private readonly IMongoCollection<BooksCollection> _booksCollection;

    public AuthorService(
        IOptions<AuthorDatabaseSettings> authorDatabaseSettings)

    {
        var mongoClient = new MongoClient(
            authorDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            authorDatabaseSettings.Value.DatabaseName);
        
        _authorCollection = mongoDatabase.GetCollection<AuthorCollection>(
            authorDatabaseSettings.Value.AuthorsCollectionName);
        
        _booksCollection = mongoDatabase.GetCollection<BooksCollection>(
            authorDatabaseSettings.Value.BooksCollectionName);

    }

    public async Task<List<AuthorCollection>> GetAsync() =>
        await _authorCollection.Find(_ => true).ToListAsync();

    public async Task<AuthorCollection?> GetAsync(string id) =>
        await _authorCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(AuthorCollection newAuthor) =>
        await _authorCollection.InsertOneAsync(newAuthor);

    public async Task UpdateAsync(string id, AuthorCollection updateAuthor) =>
        await _authorCollection.ReplaceOneAsync(x => x.Id == id, updateAuthor);

    public async Task RemoveAsync(string id) =>
        await _authorCollection.DeleteOneAsync(x => x.Id == id);
    
    public async Task<bool> DeleteAuthorCascadeAsync(string authorName)
    {
        var authorFilter = Builders<AuthorCollection>.Filter.Eq(a => a.AuthorOfBook, authorName);
        var deleteResult = await _authorCollection.DeleteOneAsync(authorFilter);

        if (deleteResult.DeletedCount > 0)
        {
            var bookFilter = Builders<BooksCollection>.Filter.AnyEq(b => b.Authors, authorName);
            var update = Builders<BooksCollection>.Update.Pull(b => b.Authors, authorName);
            var updateResult = await _booksCollection.UpdateManyAsync(bookFilter, update);

            return updateResult.ModifiedCount > 0;
        }

        return false;
    }


    
    
}
