namespace IntegrationTests;

public class UserRepositoryTests : TestBase
{
    [Test]
    public async Task Adding_A_User_Should_Result_In_That_User_Being_Added()
    {
        // Arrange...
        var configuration = GetCurrentConfiguration();
        var context = new MongoContext(configuration);
        context.IsHealthy().Should().BeTrue();
        NumberOfUsersInDatabase().Should().Be(0);
        var sut = new UserRepository(context);
        var userId = Guid.NewGuid();
        var user = new Domain.Models.User() { Id = userId, FirstName = "John", LastName = "Doe" };

        // Act...
        await sut.CreateAsync(user);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);
    }

    [Test]
    public async Task Deleting_A_User_Should_Result_In_That_User_Being_Deleted()
    {
        // Arrange...
        var configuration = GetCurrentConfiguration();
        var context = new MongoContext(configuration);
        context.IsHealthy().Should().BeTrue();        
        NumberOfUsersInDatabase().Should().Be(0);
        var sut = new UserRepository(context);
        var userId = Guid.NewGuid();
        var user = new Domain.Models.User() { Id = userId, FirstName = "John", LastName = "Doe" };
        AddUserToDatabase(user);
        NumberOfUsersInDatabase().Should().Be(1);

        // Act...
        await sut.DeleteAsync(userId);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(0);
    }

    private void AddUserToDatabase(Domain.Models.User user)
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();

        var connectionString = _fixture.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        //var section = _configuration.GetSection(nameof(MongoSettings));
        //var settings = section.Get<MongoSettings>();
        var users = database.GetCollection<Domain.Models.User>(settings.TableName);
        users.InsertOne(user);
    }

    private long NumberOfUsersInDatabase()
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();

        var connectionString = _fixture.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        //var section = _configuration.GetSection(nameof(MongoSettings));
        //var settings = section.Get<MongoSettings>();
        var users = database.GetCollection<Domain.Models.User>(settings.TableName);
        //var filter = FilterDefinition<Domain.User>().;
        var numberOfUsers = users.Find(_ => true).CountDocuments();
        return numberOfUsers;
    }
}
