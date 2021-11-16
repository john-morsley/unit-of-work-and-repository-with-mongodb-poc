namespace IntegrationTests;

public class TestBase
{
    protected readonly TestFixture _testFixture;
    protected readonly Fixture _autoFixture;

    public TestBase()
    {
        var configuration = GetConfiguration();
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();

        _testFixture = new TestFixture(settings);
        _autoFixture = new Fixture();
    }

    [SetUp]
    public async Task TestSetUp()
    {        
        await _testFixture.RunBeforeTests();
    }

    [TearDown]
    public async Task TestTearDown()
    {
        await _testFixture.RunAfterTests();
    }

    internal int ContainerPort
    {
        get { return _testFixture.GetContainerPort(); }        
    }

    protected Dictionary<string, string> GetInMemoryConfiguration()
    {
        var additional = new Dictionary<string, string>();
        additional.Add("MongoSettings:Port", ContainerPort.ToString());
        return additional;
    }

    protected IConfiguration GetConfiguration(Dictionary<string, string>? additional = null)
    {
        var builder = new ConfigurationBuilder();

        // Add values from the 'appsettings.json' file...
        builder.AddJsonFile("appsettings.json");

        // Add static values...
        if (additional != null && additional.Count > 0) builder.AddInMemoryCollection(additional);

        IConfiguration configuration = builder.Build();

        return configuration;
    }

    protected IConfiguration GetCurrentConfiguration()
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        return configuration;
    }

    protected void AddUserToDatabase(Domain.Models.User user)
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = _testFixture.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var users = database.GetCollection<Domain.Models.User>(settings.TableName);
        users.InsertOne(user);
    }

    protected void AddUsersToDatabase(int numberOfUsersToAdd)
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = _testFixture.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var collection = database.GetCollection<Domain.Models.User>(settings.TableName);
        var users = new List<Domain.Models.User>();
        for (int i = 0; i < numberOfUsersToAdd; i++)
        {
            var user = GenerateTestUser();
            users.Add(user);
        }
        collection.InsertMany(users);
    }

    protected Domain.Models.User GenerateTestUser()
    {
        var user = _autoFixture.Create<Domain.Models.User>();
        return user;
    }

    protected Domain.Models.User GenerateTestUser(Guid userId)
    {
        var user = GenerateTestUser();
        user.Id = userId;
        return user;
    }

    protected Domain.Models.User GetUserFromDatabase(Guid userId)
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = _testFixture.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var users = database.GetCollection<Domain.Models.User>(settings.TableName);
        var user = users.Find<Domain.Models.User>(user => user.Id == userId).SingleOrDefault();
        return user;
    }

    protected IQueryable<Domain.Models.User> GetUsersFromDatabase()
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = _testFixture.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var users = database.GetCollection<Domain.Models.User>(settings.TableName);
        return users.AsQueryable();
    }

    protected long NumberOfUsersInDatabase()
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = _testFixture.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var users = database.GetCollection<Domain.Models.User>(settings.TableName);
        var numberOfUsers = users.Find(_ => true).CountDocuments();
        return numberOfUsers;
    }
}
