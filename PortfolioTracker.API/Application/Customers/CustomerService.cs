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

    public CustomerService(ApplicationDbContext context)
    {
        _context = context;
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
        var customer = await _context.Customers
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.Id == id);
            
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
}
