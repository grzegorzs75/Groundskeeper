namespace Groundskeeper.Models;

using Groundskeeper.Models;

public class Site
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Address { get; set; }

    public int CustomerId { get; set; }

    public Customer Customer { get; set; } = null!;

    public ICollection<Asset> Assets { get; set; } = new List<Asset>();
}