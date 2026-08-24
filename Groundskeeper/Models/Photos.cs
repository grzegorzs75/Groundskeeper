namespace Groundskeeper.Models;

public class Photo
{
    public int Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string FilePath { get; set; } = string.Empty;

    public DateTime UploadedAt { get; set; } = DateTime.Now;

    public int? ServiceRecordId { get; set; }
    public ServiceRecord? ServiceRecord { get; set; }

    public int? IssueId { get; set; }
    public Issue? Issue { get; set; }

    public string? Caption { get; set; }
}