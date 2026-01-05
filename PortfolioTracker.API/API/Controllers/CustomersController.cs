using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using API.DTOs.Customers;
using API.DTOs.Common;
using API.Validators;
using Application.Customers;
using Infrastructure.Authorization;

namespace API.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize] // require authentication for all endpoints in this controller
public class CustomersController : BaseApiController
{
    private readonly ILogger<CustomersController> _logger;
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService, ILogger<CustomersController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    /// <summary>
    /// Create a new customer - Requires Admin or SuperAdmin role
    /// </summary>
    [HttpPost]
    [Authorize(Policy = RolePolicies.RequireAdmin)]
    public async Task<IActionResult> Create(CreateCustomerRequest request)
    {
        var validator = new CreateCustomerRequestValidator();
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var result = await _customerService.CreateAsync(request);
        return HandleResult(result);
    }

    /// <summary>
    /// Get all customers - Requires at least Customer role
    /// </summary>
    [HttpGet]
    [Authorize(Policy = RolePolicies.RequireAdmin)]
    public async Task<IActionResult> GetCustomers([FromQuery] PaginationRequest paginationRequest)
    {
        var result = await _customerService.GetAllAsync(paginationRequest);
        return HandleResult(result);
    }

    /// <summary>
    /// Get customer by ID - Requires at least Customer role
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Policy = RolePolicies.RequireCustomer)]
    public async Task<IActionResult> GetCustomerById(Guid id)
    {
        var result = await _customerService.GetByIdAsync(id);
        return HandleResult(result);
    }

    /// <summary>
    /// Update customer - Requires Admin or SuperAdmin role
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Policy = RolePolicies.RequireAdmin)]
    public async Task<IActionResult> Update(Guid id, UpdateCustomerRequest request)
    {
        var validator = new UpdateCustomerRequestValidator();
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var result = await _customerService.UpdateAsync(id, request);
        return HandleResult(result);
    }

    /// <summary>
    /// Partially update customer - Requires Admin or SuperAdmin role
    /// </summary>
    [HttpPatch("{id}")]
    [Authorize(Policy = RolePolicies.RequireAdmin)]
    public async Task<IActionResult> Patch(Guid id, PatchCustomerRequest request)
    {
        var validator = new PatchCustomerRequestValidator();
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var result = await _customerService.PatchAsync(id, request);
        return HandleResult(result);
    }
}