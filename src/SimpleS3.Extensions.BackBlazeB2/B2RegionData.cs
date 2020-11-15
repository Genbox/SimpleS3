using System.Collections.Generic;
using Genbox.SimpleS3.Core.Abstracts.Region;

namespace Genbox.SimpleS3.Extensions.BackBlazeB2
{
    public class B2RegionData : IRegionData
    {
        public IEnumerable<IRegionInfo> GetRegions()
        {
            yield return new RegionInfo(B2Region.UsWest001, "us-west-001", "US West 1");
            yield return new RegionInfo(B2Region.UsWest002, "us-west-002", "US West 2");
        }
    }
}