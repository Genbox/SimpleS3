using Genbox.SimpleS3.Cli.Core.Enums;

namespace Genbox.SimpleS3.Cli.Core.Results;

public struct DeleteResult
{
    public DeleteResult(ActionType actionType, OperationStatus operationStatus, string objectKey)
    {
        ActionType = actionType;
        OperationStatus = operationStatus;
        ObjectKey = objectKey;
    }

    public ActionType ActionType { get; }
    public OperationStatus OperationStatus { get; internal set; }
    public string ObjectKey { get; }
}