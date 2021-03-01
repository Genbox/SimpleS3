using Genbox.SimpleS3.Core.Abstracts;

namespace Genbox.SimpleS3.Core.Enums
{
    /// <summary>Storage classes used by Amazon. See https://aws.amazon.com/s3/storage-classes/ for more information about each class.</summary>
    public enum StorageClass
    {
        Unknown = 0,

        /// <summary>
        /// S3 Standard offers high durability, availability, and performance object storage for frequently accessed data. Because it delivers low latency and
        /// high throughput, S3 Standard is appropriate for a wide variety of use cases, including cloud applications, dynamic websites, content distribution,
        /// mobile and gaming applications, and big data analytics.
        /// <list type="bullet">
        ///     <item><term>Designed for durability of 99.999999999% of objects across multiple Availability Zones</term></item>
        ///     <item><term>Resilient against events that impact an entire Availability Zone</term></item>
        ///     <item><term>Designed for 99.99% availability over a given year</term></item>
        ///     <item><term>Backed with the Amazon S3 Service Level Agreement for availability</term></item>
        ///     <item><term>Supports SSL for data in transit and encryption of data at rest</term></item>
        ///     <item><term>S3 Lifecycle management for automatic migration of objects to other S3 Storage Classes</term></item>
        /// </list>
        /// </summary>
        [EnumValue("STANDARD")]
        Standard,

        [EnumValue("REDUCED_REDUNDANCY")]
        ReducedRedundancy,

        /// <summary>
        /// S3 Glacier is a secure, durable, and low-cost storage class for data archiving. You can reliably store any amount of data at costs that are
        /// competitive with or cheaper than on-premises solutions. To keep costs low yet suitable for varying needs, S3 Glacier provides three retrieval options
        /// that range from a few minutes to hours.
        /// <item><term>Designed for durability of 99.999999999% of objects across multiple Availability Zones</term></item>
        /// <list type="bullet">
        ///     <item><term>Data is resilient in the event of one entire Availability Zone destruction</term></item>
        ///     <item><term>Supports SSL for data in transit and encryption of data at rest</term></item>
        ///     <item><term>Low-cost design is ideal for long-term archive</term></item>
        ///     <item><term>Configurable retrieval times, from minutes to hours</term></item>
        ///     <item><term>S3 PUT API for direct uploads to S3 Glacier, and S3 Lifecycle management for automatic migration of objects</term></item>
        /// </list>
        /// </summary>
        [EnumValue("GLACIER")]
        Glacier,

        /// <summary>
        /// S3 Standard-IA is for data that is accessed less frequently, but requires rapid access when needed. S3 Standard-IA offers the high durability, high
        /// throughput, and low latency of S3 Standard, with a low per GB storage price and per GB retrieval fee. This combination of low cost and high
        /// performance make S3 Standard-IA ideal for long-term storage, backups, and as a data store for disaster recovery files.
        /// <list type="bullet">
        ///     <item><term>Same low latency and high throughput performance of S3 Standard</term></item>
        ///     <item><term>Designed for durability of 99.999999999% of objects across multiple Availability Zones</term></item>
        ///     <item><term>Resilient against events that impact an entire Availability Zone</term></item>
        ///     <item><term>Data is resilient in the event of one entire Availability Zone destruction</term></item>
        ///     <item><term>Designed for 99.9% availability over a given year</term></item>
        ///     <item><term>Backed with the Amazon S3 Service Level Agreement for availability</term></item>
        ///     <item><term>Supports SSL for data in transit and encryption of data at rest</term></item>
        ///     <item><term>S3 Lifecycle management for automatic migration of objects to other S3 Storage Classes</term></item>
        /// </list>
        /// </summary>
        [EnumValue("STANDARD_IA")]
        StandardIa,

