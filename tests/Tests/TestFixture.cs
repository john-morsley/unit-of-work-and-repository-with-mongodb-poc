namespace IntegrationTests;

//[SetUpFixture]
public class TestFixture
{
    //private static IConfigurationRoot _configuration; 
    
    private string _dockerContainerId;
    private string _dockerContainerPort;
    
    public int MyProperty;

    //[OneTimeSetUp]
    public async Task RunBeforeTests()
    {
        (_dockerContainerId, _dockerContainerPort) = await MongoDBInDocker.EnsureDockerStartedAndGetContainerIdAndPortAsync();

        //var context = new MongoContext();

        //EnsureDatabase();
    }

    //[OneTimeTearDown]
    public async Task RunAfterTests()
    {
        await MongoDBInDocker.EnsureDockerContainersStoppedAndRemovedAsync(_dockerContainerId);
    }

    public string GetContainerPort()
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
}
