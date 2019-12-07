using System.Threading.Tasks;
using MobileConnect.Processors.SiAuthorize;

namespace MobileConnect.Interfaces.Services
{
    public interface IMobileConnectService
    {
        Task<MobileConnectSiAuthorizeResult> SiAuthorize(MobileConnectSiAuthorizeSettings settings);
    }
}
