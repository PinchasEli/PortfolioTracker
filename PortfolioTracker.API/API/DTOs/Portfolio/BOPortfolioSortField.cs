namespace API.DTOs.Portfolio;

/// <summary>
/// Available fields to sort portfolios by
/// </summary>
public enum BOPortfolioSortField
{
    /// <summary>
    /// Sort by portfolio name
    /// </summary>
    Name,
    
    /// <summary>
    /// Sort by customer name
    /// </summary>
    Customer,
    
    /// <summary>
    /// Sort by exchange
    /// </summary>
    Exchange,
    
    /// <summary>
    /// Sort by base currency
    /// </summary>
    BaseCurrency,
    
    /// <summary>
    /// Sort by active status
    /// </summary>
    Active,
    
    /// <summary>
    /// Sort by creation date (default)
    /// </summary>
    CreatedAt,
    
    /// <summary>
    /// Sort by last update date
    /// </summary>
    UpdatedAt
}

