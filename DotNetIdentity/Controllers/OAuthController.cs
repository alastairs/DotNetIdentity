using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Newtonsoft.Json;

namespace DotNetIdentity.Controllers
{
    public class OAuthController : Controller
    {
        //
        // GET: /OAuth/Callback

        public ActionResult Callback(string code, string state)
        {
            var json = ConvertAuthCodeToJson(code);

            var data = JsonConvert.DeserializeObject<dynamic>(json);
            var accessToken = data.access_token;
            var userInfoUrl = string.Format("https://www.googleapis.com/oauth2/v1/userinfo?access_token={0}",
                                            accessToken);

            var userInfoJson = new WebClient().DownloadString(userInfoUrl);

            var userInfo = JsonConvert.DeserializeObject<dynamic>(userInfoJson);
            var userName = (string) userInfo.name;
            FormsAuthentication.SetAuthCookie(userName, false);
            return RedirectToAction("Index", "Home");
        }

        private string ConvertAuthCodeToJson(string code)
        {
            var postData = new Dictionary<string, string>
                               {
                                   {"code", code},
                                   {"client_id", ConfigurationManager.AppSettings["google_client_id"]},
                                   {"client_secret", ConfigurationManager.AppSettings["google_client_secret"]},
                                   {"redirect_uri", "http://localhost:12345/oauth/callback"},
                                   {"grant_type", "authorization_code"}
                               };

            return GetHttpPostResponse("https://accounts.google.com/o/oauth2/token", postData);
        }

        private string GetHttpPostResponse(string url, Dictionary<string, string> postData)
        {
            var postString = String.Join("&", postData.Select(pair => HttpUtility.UrlEncode(pair.Key) + "=" + HttpUtility.UrlEncode(pair.Value)).ToArray());

            var postBytes = Encoding.ASCII.GetBytes(postString);
            var request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = postBytes.Length;

            var postStream = request.GetRequestStream();
            postStream.Write(postBytes, 0, postBytes.Length);
            postStream.Close();

            var response = request.GetResponse() as HttpWebResponse;
            var json = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return json;
        }
    }
}
