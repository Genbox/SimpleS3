namespace Genbox.SimpleS3.Core.Enums
{
    public enum RetrievalTier
    {
        Unknown = 0,

        /// <summary>
        /// Expedited retrievals allow you to quickly access your data stored in the <see cref="StorageClass.Glacier" /> storage class when occasional
        /// urgent requests for a subset of archives are required. For all but the largest archived objects (250 MB+), data accessed using Expedited retrievals
        /// are typically made available within 1–5 minutes. Provisioned capacity ensures that retrieval capacity for Expedited retrievals is available when you
        /// need it. Expedited retrievals and provisioned capacity are not available for the <see cref="StorageClass.DeepArchive" /> storage class.
        /// </summary>
        Expedited,

        /// <summary>
        /// Standard retrievals allow you to access any of your archived objects within several hours. This is the default option for the
        /// <see cref="StorageClass.Glacier" /> and <see cref="StorageClass.DeepArchive" /> retrieval requests that do not specify the retrieval option. Standard
        /// retrievals typically complete within 3-5 hours from the <see cref="StorageClass.Glacier" /> storage class and typically complete within 12 hours from
        /// the <see cref="StorageClass.DeepArchive" /> storage class.
        /// </summary>
        Standard,

        /// <summary>
        /// Bulk retrievals are Amazon Glacier’s lowest-cost retrieval option, enabling you to retrieve large amounts, even petabytes, of data
        /// inexpensively in a day. Bulk retrievals typically complete within 5-12 hours from the <see cref="StorageClass.Glacier" /> storage class and typically
        /// complete within 48 hours from the <see cref="StorageClass.DeepArchive" /> storage class.
        /// </summary>
        Bulk
    }
}