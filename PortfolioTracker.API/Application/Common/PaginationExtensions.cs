using API.DTOs.Common;
using Microsoft.EntityFrameworkCore;

namespace Application.Common;

/// <summary>
/// Extension methods for applying pagination to IQueryable queries
/// </summary>
public static class PaginationExtensions
{
    /// <summary>
    /// Applies pagination to an IQueryable and returns a PaginatedResponse with the data and metadata
    /// </summary>
    /// <typeparam name="T">The entity type being queried</typeparam>
    /// <param name="query">The IQueryable to paginate</param>
    /// <param name="paginationRequest">The pagination parameters</param>
    /// <returns>A PaginatedResponse containing the items and pagination metadata</returns>
    public static async Task<PaginatedResponse<T>> ToPaginatedResponseAsync<T>(
        this IQueryable<T> query,
        PaginationRequest paginationRequest)
    {
        // Normalize pagination parameters (ensure valid page number and size)
        paginationRequest.Normalize();

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination: skip items from previous pages and take items for current page
        var items = await query
            .Skip((paginationRequest.PageNumber - 1) * paginationRequest.PageSize)
            .Take(paginationRequest.PageSize)
            .ToListAsync();

        // Return paginated response with data and metadata
        return new PaginatedResponse<T>(
            items,
            totalCount,
            paginationRequest.PageNumber,
            paginationRequest.PageSize
        );
    }

    /// <summary>
    /// Applies pagination to an IQueryable with projection and returns a PaginatedResponse
    /// </summary>
    /// <typeparam name="TSource">The source entity type</typeparam>
    /// <typeparam name="TDestination">The destination DTO type</typeparam>
    /// <param name="query">The IQueryable to paginate</param>
    /// <param name="paginationRequest">The pagination parameters</param>
    /// <param name="projection">The projection function to transform entities to DTOs</param>
    /// <returns>A PaginatedResponse containing the projected items and pagination metadata</returns>
    public static async Task<PaginatedResponse<TDestination>> ToPaginatedResponseAsync<TSource, TDestination>(
        this IQueryable<TSource> query,
        PaginationRequest paginationRequest,
        Func<TSource, TDestination> projection)
    {
        // Normalize pagination parameters
        paginationRequest.Normalize();

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination and then project to DTOs
        var items = await query
            .Skip((paginationRequest.PageNumber - 1) * paginationRequest.PageSize)
            .Take(paginationRequest.PageSize)
            .ToListAsync();

        // Apply projection to transform entities to DTOs
        var projectedItems = items.Select(projection);

        // Return paginated response with data and metadata
        return new PaginatedResponse<TDestination>(
            projectedItems,
            totalCount,
            paginationRequest.PageNumber,
            paginationRequest.PageSize
        );
    }
}

