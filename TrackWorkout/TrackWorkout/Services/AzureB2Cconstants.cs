using System;
namespace TrackWorkout.Services
{
    public class AzureB2Cconstants
    {
        public const string Authority = "https://coachMEADB2C.onmicrosoft.com/api/coachMEScope";
        public const string ClientId = "be7168c4-3c92-481a-91e9-b24c0ab112a0";
        public static readonly string[] Scopes = { ClientID };
        public const string PolicyReset = "B2C_1_PasswordReset";
        public const string PolicySignUpSignIn = "B2C_1_SignUpSignIn";
    }
}
