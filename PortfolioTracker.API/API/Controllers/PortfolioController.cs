using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.DTOs.Portfolio;
using API.Validators;
using Application.Portfolios;
using Infrastructure.Authorization;
using API.DTOs.Common;


namespace API.Controllers;


[ApiController]
[Route("api/customers/{customerId}/portfolios")]
[Authorize] // require authentication for all endpoints in this controller
public class PortfolioController : BaseApiController
{
    private readonly ILogger<PortfolioController> _logger;
    private readonly IPortfolioService _portfolioService;

    public PortfolioController(IPortfolioService portfolioService, ILogger<PortfolioController> logger)
    {
        _portfolioService = portfolioService;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(Policy = RolePolicies.RequireAdmin)]
    public async Task<IActionResult> Create(Guid customerId, CreatePortfolioRequest request)
    {
        var validator = new CreatePortfolioRequestValidator();
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var result = await _portfolioService.CreateAsync(customerId, request);
        return HandleResult(result);
    }

    [HttpGet]
    [Authorize(Policy = RolePolicies.RequireCustomer)]
    public async Task<IActionResult> GetAll(Guid customerId, [FromQuery] PaginationRequest paginationRequest)
    {
        var result = await _portfolioService.GetAllAsync(customerId, paginationRequest);
        return HandleResult(result);
    }

    [HttpGet("{portfolioId}")]
    [Authorize(Policy = RolePolicies.RequireCustomer)]
    public async Task<IActionResult> GetById(Guid customerId, Guid portfolioId)
    {
        var result = await _portfolioService.GetByIdAsync(customerId, portfolioId);
        return HandleResult(result);
    }

    [HttpPatch("{portfolioId}")]
    [Authorize(Policy = RolePolicies.RequireCustomer)]
    public async Task<IActionResult> Patch(Guid customerId, Guid portfolioId, PatchPortfolioRequest request)
    {
        var validator = new PatchPortfolioRequestValidator();
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var result = await _portfolioService.PatchAsync(customerId, portfolioId, request);
        return HandleResult(result);
    }
}