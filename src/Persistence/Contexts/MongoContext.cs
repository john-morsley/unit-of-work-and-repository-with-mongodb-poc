
namespace Persistence.Contexts;

public class MongoContext : IMongoContext
{
    private readonly IConfiguration _configuration;
    private readonly List<Func<Task>> _commands;

    private IMongoDatabase Database { get; set; }

    public MongoClient MongoClient { get; set; }

    public IClientSessionHandle Session { get; set; }

    public MongoContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _commands = new List<Func<Task>>();
    }

    public void AddCommand(Func<Task> func)
    {
        _commands.Add(func);
    }

    public IMongoCollection<T> GetCollection<T>(string name)
    {
        ConfigureMongo();

        return Database.GetCollection<T>(name);
    }

    //public async Task<int> SaveChanges()
    //{
    //    ConfigureMongo();

    //    using (Session = await MongoClient.StartSessionAsync())
    //    {
    //        Session.StartTransaction();

    //        var commandTasks = _commands.Select(c => c());

    //        await Task.WhenAll(commandTasks);

    //        await Session.CommitTransactionAsync();
    //    }

    //    return _commands.Count;
    //}

    public void Dispose()
    {
        Session?.Dispose();
        GC.SuppressFinalize(this);
    }

    private void ConfigureMongo()
    {
        if (MongoClient != null) return;

        //var host = _configuration["MongoSettings:Host"];
        //var port = _configuration["MongoSettings:Port"];
        //var username = _configuration["MongoSettings:Username"];
        //var password = _configuration["MongoSettings:Password"];
        
        var section = _configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();

        // Configure mongo (You can inject the config, just to simplify)
        var connectionString = GetConnectionString(settings);
        MongoClient = new MongoClient(connectionString);
        Database = MongoClient.GetDatabase(_configuration["MongoSettings:DatabaseName"]);
        //var collection = Database.GetCollection<Domain.User>(settings.TableName);
        //var user = new Domain.User() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Morsley" };
        //collection.InsertOne(user);
    }

    public bool IsHealthy()
    {
        try
        {
            ConfigureMongo();
            
            
            return true;
        }
        catch (Exception e)
        {
            // ToDo --> Log e            
        }
        return false;
    }

    private string GetConnectionString(MongoSettings settings)
    {
        return $"mongodb://{settings.Username}:{settings.Password}@{settings.Host}:{settings.Port}";
    }
}