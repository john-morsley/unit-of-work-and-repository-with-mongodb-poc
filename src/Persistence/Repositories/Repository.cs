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

    public virtual async Task<TEntity> GetById(Guid id)
    {
        //var data = await DbSet.FindAsync(Builders<TEntity>.Filter.Eq(" _id ", id));
        //return data.FirstOrDefault();
        throw new NotImplementedException();
    }

    public virtual async Task<IEnumerable<TEntity>> GetAll()
    {
        //var all = await DbSet.FindAsync(Builders<TEntity>.Filter.Empty);
        //return all.ToList();
        throw new NotImplementedException();
    }

    public virtual async Task CreateAsync(TEntity entity)
    {
        try
        {
            //var users = _db.GetCollection<User>(TableName);
            //users.InsertOne(user);
            await _collection.InsertOneAsync(entity);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public virtual Task UpdateAsync(TEntity obj)
    {
        //return _context.AddCommand(async () =>
        //{
        //    await DbSet.ReplaceOneAsync(Builders<TEntity>.Filter.Eq(" _id ", obj.GetId()) obj);
        //});
        throw new NotImplementedException();
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        //_context.AddCommand(() => DbSet.DeleteOneAsync(Builders<TEntity>.Filter.Eq(" _id ", id)));
        //throw new NotImplementedException();
        try
        {
            //var users = _db.GetCollection<User>(TableName);
            //users.InsertOne(user);
            await _collection.DeleteOneAsync(_ => _.Id == id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}