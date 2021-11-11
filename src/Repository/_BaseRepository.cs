namespace Repository;

public abstract class _BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    //protected readonly IMongoContext _context;
    //protected readonly IMongoCollection<TEntity> DbSet;

    //protected BaseRepository(IMongoContext context)
    //{
    //    _context = context;
    //    DbSet = _context.GetCollection<TEntity>(typeof(TEntity).Name);
    //}

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

    public virtual Task Add(TEntity obj)
    {
        //return _context.AddCommand(async () => await DbSet.InsertOneAsync(obj));
        throw new NotImplementedException();
    }

    public virtual Task Update(TEntity obj)
    {
        //return _context.AddCommand(async () =>
        //{
        //    await DbSet.ReplaceOneAsync(Builders<TEntity>.Filter.Eq(" _id ", obj.GetId()) obj);
        //});
        throw new NotImplementedException();
    }

    public virtual Task Remove(Guid id)
    {
        //_context.AddCommand(() => DbSet.DeleteOneAsync(Builders<TEntity>.Filter.Eq(" _id ", id)));
        throw new NotImplementedException();
    }
    

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}