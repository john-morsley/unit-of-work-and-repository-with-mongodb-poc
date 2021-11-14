//namespace IntegrationTests;

//public class Tests : TestBase
//{
//    [Test]
//    public async Task Add_A_User_Should_Result_In_That_User_Being_Added()
//    {
//        // Arrange...
//        var additional = GetInMemoryConfiguration();        
//        var configuration = GetConfiguration(additional);
//        var context = new UnitOfWork.MongoContext(configuration);
//        Assert.IsTrue(context.IsHealthy());
//        var sut = new UnitOfWork.UnitOfWork(context);

//        // Act...
//        var userId = Guid.NewGuid();
//        var user = new Domain.User() { Id = userId, FirstName = "John", LastName = "Doe" };
//        sut.UserRepository.Add(user);
//        await sut.Commit();

//        // Assert...
//        Assert.IsTrue(true);
//    }

//    private Dictionary<string, string> GetInMemoryConfiguration()
//    {
//        var additional = new Dictionary<string, string>();
//        additional.Add("MongoSettings:Port", ContainerPort);
//        return additional;
//    }
//}
