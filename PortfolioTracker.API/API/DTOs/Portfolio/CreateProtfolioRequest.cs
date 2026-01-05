using Domain.Enums;

namespace API.DTOs.Portfolio;

public class CreatePortfolioRequest
{
    public string Name { get; set; } = string.Empty;
    public Exchange Exchange { get; set; }
    public Currency BaseCurrency { get; set; } = Currency.USD;
}