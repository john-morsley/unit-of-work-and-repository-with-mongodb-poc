namespace Domain.Models;

public class User : Entity<Guid>
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }
}