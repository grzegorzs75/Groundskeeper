namespace Groundskeeper.Models;

public class Asset
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int SiteId { get; set; }

    public Site Site { get; set; } = null!;

    public ICollection<ServiceRecord> ServiceRecords { get; set; }
    = new List<ServiceRecord>();

    public ICollection<Issue> Issues { get; set; } = new List<Issue>();
}