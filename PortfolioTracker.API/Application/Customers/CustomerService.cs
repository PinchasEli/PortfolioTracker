using Infrastructure.Persistence;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Security;
using API.DTOs.Customers;
using API.DTOs.Common;
using Microsoft.EntityFrameworkCore;
using Application.Common;

namespace Application.Customers;

public class CustomerService : ICustomerService
{
    private readonly ApplicationDbContext _context;
    private readonly Infrastructure.Authorization.IAuthorizationService _authService;

    public CustomerService(
        ApplicationDbContext context,
        Infrastructure.Authorization.IAuthorizationService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<ServiceResult<CustomerResponse>> CreateAsync(CreateCustomerRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return ServiceResult.Fail<CustomerResponse>("Email already exists");

        var user = new User
        {
            Email = request.Email,
            PasswordHash = PasswordHasher.HashPassword(request.Password),
            Role = Role.Customer
        };

        var customer = new Customer
        {
            User = user,
            FullName = request.FullName
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        return ServiceResult.Ok(new CustomerResponse
        {
            Id = customer.Id,
            FullName = customer.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            Active = user.Active,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        });
    }

    public async Task<ServiceResult<PaginatedResponse<CustomerResponse>>> GetAllAsync(PaginationRequest paginationRequest)
    {
        // Build the query for customers with their user data
        var query = _context.Customers
            .Include(c => c.User)
            .Select(c => new CustomerResponse
            {
                Id = c.Id,
                FullName = c.FullName,
                Email = c.User.Email,
                Role = c.User.Role.ToString(),
                Active = c.User.Active,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });

        // Apply pagination using our extension method
        var paginatedResult = await query.ToPaginatedResponseAsync(paginationRequest);

        return ServiceResult.Ok(paginatedResult);
    }

    public async Task<ServiceResult<CustomerResponse>> GetByIdAsync(Guid id)
    {
        var currentUserId = _authService.GetCurrentUserId();
        var isAdminOrAbove = _authService.IsAtLeastRole(Role.Admin);

        var query = _context.Customers
            .Include(c => c.User)
            .Where(c => c.Id == id);

        if (!isAdminOrAbove)
            query = query.Where(c => c.UserId == currentUserId);
        
        var customer = await query.FirstOrDefaultAsync();

        if (customer == null)
            return ServiceResult.Fail<CustomerResponse>("Customer not found");

        return ServiceResult.Ok(new CustomerResponse
        {
            Id = customer.Id,
            FullName = customer.FullName,
            Email = customer.User.Email,
            Role = customer.User.Role.ToString(),
            Active = customer.User.Active,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        });
    }

    public async Task<ServiceResult<CustomerResponse>> UpdateAsync(Guid id, UpdateCustomerRequest request)
    {
        var customer = await _context.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null)
            return ServiceResult.Fail<CustomerResponse>("Customer not found");

        // Check if email is being changed and if new email already exists
        if (customer.User.Email != request.Email)
        {
            var emailExists = await _context.Users.AnyAsync(u => u.Email == request.Email && u.Id != customer.UserId);
            if (emailExists)
                return ServiceResult.Fail<CustomerResponse>("Email already exists");
        }

        // Update customer fields
        customer.FullName = request.FullName;
        customer.User.Email = request.Email;
        customer.User.Active = request.Active;

        await _context.SaveChangesAsync();

        return ServiceResult.Ok(new CustomerResponse
        {
            Id = customer.Id,
            FullName = customer.FullName,
            Email = customer.User.Email,
            Role = customer.User.Role.ToString(),
            Active = customer.User.Active,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        });
    }

    public async Task<ServiceResult<CustomerResponse>> PatchAsync(Guid id, PatchCustomerRequest request)
    {
        var customer = await _context.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null)
            return ServiceResult.Fail<CustomerResponse>("Customer not found");

        // Only update fields that are provided (not null)
        if (request.FullName != null)
        {
            customer.FullName = request.FullName;
        }

        if (request.Email != null)
        {
            // Check if email is being changed and if new email already exists
            if (customer.User.Email != request.Email)
            {
                var emailExists = await _context.Users.AnyAsync(u => u.Email == request.Email && u.Id != customer.UserId);
                if (emailExists)
                    return ServiceResult.Fail<CustomerResponse>("Email already exists");
            }

            customer.User.Email = request.Email;
        }

        if (request.Active.HasValue)
        {
            customer.User.Active = request.Active.Value;
        }

        await _context.SaveChangesAsync();

        return ServiceResult.Ok(new CustomerResponse
        {
            Id = customer.Id,
            FullName = customer.FullName,
            Email = customer.User.Email,
            Role = customer.User.Role.ToString(),
            Active = customer.User.Active,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        });
    }
}
