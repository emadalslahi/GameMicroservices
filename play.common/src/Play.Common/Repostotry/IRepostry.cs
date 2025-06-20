using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Play.Common.Entities;


namespace Play.Common.Repositories;

public interface IRepostry<T> where T : IEntity
{
    Task<IReadOnlyCollection<T>> GetAllAsync();
    Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter);
    //Task<IReadOnlyCollection<T>> GetAsync(Expression<Func<T, bool>> filter, int page, int pageSize);
    //Task<IReadOnlyCollection<T>> GetAsync(int page, int pageSize);
    //Task<IReadOnlyCollection<T>> GetAsync(Expression<Func<T, object>> orderBy, bool ascending = true);

    Task<T> GetAsync(Guid id);
    Task<T> GetAsync(Expression<Func<T, bool>> filter);


    Task<T> CreateAsync(T entity);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);


    // Task<IReadOnlyCollection<T>> GetByNameAsync(string name);
    // Task<IReadOnlyCollection<T>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    Task<bool> UpdateAsync(T entity);


}
