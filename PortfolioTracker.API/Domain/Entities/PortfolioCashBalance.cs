using Domain.Enums;

namespace Domain.Entities;


public class PortfolioCashBalance : AuditableEntity
{
    public Guid PortfolioId { get; set; }

    public decimal Amount { get; set; } = 0m;

    public Currency Currency { get; set; } = Currency.USD;

    public Portfolio Portfolio { get; set; } = null!;
}