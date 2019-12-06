using System.Threading.Tasks;

namespace MobileConnectDemo.Services.MobileConnect.Interfaces
{
    public interface IMobileConnectService
    {
        Task<MobileConnectAuthorizeResult> SiAuthorize(MobileConnectAuthorizeSettings settings);
    }
}
