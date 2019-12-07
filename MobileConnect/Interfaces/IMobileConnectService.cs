using System.Threading.Tasks;
using MobileConnect.Processors.SiAuthorize;

namespace MobileConnect.Interfaces
{
    public interface IMobileConnectService
    {
        Task<MobileConnectSiAuthorizeResult> SiAuthorize(MobileConnectSiAuthorizeSettings settings);
    }
}
