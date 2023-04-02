namespace Genbox.SimpleS3.Core.Network.Requests.Interfaces;

public interface IHasPartNumber
{
    /// <summary>Part number of the object part being read. This is a positive integer between 1 and the maximum number of
    /// parts supported. Only objects uploaded using the multipart upload API have part numbers. For information about
    /// multipart uploads, see Multipart Upload Overview in the Amazon Simple Storage Service Developer Guide.</summary>
    int? PartNumber { get; set; }
}