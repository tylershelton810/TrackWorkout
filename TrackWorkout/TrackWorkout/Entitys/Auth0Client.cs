using System;
using System.Threading;
using System.Threading.Tasks;
using Auth0.OidcClient;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using IdentityModel.OidcClient.Results;

namespace TrackWorkout.Entitys
{
    public class Auth0Client : IAuth0Client
    {
        public Auth0Client(Auth0ClientOptions auth0ClientOptions)
        {
        }

        public Task<UserInfoResult> GetUserInfoAsync(string accessToken)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResult> LoginAsync(object extraParameters = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<BrowserResultType> LogoutAsync(bool federated = false, object extraParameters = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<AuthorizeState> PrepareLoginAsync(object extraParameters = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<LoginResult> ProcessResponseAsync(string data, AuthorizeState state, object extraParameters = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken, object extraParameters = null, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
