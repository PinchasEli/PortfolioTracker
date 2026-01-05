using API.DTOs.Portfolio;
using Application.Common;
using API.DTOs.Common;

namespace Application.Portfolios;

public interface IPortfolioService
{
    Task<ServiceResult<PortfolioResponse>> CreateAsync(Guid customerId, CreatePortfolioRequest request);
    Task<ServiceResult<PaginatedResponse<PortfolioResponse>>> GetAllAsync(Guid customerId, PaginationRequest paginationRequest);
    Task<ServiceResult<PortfolioResponse>> GetByIdAsync(Guid customerId, Guid portfolioId);
    Task<ServiceResult<PortfolioResponse>> PatchAsync(Guid customerId, Guid portfolioId, PatchPortfolioRequest request);
    Task<ServiceResult<PaginatedResponse<BOPortfolioResponse>>> BOGetAllAsync(PaginationRequest paginationRequest);
}