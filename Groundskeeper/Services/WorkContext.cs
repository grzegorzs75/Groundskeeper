namespace Groundskeeper.Services;

public class WorkContext
{
    public int? CustomerId { get; set; }
    public int? SiteId { get; set; }
    public int? AssetId { get; set; }

    public void Clear()
    {
        CustomerId = null;
        SiteId = null;
        AssetId = null;
    }
}