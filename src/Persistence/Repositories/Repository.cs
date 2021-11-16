

namespace Persistence.Repositories;

public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity<Guid>
{
    protected readonly IMongoContext _context;
    protected readonly IMongoCollection<TEntity> _collection;

    protected Repository(IMongoContext context, string tableName)
    {
        _context = context;
        _collection = _context.GetCollection<TEntity>(tableName);
    }

    public virtual async Task<TEntity> GetByIdAsync(Guid id)
    {
        try
        {
            return _collection.Find<TEntity>(user => user.Id == id).SingleOrDefault();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public virtual async Task<IPagedList<TEntity>> GetPageAsync(IGetOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        var entities = GetAll(options);

        return PagedList<TEntity>.Create(entities, options.PageNumber, options.PageSize);
    }

    public virtual async Task CreateAsync(TEntity entity)
    {
        try
        {
            await _collection.InsertOneAsync(entity);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public virtual async Task UpdateAsync(TEntity update)
    {
        try
        {
            await _collection.ReplaceOneAsync(_ => _.Id == update.Id, update);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        try
        {
            await _collection.DeleteOneAsync(_ => _.Id == id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    protected virtual IQueryable<TEntity> GetAll(IGetOptions options)
    {
        var entities = _collection.AsQueryable();

        //entities = (MongoDB.Driver.Linq.IMongoQueryable<TEntity>)Sort(entities, options);
        //entities = Filter(entities, options);
        //entities = Search(entities, options);

        return entities;
    }

    protected virtual IQueryable<TEntity> Filter(IQueryable<TEntity> entities, IGetOptions options)
    {
        return entities;
    }

    protected virtual IQueryable<TEntity> Search(IQueryable<TEntity> entities, IGetOptions options)
    {
        return entities;
    }

    protected virtual IQueryable<TEntity> Sort(IQueryable<TEntity> entities, IGetOptions options)
    {
        if (!options.Orderings.Any()) return entities;

        return entities.OrderBy(ToOrderByString(options.Orderings));
    }

    private string ToOrderByString(IEnumerable<IOrdering> orderings)
    {
        var orderBys = new List<string>();

        foreach (var ordering in orderings)
        {
            var orderBy = ordering.Key;
            switch (ordering.Order)
            {
                case SortOrder.Ascending:
                    orderBy += " asc";
                    break;
                case SortOrder.Descending:
                    orderBy += " desc";
                    break;
            }
            orderBys.Add(orderBy);
        }

        return string.Join(",", orderBys);
    }


    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}