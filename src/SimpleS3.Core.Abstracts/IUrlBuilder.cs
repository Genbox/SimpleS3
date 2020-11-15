using System.Text;

namespace Genbox.SimpleS3.Core.Abstracts
{
    public interface IUrlBuilder
    {
        void AppendHost<TReq>(StringBuilder sb, TReq request) where TReq : IRequest;
        void AppendUrl<TReq>(StringBuilder sb, TReq request) where TReq : IRequest;
    }
}