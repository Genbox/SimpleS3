namespace Genbox.SimpleS3.Core.Network.Responses.Buckets;

/// <summary>Empty response from creating a metadata configuration.</summary>
public sealed class CreateBucketMetadataConfigurationResponse : BaseResponse {}

/// <summary>Empty response from deleting a metadata configuration.</summary>
public sealed class DeleteBucketMetadataConfigurationResponse : BaseResponse {}

/// <summary>Empty response from updating an inventory table configuration.</summary>
public sealed class UpdateBucketMetadataInventoryTableConfigurationResponse : BaseResponse {}

/// <summary>Empty response from updating a journal table configuration.</summary>
public sealed class UpdateBucketMetadataJournalTableConfigurationResponse : BaseResponse {}

/// <summary>Response containing an S3 Metadata V2 configuration result.</summary>
public sealed class GetBucketMetadataConfigurationResponse : BaseResponse
{
    /// <summary>Metadata configuration result.</summary>
    public S3MetadataConfigurationResult Configuration { get; internal set; } = null!;
}

/// <summary>S3 Metadata V2 configuration result.</summary>
public sealed class S3MetadataConfigurationResult
{
    /// <summary>Destination result.</summary>
    public S3MetadataDestinationResult Destination { get; internal set; } = null!;

    /// <summary>Journal table result.</summary>
    public S3MetadataJournalTableResult? JournalTable { get; internal set; }

    /// <summary>Inventory table result.</summary>
    public S3MetadataInventoryTableResult? InventoryTable { get; internal set; }

    /// <summary>Annotation table result.</summary>
    public S3MetadataAnnotationTableResult? AnnotationTable { get; internal set; }
}

/// <summary>S3 Metadata destination result.</summary>
public sealed class S3MetadataDestinationResult
{
    /// <summary>ARN of the table bucket.</summary>
    public string? TableBucketArn { get; internal set; }

    /// <summary>Type of the table bucket.</summary>
    public string? TableBucketType { get; internal set; }

    /// <summary>Namespace containing the metadata tables.</summary>
    public string? TableNamespace { get; internal set; }
}

/// <summary>S3 Metadata operation error.</summary>
public sealed class S3MetadataError
{
    /// <summary>Service error code.</summary>
    public string? ErrorCode { get; internal set; }

    /// <summary>Service error message.</summary>
    public string? ErrorMessage { get; internal set; }
}

/// <summary>Metadata journal table result.</summary>
public sealed class S3MetadataJournalTableResult
{
    /// <summary>Error reported while configuring the table.</summary>
    public S3MetadataError? Error { get; internal set; }

    /// <summary>Number of days journal records are retained.</summary>
    public int? ExpirationDays { get; internal set; }

    /// <summary>Record expiration state.</summary>
    public string? Expiration { get; internal set; }

    /// <summary>ARN of the journal table.</summary>
    public string? TableArn { get; internal set; }

    /// <summary>Name of the journal table.</summary>
    public string? TableName { get; internal set; }

    /// <summary>Status of the journal table.</summary>
    public string? TableStatus { get; internal set; }
}

/// <summary>Metadata inventory table result.</summary>
public sealed class S3MetadataInventoryTableResult
{
    /// <summary>Inventory table configuration state.</summary>
    public string? ConfigurationState { get; internal set; }

    /// <summary>Error reported while configuring the table.</summary>
    public S3MetadataError? Error { get; internal set; }

    /// <summary>ARN of the inventory table.</summary>
    public string? TableArn { get; internal set; }

    /// <summary>Name of the inventory table.</summary>
    public string? TableName { get; internal set; }

    /// <summary>Status of the inventory table.</summary>
    public string? TableStatus { get; internal set; }
}

/// <summary>Metadata annotation table result.</summary>
public sealed class S3MetadataAnnotationTableResult
{
    /// <summary>Annotation table configuration state.</summary>
    public string? ConfigurationState { get; internal set; }

    /// <summary>Error reported while configuring the table.</summary>
    public S3MetadataError? Error { get; internal set; }

    /// <summary>IAM role used by the annotation table.</summary>
    public string? Role { get; internal set; }

    /// <summary>ARN of the annotation table.</summary>
    public string? TableArn { get; internal set; }

    /// <summary>Name of the annotation table.</summary>
    public string? TableName { get; internal set; }

    /// <summary>Status of the annotation table.</summary>
    public string? TableStatus { get; internal set; }
}