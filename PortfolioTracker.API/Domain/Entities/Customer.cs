namespace Domain.Entities;

public class Customer : AuditableEntity
{
    public Guid UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public User User { get; set; } = null!;

    // Navigation
    public ICollection<Portfolio> Portfolios { get; set; } = new List<Portfolio>();

}
