using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;
using Play.Common.Entities;
using Play.Common.Repositories;

namespace Play.Common.MongoDB;

public class MongoReposotry<T> : IRepostry<T> where T : IEntity
{
    private readonly IMongoCollection<T> _collection;
    private readonly FilterDefinitionBuilder<T> _filterBuilder = Builders<T>.Filter;
    public MongoReposotry(IMongoDatabase database, string collectionName)
    {
        _collection = database.GetCollection<T>(collectionName);
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync()
    {
        return await _collection.Find(_filterBuilder.Empty).ToListAsync();
    }

    public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter)
    {
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<T> GetAsync(Guid id)
    {
        var filter = _filterBuilder.Eq("Id", id);
        return await _collection.Find(filter).SingleOrDefaultAsync();
    }
    public async Task<T> GetAsync(Expression<Func<T, bool>> filter)
    {
        return await _collection.Find(filter).SingleOrDefaultAsync();
    }

    public async Task<T> CreateAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }
    public async Task<bool> UpdateAsync(T entity)
    {
        var filter = _filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
        var result = await _collection.ReplaceOneAsync(filter, entity);
        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var filter = _filterBuilder.Eq("Id", id);
        var result = await _collection.DeleteOneAsync(filter);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }
    public async Task<bool> ExistsAsync(Guid id)
    {
        var filter = _filterBuilder.Eq("Id", id);
        return await _collection.Find(filter).AnyAsync();
    }




    /*
public async Task<IReadOnlyCollection<T>> GetByNameAsync(string name)
{
   var filter = _filterBuilder.Eq(item => item.Name, name);
   return await _collection.Find(filter).ToListAsync();
}
public async Task<IReadOnlyCollection<Item>> GetItemsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
{
   var filter = _filterBuilder.And(
       _filterBuilder.Gte(item => item.Price, minPrice),
       _filterBuilder.Lte(item => item.Price, maxPrice)
   );
   return await _collection.Find(filter).ToListAsync();
}
*/
}