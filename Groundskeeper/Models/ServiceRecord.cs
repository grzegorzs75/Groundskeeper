namespace Groundskeeper.Models;

public class ServiceRecord
{
    public int Id { get; set; }

    // Asset that was serviced
    public int AssetId { get; set; }

    public Asset Asset { get; set; } = null!;

    // When the work was performed
    public DateTime ServiceDate { get; set; } = DateTime.Today;

    // Short description shown in lists
    public string Title { get; set; } = string.Empty;

    // Detailed description of work performed
    public string? Description { get; set; }

    // Optional internal/customer notes
    public string? Notes { get; set; }

    public ICollection<Issue> Issues { get; set; } = new List<Issue>();

    public ICollection<Photo> Photos { get; set; } = new List<Photo>();
}