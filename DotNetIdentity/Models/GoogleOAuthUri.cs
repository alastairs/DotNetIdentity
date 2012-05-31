using System;
using System.Configuration;
using System.Web;

namespace DotNetIdentity.Models
{
    public static class GoogleOAuthUri
    {
        const string GoogleUriFormat = "https://accounts.google.com/o/oauth2/auth?"
            + "scope={0}&state={1}&redirect_uri={2}&client_id={3}&response_type=code";

        const string scope = "https://www.googleapis.com/auth/userinfo.profile";

        public static string MakeUri(string redirectUri, string state)
        {
            var clientId = ConfigurationManager.AppSettings["google_client_id"];
            return (String.Format(GoogleUriFormat,
              HttpUtility.UrlEncode(scope),
              HttpUtility.UrlEncode(state),
              HttpUtility.UrlEncode(redirectUri),
              HttpUtility.UrlEncode(clientId)
            ));
        }
    }
}