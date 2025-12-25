using Domain.Enums;

namespace Domain.Entities;

public class Portfolio : AuditableEntity
{
    public Guid CustomerId { get; set; }

    public string Name { get; set; } = string.Empty;

    public Exchange Exchange { get; set; }
    public Currency BaseCurrency { get; set; } = Currency.USD;

    public bool Active { get; set; } = true;

    // Navigation
    public Customer Customer { get; set; } = null!;
    public ICollection<PortfolioCashBalance> CashBalances { get; set; } = new List<PortfolioCashBalance>();
}
