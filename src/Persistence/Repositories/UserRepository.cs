namespace Persistence.Repositories;

public class UserRepository : Repository<Domain.Models.User>, IUserRepository
{
    public UserRepository(IMongoContext context) : base (context, "users") {}    
}