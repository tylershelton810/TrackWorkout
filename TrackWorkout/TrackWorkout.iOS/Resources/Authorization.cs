using System;
using Auth0.OidcClient;
using IdentityModel.OidcClient.Browser;

namespace TrackWorkout.iOS
{
    public class Authorization : IAuthorization
    {
        public Auth0Client client;
        public IdentityModel.OidcClient.LoginResult loginResult;

        public Authorization()
        {
        }

        public void Init()
        {
        }
        public async System.Threading.Tasks.Task<IdentityModel.OidcClient.LoginResult> Login()
        {
            client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = "dev--28cyisb.auth0.com",
                ClientId = "4qhP08l7Vub3qrBCQhKu7b0mooKCSmRi"
            });

            loginResult = await client.LoginAsync();


            return loginResult;

            //App.StoreLoginToken(loginResult);
        }
        public async System.Threading.Tasks.Task<BrowserResultType> Logout()
        {
            BrowserResultType browserResult = await client.LogoutAsync();

            return browserResult;
        }
    }
}
