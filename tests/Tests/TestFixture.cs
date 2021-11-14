namespace IntegrationTests;

//[SetUpFixture]
public class TestFixture
{
    //private static IConfigurationRoot _configuration; 
    
    private string? _dockerContainerId;
    private int _dockerContainerPort;

    public MongoSettings _mongoSsettings { get; }

    public TestFixture(MongoSettings settings)
    {
        _mongoSsettings = settings;
    }

    //[OneTimeSetUp]
    public async Task RunBeforeTests()
    {
        (_dockerContainerId, var dockerContainerPort) = await MongoDBInDocker.EnsureDockerStartedAndGetContainerIdAndPortAsync(_mongoSsettings);

        _dockerContainerPort = Convert.ToInt32(dockerContainerPort);

        //var context = new MongoContext();

        //EnsureDatabase();
    }

    //[OneTimeTearDown]
    public async Task RunAfterTests()
    {
        await MongoDBInDocker.EnsureDockerContainersStoppedAndRemovedAsync(_dockerContainerId);
    }

    public int GetContainerPort()
    {
        //var port = await MongoDBInDocker.GetPort();
        //await _checkpoint.Reset(_configuration.GetConnectionString("AccessioningDbContext"));
        //throw new NotImplementedException();
        //return string.Empty;
        //throw new NotImplementedException();

        return _dockerContainerPort;
    }

    //private static void EnsureDatabase()
    //{
    //    using var scope = _scopeFactory.CreateScope();

    //    var context = scope.ServiceProvider.GetService<AccessioningDbContext>();

    //    context.Database.Migrate();
    //}

    public string MongoDBConnectionString
    {
        get { return MongoDBInDocker.ConnectionString(_mongoSsettings, _dockerContainerPort); }
    }

    //public string DatabaseName 
    //{
    //    get { return MongoDBInDocker. }
    //}

    
}
