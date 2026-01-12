using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.DTOs.Portfolio;
using Application.Portfolios;
using Infrastructure.Authorization;

namespace API.Controllers;

[ApiController]
[Route("bo/api/portfolios")]
[Authorize(Policy = RolePolicies.RequireAdmin)] // require authentication for all endpoints in this controller
public class BOPortfolioController : BaseApiController
{
    private readonly ILogger<BOPortfolioController> _logger;
    private readonly IPortfolioService _portfolioService;

    public BOPortfolioController(IPortfolioService portfolioService, ILogger<BOPortfolioController> logger)
    {
        _portfolioService = portfolioService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] BOPortfolioQueryRequest queryRequest)
    {
        var result = await _portfolioService.BOGetAllAsync(queryRequest);
        return HandleResult(result);
    }
}
