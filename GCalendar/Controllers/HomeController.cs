using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GCalendar.Models;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2.Responses;
using System.Threading;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;

namespace GCalendar.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Calendar()
        {
            try
            {
                string[] scopes = { CalendarService.Scope.Calendar }; ;
                var secrets = new ClientSecrets()
                {
                    ClientSecret = "jNBLrfCz3GH51Z2VGeuId9AK",
                    ClientId = "1091665464785-27vik6do89in04ej2ssa5ui5031cv0ot.apps.googleusercontent.com"
                };
                var credential = await GetCalendarCredential("TestUser", scopes, secrets);
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Calendar App"
                });
                var calendar = new Calendar
                {
                    Summary = "Test Calendar ",
                    TimeZone = "Asia/Kathmandu",
                    Description = "Calendar created."
                };
                //calendar = service.Calendars.Insert(calendar).Execute();
                //it is working fine.
                return Ok($"Calendar service connected successfully.");
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        private async Task<UserCredential> GetCalendarCredential(string key, string[] scopes, ClientSecrets gsec)
        {
            try
            {
                var initializer = new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = gsec,
                    Scopes = scopes
                };
                initializer.DataStore = new FileDataStore("OAuth", true);
                var flow = new GoogleAuthorizationCodeFlow(initializer);

                var token = await initializer.DataStore.GetAsync<TokenResponse>(key);
                if (token == null)
                {
                    var result = await AuthorizeAsync(initializer, key);
                    return new UserCredential(result.Flow, key, result.Token);
                }
                return new UserCredential(flow, key, token);
            }
            catch (Exception ex)
            {
                System.IO.File.WriteAllText("error.txt", ex.Message);
                return null;
            }
        }

        private async Task<UserCredential> AuthorizeAsync(GoogleAuthorizationCodeFlow.Initializer initializer, string user)
        {
            var flow = new GoogleAuthorizationCodeFlow(initializer);
            var codeReceiver = new LocalServerCodeReceiver();

            // Create an authorization code installed app instance and authorize the user.
            return await new AuthorizationCodeInstalledApp(flow, codeReceiver).AuthorizeAsync
                (user, CancellationToken.None).ConfigureAwait(false);
        }


    }
}
