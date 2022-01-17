using System.Text;
using Genbox.SimpleS3.Core.Abstracts.Request;

namespace Genbox.SimpleS3.Core.Abstracts.Provider
{
    public interface IUrlBuilder
    {
        void AppendHost<TReq>(StringBuilder sb, TReq request) where TReq : IRequest;
        void AppendUrl<TReq>(StringBuilder sb, TReq request) where TReq : IRequest;
    }
}