using API.DTOs.Customers;
using Application.Common;

namespace Application.Customers;

public interface ICustomerService
{
    Task<ServiceResult<CustomerResponse>> CreateAsync(CreateCustomerRequest request);
    Task<ServiceResult<IEnumerable<CustomerResponse>>> GetAllAsync();
}

