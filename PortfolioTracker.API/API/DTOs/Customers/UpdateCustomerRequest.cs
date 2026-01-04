namespace API.DTOs.Customers;

/// <summary>
/// DTO for full customer update (PUT)
/// All fields are required
/// </summary>
public class UpdateCustomerRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Active { get; set; }
}

