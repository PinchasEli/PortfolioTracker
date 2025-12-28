namespace API.DTOs.Common;

/// <summary>
/// Global pagination request parameters that can be used across all list endpoints
/// </summary>
public class PaginationRequest
{
    /// <summary>
    /// Page number (1-based). Defaults to 1.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of items per page. Defaults to 10. Maximum is 100.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Validates and normalizes pagination parameters
    /// </summary>
    public void Normalize()
    {
        // Ensure page number is at least 1
        if (PageNumber < 1)
            PageNumber = 1;

        // Ensure page size is between 1 and 100
        if (PageSize < 1)
            PageSize = 10;
        else if (PageSize > 100)
            PageSize = 100;
    }
}

