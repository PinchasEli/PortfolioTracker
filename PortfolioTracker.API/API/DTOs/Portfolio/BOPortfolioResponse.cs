namespace API.DTOs.Portfolio;

public class BOPortfolioResponse : PortfolioResponse
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
}
