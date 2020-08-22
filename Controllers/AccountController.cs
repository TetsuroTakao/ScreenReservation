using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SampleApp.Controllers
{
    public class AccountController : Controller
    {
        string clientId { get; set; }
        string redirectUri { get; set; }
        string endpoint { get; set; }
        string profileScope { get; set; }
        public AccountController()
        {
            var key = string.Empty;
            switch (Environment.GetEnvironmentVariable("TargetBrand"))
            {
                case "microsoft":
                    key = "MicrosoftIdentity";
                    break;
                case "facebook":
                    key = "FacebookIdentity";
                    break;
                case "twitter":
                    key = "TwitterIdentity";
                    break;
                default:
                    key = "MicrosoftIdentity";
                    break;
            }
            endpoint = Environment.GetEnvironmentVariable(key + "_Endpoint");
            clientId = Environment.GetEnvironmentVariable(key + "_ClientId");
            profileScope = Environment.GetEnvironmentVariable(key + "_ProfileScope");

            redirectUri = Environment.GetEnvironmentVariable("RedirectLocal");
        }
        public IActionResult Index()
        {
            return View();
        }
        public void SignIn(string brand,string brands)
        {
            var param = "?client_id=" + clientId + "&redirect_uri=" + redirectUri + "&grant_type=implicit&response_type=code&scope=" + profileScope;
            Environment.SetEnvironmentVariable("TargetBrand", brand);
            Environment.SetEnvironmentVariable("Brands", brands);
            switch (brand) 
            {
                case "microsoft":
                    var seed = Environment.GetEnvironmentVariable("ChallengeSeed");
                    //challenge length is 43-128 characters
                    var challengeLength = int.Parse(Environment.GetEnvironmentVariable("ChallengeLength"));
                    param += "&code_challenge=" + new Encryptor().CreateChallengeText(seed,challengeLength);
                    //if this parameter is omitted, the endpoint will recognize the code_challenge parameter is text format.
                    //param += "&code_challenge_method=S256";
                    Response.Redirect(endpoint + "authorize" + param);
                    break;
                case "facebook":
                    var key = "FacebookIdentity";
                    endpoint = Environment.GetEnvironmentVariable(key + "_Endpoint");
                    clientId = Environment.GetEnvironmentVariable(key + "_ClientId");
                    profileScope = Environment.GetEnvironmentVariable(key + "_ProfileScope");
                    param = "?client_id=" + clientId + "&redirect_uri=" + redirectUri + "&grant_type=implicit&response_type=code&scope=" + profileScope;
                    Response.Redirect(endpoint + "oauth" + param);
                    break;
                case "twitter":
                    key = "TwitterIdentity";
                    endpoint = Environment.GetEnvironmentVariable(key + "_Endpoint");
                    clientId = Environment.GetEnvironmentVariable(key + "_ClientId");
                    profileScope = Environment.GetEnvironmentVariable(key + "_ProfileScope");
                    var requestEndpoint = Environment.GetEnvironmentVariable(key + "_RequestEndpoint");
                    var clientSecret = Environment.GetEnvironmentVariable(key + "_ClientSecret");
                    var bearerToken = Environment.GetEnvironmentVariable(key + "_BearerToken");
                    var nonceSeed = Environment.GetEnvironmentVariable(key + "_NonceSeed");
                    #region creating now
                    //using (var httpClient = new HttpClient()) 
                    //{
                    //}
                    #endregion

                    break;
                default:
                    Response.Redirect("Home");
                    break;
            }
        }
    }
}
