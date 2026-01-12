namespace API.DTOs.Customers;

/// <summary>
/// DTO for partial customer update (PATCH)
/// All fields are optional - only provided fields will be updated
/// </summary>
public class PatchCustomerRequest
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public bool? Active { get; set; }
}
