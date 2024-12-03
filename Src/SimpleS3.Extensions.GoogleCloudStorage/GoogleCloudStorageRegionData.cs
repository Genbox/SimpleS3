using Genbox.SimpleS3.Core.Abstracts.Region;

namespace Genbox.SimpleS3.Extensions.GoogleCloudStorage;

public class GoogleCloudStorageRegionData : IRegionData
{
    public static GoogleCloudStorageRegionData Instance { get; } = new GoogleCloudStorageRegionData();

    public IEnumerable<IRegionInfo> GetRegions()
    {
        //North America
        yield return new RegionInfo(GoogleCloudStorageRegion.CanadaNorthEast1, "NORTHAMERICA-NORTHEAST1", "North America (Montréal)");
        yield return new RegionInfo(GoogleCloudStorageRegion.CanadaNorthEast2, "NORTHAMERICA-NORTHEAST2", "North America (Toronto)");
        yield return new RegionInfo(GoogleCloudStorageRegion.UsCentral1, "US-CENTRAL1", "North America (Iowa)");
        yield return new RegionInfo(GoogleCloudStorageRegion.UsEast1, "US-EAST1", "North America (South Carolina)");
        yield return new RegionInfo(GoogleCloudStorageRegion.UsEast4, "US-EAST4", "North America (Northern Virginia)");
        yield return new RegionInfo(GoogleCloudStorageRegion.UsEast5, "US-EAST5", "North America (Columbus)");
        yield return new RegionInfo(GoogleCloudStorageRegion.UsSouth1, "US-SOUTH1", "North America (Dallas)");
        yield return new RegionInfo(GoogleCloudStorageRegion.UsWest1, "US-WEST1", "North America (Oregon)");
        yield return new RegionInfo(GoogleCloudStorageRegion.UsWest2, "US-WEST2", "North America (Los Angeles)");
        yield return new RegionInfo(GoogleCloudStorageRegion.UsWest3, "US-WEST3", "North America (Salt Lake City)");
        yield return new RegionInfo(GoogleCloudStorageRegion.UsWest4, "US-WEST4", "North America (Las Vegas)");

        //South America
        yield return new RegionInfo(GoogleCloudStorageRegion.SouthAmericaEast1, "SOUTHAMERICA-EAST1", "South America (São Paulo)");
        yield return new RegionInfo(GoogleCloudStorageRegion.SouthAmericaEast1, "SOUTHAMERICA-WEST1", "South America (Santiago)");

        //Europe
        yield return new RegionInfo(GoogleCloudStorageRegion.EuropeCentral2, "EUROPE-CENTRAL2", "Europe (Warsaw)");
        yield return new RegionInfo(GoogleCloudStorageRegion.EuropeNorth1, "EUROPE-NORTH1", "Europe (Finland)");
        yield return new RegionInfo(GoogleCloudStorageRegion.EuropeSouthWest1, "EUROPE-SOUTHWEST1", "Europe (Madrid)");
        yield return new RegionInfo(GoogleCloudStorageRegion.EuropeWest1, "EUROPE-WEST1", "Europe (Belgium)");
        yield return new RegionInfo(GoogleCloudStorageRegion.EuropeWest2, "EUROPE-WEST2", "Europe (London)");
        yield return new RegionInfo(GoogleCloudStorageRegion.EuropeWest3, "EUROPE-WEST3", "Europe (Frankfurt)");
        yield return new RegionInfo(GoogleCloudStorageRegion.EuropeWest4, "EUROPE-WEST4", "Europe (Netherlands)");
        yield return new RegionInfo(GoogleCloudStorageRegion.EuropeWest6, "EUROPE-WEST6", "Europe (Zürich)");
        yield return new RegionInfo(GoogleCloudStorageRegion.EuropeWest8, "EUROPE-WEST8", "Europe (Milan)");
        yield return new RegionInfo(GoogleCloudStorageRegion.EuropeWest9, "EUROPE-WEST9", "Europe (Paris)");
        yield return new RegionInfo(GoogleCloudStorageRegion.EuropeWest10, "EUROPE-WEST10", "Europe (Berlin)");
        yield return new RegionInfo(GoogleCloudStorageRegion.EuropeWest12, "EUROPE-WEST12", "Europe (Turin)");

        //Asia
        yield return new RegionInfo(GoogleCloudStorageRegion.AsiaEast1, "ASIA-EAST1", "Asia (Taiwan)");
        yield return new RegionInfo(GoogleCloudStorageRegion.AsiaEast2, "ASIA-EAST2", "Asia (Hong Kong)");
        yield return new RegionInfo(GoogleCloudStorageRegion.AsiaNorthEast1, "ASIA-NORTHEAST1", "Asia (Tokyo)");
        yield return new RegionInfo(GoogleCloudStorageRegion.AsiaNorthEast2, "ASIA-NORTHEAST2", "Asia (Osaka)");
        yield return new RegionInfo(GoogleCloudStorageRegion.AsiaNorthEast3, "ASIA-NORTHEAST3", "Asia (Seoul)");
        yield return new RegionInfo(GoogleCloudStorageRegion.AsiaSouthEast1, "ASIA-SOUTHEAST1", "Asia (Singapore)");

        //India
        yield return new RegionInfo(GoogleCloudStorageRegion.AsiaSouth1, "ASIA-SOUTH1", "Asia (Mumbai)");
        yield return new RegionInfo(GoogleCloudStorageRegion.AsiaSouth2, "ASIA-SOUTH2", "Asia (Delhi)");

        //Indonesia
        yield return new RegionInfo(GoogleCloudStorageRegion.AsiaSouthEast2, "ASIA-SOUTHEAST2", "Asia (Jakarta)");

        //Middle East
        yield return new RegionInfo(GoogleCloudStorageRegion.MiddleEastCentral1, "ME-CENTRAL1", "Middle East (Doha)");
        yield return new RegionInfo(GoogleCloudStorageRegion.MiddleEastCentral2, "ME-CENTRAL2", "Middle East (Dammam, Saudi Arabia)");
        yield return new RegionInfo(GoogleCloudStorageRegion.MiddleEastWest1, "ME-WEST1", "Middle East (Tel Aviv)");

        //Australia
        yield return new RegionInfo(GoogleCloudStorageRegion.AustraliaSouthEast1, "AUSTRALIA-SOUTHEAST1", "Australia (Sydney)");
        yield return new RegionInfo(GoogleCloudStorageRegion.AustraliaSouthEast2, "AUSTRALIA-SOUTHEAST2", "Australia (Melbourne)");

        //Africa
        yield return new RegionInfo(GoogleCloudStorageRegion.AfricaSouth1, "AFRICA-SOUTH1", "Africa (Johannesburg)");
    }
}