using Genbox.SimpleS3.Core.Abstracts.Region;

namespace Genbox.SimpleS3.Extensions.GoogleCloudStorage;

public class GoogleCloudStorageRegionData : IRegionData
{
    public static GoogleCloudStorageRegionData Instance { get; } = new GoogleCloudStorageRegionData();

    public IEnumerable<IRegionInfo> GetRegions()
    {
        yield return new RegionInfo(GoogleCloudStorageRegion.CanadaNorthEast1, "NORTHAMERICA-NORTHEAST1", "North America (Montréal)");
        yield return new RegionInfo(GoogleCloudStorageRegion.CanadaNorthEast2, "NORTHAMERICA-NORTHEAST2", "North America (Toronto)");
        yield return new RegionInfo(GoogleCloudStorageRegion.UsCentral1, "US-CENTRAL1", "North America (Iowa)");
        yield return new RegionInfo(GoogleCloudStorageRegion.UsEast1, "US-EAST1", "East US (South Carolina)");
        yield return new RegionInfo(GoogleCloudStorageRegion.UsEast4, "US-EAST4", "East US (Northern Virginia)");
        yield return new RegionInfo(GoogleCloudStorageRegion.UsWest1, "US-WEST1", "West US (Oregon)");
        yield return new RegionInfo(GoogleCloudStorageRegion.UsWest2, "US-WEST2", "West US (Los Angeles)");
        yield return new RegionInfo(GoogleCloudStorageRegion.UsWest3, "US-WEST3", "West US (Salt Lake City)");
        yield return new RegionInfo(GoogleCloudStorageRegion.UsWest4, "US-WEST4", "West US (Las Vegas)");
        yield return new RegionInfo(GoogleCloudStorageRegion.SouthAmericaEast1, "SOUTHAMERICA-EAST1", "South America (São Paulo)");
        yield return new RegionInfo(GoogleCloudStorageRegion.EuropeCentral2, "EUROPE-CENTRAL2", "Central Europe (Warsaw)");
        yield return new RegionInfo(GoogleCloudStorageRegion.EuropeNorth1, "EUROPE-NORTH1", "North Europe (Finland)");
        yield return new RegionInfo(GoogleCloudStorageRegion.EuropeWest1, "EUROPE-WEST1", "West Europe (Belgium)");
        yield return new RegionInfo(GoogleCloudStorageRegion.EuropeWest2, "EUROPE-WEST2", "West Europe (London)");
        yield return new RegionInfo(GoogleCloudStorageRegion.EuropeWest3, "EUROPE-WEST3", "West Europe (Frankfurt)");
        yield return new RegionInfo(GoogleCloudStorageRegion.EuropeWest4, "EUROPE-WEST4", "West Europe (Netherlands)");
        yield return new RegionInfo(GoogleCloudStorageRegion.EuropeWest6, "EUROPE-WEST6", "West Europe (Zürich)");
        yield return new RegionInfo(GoogleCloudStorageRegion.AsiaEast1, "ASIA-EAST1", "East Asia (Taiwan)");
        yield return new RegionInfo(GoogleCloudStorageRegion.AsiaEast2, "ASIA-EAST2", "East Asia (Hong Kong)");
        yield return new RegionInfo(GoogleCloudStorageRegion.AsiaNorthEast1, "ASIA-NORTHEAST1", "Northeast Asia (Tokyo)");
        yield return new RegionInfo(GoogleCloudStorageRegion.AsiaNorthEast2, "ASIA-NORTHEAST2", "Northeast Asia (Osaka)");
        yield return new RegionInfo(GoogleCloudStorageRegion.AsiaNorthEast3, "ASIA-NORTHEAST3", "Northeast Asia (Seoul)");
        yield return new RegionInfo(GoogleCloudStorageRegion.AsiaSouth1, "ASIA-SOUTH1", "South Asia (Mumbai)");
        yield return new RegionInfo(GoogleCloudStorageRegion.AsiaSouth2, "ASIA-SOUTH2", "South Asia (Delhi)");
        yield return new RegionInfo(GoogleCloudStorageRegion.AsiaSouthEast1, "ASIA-SOUTHEAST1", "Southeast Asia (Singapore)");
        yield return new RegionInfo(GoogleCloudStorageRegion.AsiaSouthEast2, "ASIA-SOUTHEAST2", "Southeast Asia (Jakarta)");
        yield return new RegionInfo(GoogleCloudStorageRegion.AustraliaSouthEast1, "AUSTRALIA-SOUTHEAST1", "Australia (Sydney)");
        yield return new RegionInfo(GoogleCloudStorageRegion.AustraliaSouthEast2, "AUSTRALIA-SOUTHEAST2", "Australia (Melbourne)");
    }
}