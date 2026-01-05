using Domain.Enums;

namespace API.DTOs.Portfolio;

public class PatchPortfolioRequest
{
    public string? Name { get; set; }
    public Currency? BaseCurrency { get; set; }
    public bool? Active { get; set; }
}
