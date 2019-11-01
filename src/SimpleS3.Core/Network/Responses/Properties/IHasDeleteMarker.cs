namespace Genbox.SimpleS3.Core.Network.Responses.Properties
{
    public interface IHasDeleteMarker
    {
        /// <summary>
        /// Specifies whether the versioned object that was permanently deleted was (true) or was not (false) a delete marker. In a simple DELETE, this
        /// header indicates whether (true) or not (false) a delete marker was created.
        /// </summary>
        bool IsDeleteMarker { get; }
    }
}