        /// <summary>
        /// S3 One Zone-IA is for data that is accessed less frequently, but requires rapid access when needed. Unlike other S3 Storage Classes which store data
        /// in a minimum of three Availability Zones (AZs), S3 One Zone-IA stores data in a single AZ and costs 20% less than S3 Standard-IA. S3 One Zone-IA is
        /// ideal for customers who want a lower-cost option for infrequently accessed data but do not require the availability and resilience of S3 Standard or
        /// S3 Standard-IA. It’s a good choice for storing secondary backup copies of on-premises data or easily re-creatable data. You can also use it as
        /// cost-effective storage for data that is replicated from another AWS Region using S3 Cross-Region Replication.
        /// <list type="bullet">
        ///     <item><term>Same low latency and high throughput performance of S3 Standard</term></item>
        ///     <item><term>Designed for durability of 99.999999999% of objects in a single Availability Zone</term></item>
        ///     <item><term>Designed for 99.5% availability over a given year</term></item>
        ///     <item><term>Backed with the Amazon S3 Service Level Agreement for availability</term></item>
        ///     <item><term>Supports SSL for data in transit and encryption of data at rest</term></item>
        ///     <item><term>S3 Lifecycle management for automatic migration of objects to other S3 Storage Classes</term></item>
        /// </list>
        /// </summary>
        [EnumValue("ONEZONE_IA")]
        OneZoneIa,

        /// <summary>
        /// The S3 Intelligent-Tiering storage class is designed to optimize costs by automatically moving data to the most cost-effective access tier, without
        /// performance impact or operational overhead. It works by storing objects in two access tiers: one tier that is optimized for frequent access and
        /// another lower-cost tier that is optimized for infrequent access. For a small monthly monitoring and automation fee per object, Amazon S3 monitors
        /// access patterns of the objects in S3 Intelligent-Tiering, and moves the ones that have not been accessed for 30 consecutive days to the infrequent
        /// access tier. If an object in the infrequent access tier is accessed, it is automatically moved back to the frequent access tier. There are no
        /// retrieval fees when using the S3 Intelligent-Tiering storage class, and no additional tiering fees when objects are moved between access tiers. It is
        /// the ideal storage class for long-lived data with access patterns that are unknown or unpredictable.
        /// <list type="bullet">
        ///     <item><term>Same low latency and high throughput performance of S3 Standard</term></item>
        ///     <item><term>Small monthly monitoring and auto-tiering fee</term></item>
        ///     <item><term>Automatically moves objects between two access tiers based on changing access patterns</term></item>
        ///     <item><term>Designed for durability of 99.999999999% of objects across multiple Availability Zones</term></item>
        ///     <item><term>Resilient against events that impact an entire Availability Zone</term></item>
        ///     <item><term>Designed for 99.9% availability over a given year</term></item>
        ///     <item><term>Backed with the Amazon S3 Service Level Agreement for availability</term></item>
        ///     <item><term>Supports SSL for data in transit and encryption of data at rest</term></item>
        ///     <item><term>S3 Lifecycle management for automatic migration of objects to other S3 Storage Classes</term></item>
        /// </list>
        /// </summary>
        [EnumValue("INTELLIGENT_TIERING")]
        IntelligentTiering,

        /// <summary>
        /// S3 Glacier Deep Archive is Amazon S3’s lowest-cost storage class and supports long-term retention and digital preservation for data that may be
        /// accessed once or twice in a year. It is designed for customers — particularly those in highly-regulated industries, such as the Financial Services,
        /// Healthcare, and Public Sectors — that retain data sets for 7-10 years or longer to meet regulatory compliance requirements. S3 Glacier Deep Archive
        /// can also be used for backup and disaster recovery use cases, and is a cost-effective and easy-to-manage alternative to magnetic tape systems, whether
        /// they are on-premises libraries or off-premises services. S3 Glacier Deep Archive complements Amazon S3 Glacier, which is ideal for archives where
        /// data is regularly retrieved and some of the data may be needed in minutes. All objects stored in S3 Glacier Deep Archive are replicated and stored
        /// across at least three geographically-dispersed Availability Zones, protected by 99.999999999% of durability, and can be restored within 12 hours.
        /// <list type="bullet">
        ///     <item><term>Designed for durability of 99.999999999% of objects across multiple Availability Zones</term></item>
        ///     <item><term>Lowest cost storage class designed for long-term retention of data that will be retained for 7-10 years</term></item>
        ///     <item><term>Ideal alternative to magnetic tape libraries</term></item> <item><term>Retrieval time within 12 hours</term></item>
        ///     <item>
        ///         <term>S3 PUT API for direct uploads to S3 Glacier Deep Archive, and S3 Lifecycle management for automatic migration of objects</term>
        ///     </item>
        /// </list>
        /// </summary>
        [EnumValue("DEEP_ARCHIVE")]
        DeepArchive
    }
}