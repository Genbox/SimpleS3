using Genbox.SimpleS3.Core.Enums;
using JetBrains.Annotations;

namespace Genbox.SimpleS3.Core.Network.Responses.Interfaces;

[PublicAPI]
public interface IHasReplicationStatus
{
    /// <summary>
    /// Amazon S3 can return this header if your request involves a bucket that is either a source or destination in a cross-region replication. In
    /// cross-region replication you have a source bucket on which you configure replication and destination bucket where Amazon S3 stores object replicas.
    /// When you request an object (GET Object) or object metadata (HEAD Object) from these buckets, Amazon S3 will return the x-amz-replication-status
    /// header in the response as follow: If requesting object from the source bucket — Amazon S3 will return the x-amz-replication-status header if object
    /// in your request is eligible for replication. For example, suppose in your replication configuration you specify object prefix "TaxDocs" requesting
    /// Amazon S3 to replicate objects with key prefix "TaxDocs". Then any objects you upload with this key name prefix, for example "TaxDocs/document1.pdf",
    /// is eligible for replication.For any object request with this key name prefix Amazon S3 will return the x-amz-replication-status header with value
    /// PENDING, COMPLETED or FAILED indicating object replication status. If requesting object from the destination bucket — Amazon S3 will return the
    /// x-amz-replication-status header with value REPLICA if object in your request is a replica that Amazon S3 created.
    /// </summary>
    ReplicationStatus ReplicationStatus { get; }
}