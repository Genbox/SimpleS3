using Genbox.SimpleS3.Core.Abstracts.Region;

namespace Genbox.SimpleS3.Extensions.BackBlazeB2;

/// <summary>Provides the <c>BackblazeB2RegionData</c> API.</summary>
public class BackblazeB2RegionData : IRegionData
{
    /// <summary>Provides the <c>Instance</c> API.</summary>
    public static BackblazeB2RegionData Instance { get; } = new BackblazeB2RegionData();

    /// <summary>Provides the <c>GetRegions</c> API.</summary>
    public IEnumerable<IRegionInfo> GetRegions()
    {
        yield return new RegionInfo(BackBlazeB2Region.UsWest000, "us-west-000", "US West 0");
        yield return new RegionInfo(BackBlazeB2Region.UsWest001, "us-west-001", "US West 1");
        yield return new RegionInfo(BackBlazeB2Region.UsWest002, "us-west-002", "US West 2");
        yield return new RegionInfo(BackBlazeB2Region.EuCentral003, "eu-central-003", "EU Central 3");
        yield return new RegionInfo(BackBlazeB2Region.UsWest004, "us-west-004", "US West 4");
        yield return new RegionInfo(BackBlazeB2Region.UsEast005, "us-east-005", "US East 5");
    }
}