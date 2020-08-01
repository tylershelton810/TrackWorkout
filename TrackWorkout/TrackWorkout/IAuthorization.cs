using System;
using IdentityModel.OidcClient.Browser;

namespace TrackWorkout
{
    public interface IAuthorization
    {
        void Init();
        System.Threading.Tasks.Task<IdentityModel.OidcClient.LoginResult> Login();
        System.Threading.Tasks.Task<BrowserResultType> Logout();
    }
}
