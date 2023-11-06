using MongoDBCRUD.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MongoDBCRUD.Services;

public class BooksService
{
    private readonly IMongoCollection<BooksCollection> _booksCollection;

    public BooksService(
        IOptions<BooksDatabaseSettings> booksDatabaseSettings)

    {
        var mongoClient = new MongoClient(
            booksDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            booksDatabaseSettings.Value.DatabaseName);

        _booksCollection = mongoDatabase.GetCollection<BooksCollection>(
            booksDatabaseSettings.Value.BooksCollectionName);
    }

    public async Task<List<BooksCollection>> GetBooks() =>
        await _booksCollection.Find(_ => true).ToListAsync();

    public async Task<BooksCollection?> GetBookById(string id) =>
        await _booksCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateBook(BooksCollection newBook) =>
        await _booksCollection.InsertOneAsync(newBook);

    public async Task UpdateBook(string id, BooksCollection updatedBook) =>
        await _booksCollection.ReplaceOneAsync(x => x.Id == id, updatedBook);

    public async Task RemoveBook(string id) =>
        await _booksCollection.DeleteOneAsync(x => x.Id == id);
    
}
