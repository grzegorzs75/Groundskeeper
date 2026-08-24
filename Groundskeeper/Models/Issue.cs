namespace Groundskeeper.Models;

public class Issue
{
    public int Id { get; set; }

    public int AssetId { get; set; }

    public Asset Asset { get; set; } = null!;

    public int? ServiceRecordId { get; set; }

    public ServiceRecord? ServiceRecord { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime ReportedDate { get; set; } = DateTime.Today;

    public IssuePriority Priority { get; set; } = IssuePriority.Normal;

    public IssueStatus Status { get; set; } = IssueStatus.Open;

    public string? ResolutionNotes { get; set; }

    public DateTime? ResolvedDate { get; set; }

    public ICollection<Photo> Photos { get; set; } = new List<Photo>();
}

public enum IssuePriority
{
    Low,
    Normal,
    High,
    Critical
}

public enum IssueStatus
{
    Open,
    InProgress,
    AwaitingApproval,
    AwaitingParts,
    Resolved,
    Closed
}