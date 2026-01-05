using Infrastructure.Persistence;
using Infrastructure.Authorization;
using Application.Common;
using API.DTOs.Portfolio;
using API.DTOs.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Domain.Enums;

namespace Application.Portfolios;

public class PortfolioService : IPortfolioService
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthorizationService _authService;
    private readonly ILogger<PortfolioService> _logger;

    public PortfolioService(ApplicationDbContext context, IAuthorizationService authorizationService, ILogger<PortfolioService> logger)
    {
        _context = context;
        _authService = authorizationService;
        _logger = logger;
    }

    /// <summary>
    /// Validates customer exists and user has authorization to access it
    /// </summary>
    private async Task<ServiceResult<Customer>> ValidateCustomerAccessAsync(Guid customerId)
    {
        var currentUserId = _authService.GetCurrentUserId();
        var isAdminOrAbove = _authService.IsAtLeastRole(Role.Admin);

        var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Id == customerId);

        if (customer == null)
            return ServiceResult.Fail<Customer>("Customer not found", 404);

        if (!isAdminOrAbove && customer.UserId != currentUserId)
            return ServiceResult.Fail<Customer>("Access denied", 403);

        return ServiceResult.Ok(customer);
    }

    public async Task<ServiceResult<PortfolioResponse>> CreateAsync(Guid customerId, CreatePortfolioRequest request)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        if (customer == null)
            return ServiceResult.Fail<PortfolioResponse>("Customer not found");

        // Check for duplicate portfolio (race condition prevention)
        var existingPortfolio = await _context.Portfolios
            .FirstOrDefaultAsync(p =>
                p.CustomerId == customerId &&
                p.Name == request.Name &&
                p.Exchange == request.Exchange);

        if (existingPortfolio != null)
            return ServiceResult.Conflict<PortfolioResponse>("A portfolio with this name and exchange already exists for this customer");

        var portfolio = new Portfolio
        {
            CustomerId = customer.Id,
            Name = request.Name,
            Exchange = request.Exchange,
            BaseCurrency = request.BaseCurrency,
            Active = true,
        };

        try
        {
            _context.Portfolios.Add(portfolio);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            // Handle unique constraint violation (race condition edge case)
            if (ex.InnerException?.Message.Contains("duplicate") == true ||
                ex.InnerException?.Message.Contains("unique") == true ||
                ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                return ServiceResult.Conflict<PortfolioResponse>("A portfolio with this name and exchange already exists for this customer");
            }

            // Re-throw if it's a different database error
            throw;
        }

        return ServiceResult.Ok(new PortfolioResponse
        {
            Id = portfolio.Id,
            Name = portfolio.Name,
            Exchange = portfolio.Exchange,
            BaseCurrency = portfolio.BaseCurrency,
            Active = portfolio.Active,
            CreatedAt = portfolio.CreatedAt,
            UpdatedAt = portfolio.UpdatedAt
        });
    }

    public async Task<ServiceResult<PaginatedResponse<PortfolioResponse>>> GetAllAsync(Guid customerId, PaginationRequest paginationRequest)
    {
        // Validate customer access
        var validationResult = await ValidateCustomerAccessAsync(customerId);
        if (!validationResult.Success)
            return ServiceResult.Fail<PaginatedResponse<PortfolioResponse>>(validationResult.ErrorMessage!, validationResult.StatusCode ?? 400);

        var customer = validationResult.Data!;

        // Get portfolios for the customer with pagination
        var portfoliosQuery = _context.Portfolios
            .Where(p => p.CustomerId == customerId)
            .OrderByDescending(p => p.CreatedAt);

        // Apply pagination and project to PortfolioResponse
        var paginatedResult = await portfoliosQuery.ToPaginatedResponseAsync(
            paginationRequest,
            p => new PortfolioResponse
            {
                Id = p.Id,
                Name = p.Name,
                Exchange = p.Exchange,
                BaseCurrency = p.BaseCurrency,
                Active = p.Active,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            });

        return ServiceResult.Ok(paginatedResult);
    }

    public async Task<ServiceResult<PortfolioResponse>> GetByIdAsync(Guid customerId, Guid portfolioId)
    {
        // Validate customer access
        var validationResult = await ValidateCustomerAccessAsync(customerId);
        if (!validationResult.Success)
            return ServiceResult.Fail<PortfolioResponse>(validationResult.ErrorMessage!, validationResult.StatusCode ?? 400);

        var portfolio = await _context.Portfolios
            .FirstOrDefaultAsync(p => p.Id == portfolioId && p.CustomerId == customerId);

        if (portfolio == null)
            return ServiceResult.Fail<PortfolioResponse>("Portfolio not found", 404);

        return ServiceResult.Ok(new PortfolioResponse
        {
            Id = portfolio.Id,
            Name = portfolio.Name,
            Exchange = portfolio.Exchange,
            BaseCurrency = portfolio.BaseCurrency,
            Active = portfolio.Active,
            CreatedAt = portfolio.CreatedAt,
            UpdatedAt = portfolio.UpdatedAt
        });
    }
}