//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace Domain.Interfaces;

public interface IRepository<TEntity> : IDisposable where TEntity : class
{
    Task<TEntity> GetById(Guid id);

    Task<IEnumerable<TEntity>> GetAll();

    Task CreateAsync(TEntity obj);

    Task UpdateAsync(TEntity obj);

    Task DeleteAsync(Guid id);
}