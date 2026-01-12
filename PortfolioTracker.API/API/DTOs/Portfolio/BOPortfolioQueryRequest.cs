using API.DTOs.Common;
using Domain.Enums;

namespace API.DTOs.Portfolio;


public class BOPortfolioQueryRequest : PaginationRequest
{
    /// Search term to filter by portfolio name or customer name
    public string? Search { get; set; }

    /// Filter by active status (true/false/null for all)
    public bool? Active { get; set; }
    /// Filter by exchange
    public Exchange? Exchange { get; set; }
    /// Filter by base currency
    public Currency? BaseCurrency { get; set; }
    /// Filter by customer ID
    public Guid? CustomerId { get; set; }

    /// <summary>
    /// Sort by field
    /// Default: CreatedAt
    /// </summary>
    public BOPortfolioSortField SortBy { get; set; } = BOPortfolioSortField.CreatedAt;

    /// <summary>
    /// Sort order: Asc (ascending) or Desc (descending)
    /// Default: Desc
    /// </summary>
    public SortOrder SortOrder { get; set; } = SortOrder.Desc;
}

