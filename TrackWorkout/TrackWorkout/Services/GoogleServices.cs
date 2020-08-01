using System.Net.Http;
using System.Threading.Tasks;
using TrackWorkout.Entitys;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TrackWorkout.Services
{
    /// <summary>
    /// Doc: https://developers.google.com/identity/protocols/OAuth2InstalledApp
    /// </summary>
    public class GoogleServices
    {

        /// <summary>
        /// Create a new app and get new creadentials: 
        /// https://console.developers.google.com/apis/
        /// </summary>
        public static string AppName = "TrackWorkout.Services";
        public static readonly string AuthorizeUrl = "https://accounts.google.com/o/oauth2/auth";
        public static readonly string AccessTokenUrl = "https://oauth2.googleapis.com/token";

        // OAuth
        // For Google login, configure at https://console.developers.google.com/
        public static string iOSClientId = "<insert IOS client ID here>";
        public static string AndroidClientId = "577834421371-1kiub8ohq9p27d1rr4d6mqd1pbo1f27a.apps.googleusercontent.com";

        // These values do not need changing
        public static string Scope = "https://www.googleapis.com/auth/userinfo.email";
        public static string UserInfoUrl = "https://www.googleapis.com/oauth2/v2/userinfo";

        // Set these to reversed iOS/Android client ids, with :/oauth2redirect appended
        public static string iOSRedirectUrl = "<insert IOS redirect URL here>:/oauth2redirect";
        public static string AndroidRedirectUrl = "com.googleusercontent.apps.577834421371-1kiub8ohq9p27d1rr4d6mqd1pbo1f27a:/oauth2redirect";        

        public async Task<string> GetAccessTokenAsync(string code)
        {
            var requestUrl =
                AccessTokenUrl
                + "?code=" + code
                + "&client_id=" + AndroidClientId
                + "&redirect_uri=" + AndroidRedirectUrl
                + "&grant_type=authorization_code";

            var httpClient = new HttpClient();

            var response = await httpClient.PostAsync(requestUrl, null);

            var json = await response.Content.ReadAsStringAsync();

            var accessToken = JsonConvert.DeserializeObject<JObject>(json).Value<string>("access_token");

            return accessToken;
        }

        public async Task<GoogleProfile> GetGoogleUserProfileAsync(string accessToken)
        {

            var requestUrl = "https://www.googleapis.com/plus/v1/people/me"
                             + "?access_token=" + accessToken;

            var httpClient = new HttpClient();

            var userJson = await httpClient.GetStringAsync(requestUrl);

            var googleProfile = JsonConvert.DeserializeObject<GoogleProfile>(userJson);

            return googleProfile;
        }
    }
}
