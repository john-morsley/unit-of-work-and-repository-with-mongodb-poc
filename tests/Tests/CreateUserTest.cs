namespace IntegrationTests;

public class Tests : TestBase
{
    [Test]
    public void Add_A_User_Should_Result_In_That_User_Being_Added()
    {
        // Arrange...
        var additional = GetInMemoryConfiguration();        
        var configuration = GetConfiguration(additional);
        var context = new UnitOfWork.MongoContext(configuration);
        var sut = new UnitOfWork.UnitOfWork(context);
       
        // Act...
        //sut.

        // Assert...
        Assert.IsTrue(true);
    }

    private Dictionary<string, string> GetInMemoryConfiguration()
    {
        var additional = new Dictionary<string, string>();
        additional.Add("MongoSettings:Port", ContainerPort);
        return additional;
    }
}
