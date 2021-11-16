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
        var user = GenerateTestUser(userId);

        // Act...
        await sut.CreateAsync(user);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);
        var added = GetUserFromDatabase(userId);
        added.Id.Should().Be(userId);
        added.FirstName.Should().Be(user.FirstName);
        added.LastName.Should().Be(user.LastName);
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
        var existing = GenerateTestUser(userId);
        AddUserToDatabase(existing);
        NumberOfUsersInDatabase().Should().Be(1);

        // Act...
        existing.FirstName = _autoFixture.Create<string>();
        existing.LastName = _autoFixture.Create<string>();
        await sut.UpdateAsync(existing);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);
        var updated = GetUserFromDatabase(userId);
        updated.Id.Should().Be(userId);
        updated.FirstName.Should().Be(existing.FirstName);
        updated.LastName.Should().Be(existing.LastName);
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
        var user = GenerateTestUser(userId);
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
        var user = GenerateTestUser(userId);
        AddUserToDatabase(user);
        NumberOfUsersInDatabase().Should().Be(1);

        // Act...
        var result = await sut.GetByIdAsync(userId);

        // Assert...
        NumberOfUsersInDatabase().Should().Be(1);
        result.Id.Should().Be(userId);
        result.FirstName.Should().Be(user.FirstName);
        result.LastName.Should().Be(user.LastName);
    }

    [Test]
    public async Task Getting_A_Page_Of_Users_With_No_Pagination_Options_Should_Result_In_A_Page_Of_All_Users_Being_Returned()
    {
        // Arrange...
        var configuration = GetCurrentConfiguration();
        var context = new MongoContext(configuration);
        context.IsHealthy().Should().BeTrue();
        NumberOfUsersInDatabase().Should().Be(0);
        var sut = new UserRepository(context);
        AddUsersToDatabase(10);
        NumberOfUsersInDatabase().Should().Be(10);

        // Act...
        var getUsersOptions = new GetOptions();
        var pageOfUsers = await sut.GetPageAsync(getUsersOptions);

        // Assert...
        pageOfUsers.Count.Should().Be(10);
        var usersInDatabase = GetUsersFromDatabase().ToList();
        var firstUserOnPage = pageOfUsers.First();
        var lastUserOnPage = pageOfUsers.Last();
        var firstUserInDatabase = usersInDatabase.First();
        var lastUserInDatabase = usersInDatabase.Last();
        firstUserOnPage.Should().Equals(firstUserInDatabase);
        lastUserOnPage.Should().Equals(lastUserInDatabase);
    }

    [Test]
    public async Task Getting_A_Page_Of_Users_With_Pagination_Options_Should_Result_In_A_Page_Of_Users_Being_Returned()
    {
        // Arrange...
        var configuration = GetCurrentConfiguration();
        var context = new MongoContext(configuration);
        context.IsHealthy().Should().BeTrue();
        NumberOfUsersInDatabase().Should().Be(0);
        var sut = new UserRepository(context);
        AddUsersToDatabase(10);
        NumberOfUsersInDatabase().Should().Be(10);

        // Act...
        var getUsersOptions = new GetOptions();
        getUsersOptions.PageSize = 1;
        getUsersOptions.PageNumber = 1;
        var pageOfUsers = await sut.GetPageAsync(getUsersOptions);

        // Assert...
        pageOfUsers.Count.Should().Be(1);
        var usersInDatabase = GetUsersFromDatabase().ToList();
        var firstUserOnPage = pageOfUsers.First();
        var lastUserOnPage = pageOfUsers.Last();
        var firstUserInDatabase = usersInDatabase.First();
        firstUserOnPage.Should().Equals(firstUserInDatabase);
        lastUserOnPage.Should().Equals(firstUserInDatabase);
    }

    [Test]
    public async Task Getting_A_Page_Of_Users_With_Search_Criteria_Should_Result_In_A_Page_Of_Users_That_Match_The_Search_Being_Returned()
    {
        // Arrange...
        var configuration = GetCurrentConfiguration();
        var context = new MongoContext(configuration);
        context.IsHealthy().Should().BeTrue();
        NumberOfUsersInDatabase().Should().Be(0);
        var sut = new UserRepository(context);
        AddUsersToDatabase(5);
        var expected = new Domain.Models.User() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Morsley" };
        AddUserToDatabase(expected);
        AddUsersToDatabase(5);
        NumberOfUsersInDatabase().Should().Be(11);

        // Act...
        var getUsersOptions = new GetOptions();
        getUsersOptions.PageSize = 1;
        getUsersOptions.PageNumber = 1;
        getUsersOptions.SearchQuery = "orsle";
        var pageOfUsers = await sut.GetPageAsync(getUsersOptions);

        // Assert...
        pageOfUsers.Count.Should().Be(1);
        var firstUserOnPage = pageOfUsers.First();
        firstUserOnPage.Should().Equals(expected);        
    }

    [Test]
    public async Task Getting_A_Page_Of_Users_With_Filter_Criteria_Should_Result_In_A_Page_Of_Users_That_Match_The_Filter()
    {
        // Arrange...
        var configuration = GetCurrentConfiguration();
        var context = new MongoContext(configuration);
        context.IsHealthy().Should().BeTrue();
        NumberOfUsersInDatabase().Should().Be(0);
        var sut = new UserRepository(context);
        AddUsersToDatabase(5);
        var john = new Domain.Models.User() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
        AddUserToDatabase(john);
        var jane = new Domain.Models.User() { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Doe" };
        AddUserToDatabase(jane);
        AddUsersToDatabase(5);
        NumberOfUsersInDatabase().Should().Be(12);

        // Act...
        var getUsersOptions = new GetOptions();
        getUsersOptions.PageSize = 2;
        getUsersOptions.PageNumber = 1;
        getUsersOptions.AddFilter(new Filter("LastName", "Doe"));
        var pageOfUsers = await sut.GetPageAsync(getUsersOptions);

        // Assert...
        pageOfUsers.Count.Should().Be(2);
        var firstUserOnPage = pageOfUsers.First();
        firstUserOnPage.Should().Equals(john);
        var secondUserOnPage = pageOfUsers.Skip(1).First();
        secondUserOnPage.Should().Equals(jane);
    }
}
