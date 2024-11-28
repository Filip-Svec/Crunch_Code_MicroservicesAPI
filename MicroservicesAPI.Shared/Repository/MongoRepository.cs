using MongoDB.Driver;
using System.Linq.Expressions;
using MicroservicesAPI.Shared.Entities;
using MicroservicesAPI.Shared.Repository.Interfaces;
using MongoDB.Bson;

namespace MicroservicesAPI.Shared.Repository;

public class MongoRepository<T>(IMongoCollection<T> dbCollection) : IRepository<T> where T : BaseEntity
{
    protected readonly IMongoCollection<T> _dbCollection = dbCollection;
    protected readonly FilterDefinitionBuilder<T> _filterBuilder = Builders<T>.Filter;

    public async Task<T> GetByIdAsync(string id)
    {
        var objectId = ObjectId.Parse(id); // Convert string to ObjectId
        var filter = _filterBuilder.Eq(item => item.Id, objectId);
        return await _dbCollection.Find(filter).FirstOrDefaultAsync();
    }
}