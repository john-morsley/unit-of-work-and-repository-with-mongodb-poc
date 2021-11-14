//namespace UnitOfWork;

//public class UnitOfWork : IUnitOfWork
//{
//    private readonly IMongoContext _context;

//    public UnitOfWork(IMongoContext context)
//    {
//        _context = context;
//    }

//    public async Task<bool> Commit()
//    {
//        var numberAffected = await _context.SaveChanges();

//        return numberAffected > 0;
//    }

//    public void Dispose()
//    {
//        _context.Dispose();
//    }
//}