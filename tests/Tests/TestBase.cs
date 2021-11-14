namespace IntegrationTests;

public class TestBase
{
    //protected readonly IConfiguration _configuration;
    //protected readonly MongoSettings _mongoSettings;
    protected readonly TestFixture _fixture;    


    public TestBase()
    {
        //var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration();
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();

        _fixture = new TestFixture(settings);
    }

    [SetUp]
    public async Task TestSetUp()
    {        
        await _fixture.RunBeforeTests();
    }

    [TearDown]
    public async Task TestTearDown()
    {
        await _fixture.RunAfterTests();
    }

    internal int ContainerPort
    {
        get { return _fixture.GetContainerPort(); }        
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
}
