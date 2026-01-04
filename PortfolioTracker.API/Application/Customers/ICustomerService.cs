using API.DTOs.Customers;
using API.DTOs.Common;
using Application.Common;

namespace Application.Customers;

public interface ICustomerService
{
    Task<ServiceResult<CustomerResponse>> CreateAsync(CreateCustomerRequest request);
    Task<ServiceResult<PaginatedResponse<CustomerResponse>>> GetAllAsync(PaginationRequest paginationRequest);
    Task<ServiceResult<CustomerResponse>> GetByIdAsync(Guid id);
    Task<ServiceResult<CustomerResponse>> UpdateAsync(Guid id, UpdateCustomerRequest request);
    Task<ServiceResult<CustomerResponse>> PatchAsync(Guid id, PatchCustomerRequest request);
}

