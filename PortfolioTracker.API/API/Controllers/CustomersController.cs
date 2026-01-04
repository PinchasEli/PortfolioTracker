using Microsoft.AspNetCore.Mvc;
using API.DTOs.Customers;
using API.DTOs.Common;
using API.Validators;
using Application.Customers;
using Infrastructure.Persistence;

namespace API.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ILogger<CustomersController> _logger;
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateCustomerRequest request)
    {
        var validator = new CreateCustomerRequestValidator();
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var result = await _customerService.CreateAsync(request);

        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpGet]
    public async Task<IActionResult> GetCustomers([FromQuery] PaginationRequest paginationRequest)
    {
        var result = await _customerService.GetAllAsync(paginationRequest);
        if (!result.Success)
            return BadRequest(result.ErrorMessage);

        return Ok(result.Data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomerById(Guid id)
    {
        var result = await _customerService.GetByIdAsync(id);
        if (!result.Success)
            return NotFound(result.ErrorMessage);

        return Ok(result.Data);
    }
}