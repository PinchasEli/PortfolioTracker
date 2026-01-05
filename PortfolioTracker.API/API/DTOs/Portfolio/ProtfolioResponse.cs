using Domain.Enums;

namespace API.DTOs.Portfolio;

public class PortfolioResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Exchange Exchange { get; set; }
    public Currency BaseCurrency { get; set; } = Currency.USD;
    public bool Active { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class BOPortfolioResponse : PortfolioResponse
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
}
