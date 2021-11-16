namespace IntegrationTests;

public class UserRepositoryTests : TestBase
{
    private readonly Fixture _autoFixture;

    public UserRepositoryTests()
    {
        _autoFixture = new Fixture();
    }

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
        var added = GetUserFromDatabase(user.Id);
        added.FirstName.Should().Be("John");
        added.LastName.Should().Be("Doe");
    }

    [Test]
    public async Task Updating_A_User_Should_Result_In_That_User_Being_Updated()
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
        user.FirstName = "Joe";
        user.LastName = "Bloggs";
        await sut.UpdateAsync(user);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);
        var updated = GetUserFromDatabase(user.Id);
        updated.FirstName.Should().Be("Joe");
        updated.LastName.Should().Be("Bloggs");
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

    [Test]
    public async Task Getting_A_User_Should_Result_In_That_User_Being_Returned()
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
        var result = await sut.GetByIdAsync(userId);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
    }

    [Test]
    public async Task Getting_A_Page_Of_Users_With_Empty_Get_Options_Should_Result_In_A_Page_Of_All_Users_Being_Returned()
    {
        // Arrange...
        var configuration = GetCurrentConfiguration();
        var context = new MongoContext(configuration);
        context.IsHealthy().Should().BeTrue();
        NumberOfUsersInDatabase().Should().Be(0);
        var sut = new UserRepository(context);
        AddMultipleUsersToDatabase(10);
        NumberOfUsersInDatabase().Should().Be(10);

        // Act...
        var getUsersOptions = new GetOptions();
        var pageOfUsers = await sut.GetPageAsync(getUsersOptions);

        // Assert...
        pageOfUsers.Count.Should().Be(10);
        //NumberOfUsersInDatabase().Should().Be(1);
        //result.FirstName.Should().Be("John");
        //result.LastName.Should().Be("Doe");
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
        var users = database.GetCollection<Domain.Models.User>(settings.TableName);
        users.InsertOne(user);
    }

    private void AddMultipleUsersToDatabase(int numberOfUsersToAdd)
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = _fixture.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var collection = database.GetCollection<Domain.Models.User>(settings.TableName);
        var users = new List<Domain.Models.User>();
        for (int i = 0; i < numberOfUsersToAdd; i++)
        {
            var user = _autoFixture.Create<Domain.Models.User>();
            users.Add(user);
        }
        collection.InsertMany(users);
    }

    private Domain.Models.User GetUserFromDatabase(Guid userId)
    {
        var additional = GetInMemoryConfiguration();
        var configuration = GetConfiguration(additional);
        var section = configuration.GetSection(nameof(MongoSettings));
        var settings = section.Get<MongoSettings>();
        var connectionString = _fixture.MongoDBConnectionString;
        var mongoClient = new MongoClient(connectionString);
        var database = mongoClient.GetDatabase(settings.DatabaseName);
        var users = database.GetCollection<Domain.Models.User>(settings.TableName);
        var user = users.Find<Domain.Models.User>(user => user.Id == userId).SingleOrDefault();
        return user;
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
        var users = database.GetCollection<Domain.Models.User>(settings.TableName);
        var numberOfUsers = users.Find(_ => true).CountDocuments();
        return numberOfUsers;
    }
}
