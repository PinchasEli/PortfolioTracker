namespace API.DTOs.Common;

/// <summary>
/// Generic sort order enum for all list endpoints
/// </summary>
public enum SortOrder
{
    /// <summary>
    /// Ascending order (A-Z, 0-9, oldest to newest)
    /// </summary>
    Asc,
    
    /// <summary>
    /// Descending order (Z-A, 9-0, newest to oldest)
    /// </summary>
    Desc
}

