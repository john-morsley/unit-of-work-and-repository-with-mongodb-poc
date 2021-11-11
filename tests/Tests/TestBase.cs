namespace IntegrationTests;

public class TestBase
{
    private TestFixture _fixture;

    [SetUp]
    public async Task TestSetUp()
    {
        _fixture = new TestFixture();
        await _fixture.RunBeforeTests();
    }

    [TearDown]
    public async Task TestTearDown()
    {
        await _fixture.RunAfterTests();
    }

    internal string ContainerPort
    {
        get { return _fixture.GetContainerPort(); }
        
    }

    internal IConfiguration GetConfiguration(Dictionary<string, string>? additional = null)
    {
        var builder = new ConfigurationBuilder();

        // Add static values...
        if (additional != null && additional.Count > 0) builder.AddInMemoryCollection(additional);

        // Add values from a JSON file...
        builder.AddJsonFile("appsettings.json");

        IConfiguration configuration = builder.Build();

        return configuration;
    }
}
