using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Services.ApiModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Services.ServicesHelpers.GoogleMeetService
{
    public class GoogleMeetService
    {
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _redirectUri;
        private TokenResponse token;

        public GoogleMeetService(IConfiguration configuration)
        {
            _clientId = configuration["Google:ClientId"];
            _clientSecret = configuration["Google:ClientSecret"];
            _redirectUri = configuration["Google:RedirectUri"];
        }

        public string GetAuthorizationUrl()
        {
            var authorizationUrl = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                },
                Scopes = new[] { CalendarService.Scope.Calendar },
                DataStore = new Google.Apis.Util.Store.FileDataStore("Tokens")
            }).CreateAuthorizationCodeRequest(_redirectUri).Build();

            return authorizationUrl.ToString();
        }

        public async Task<bool> ExchangeCodeForToken(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                Console.WriteLine("Code is null or empty");
                throw new ArgumentException("Authorization code is missing or invalid.");
            }

            string decodedCode = System.Net.WebUtility.UrlDecode(code);

            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                },
                Scopes = new[] { CalendarService.Scope.Calendar },
                DataStore = new Google.Apis.Util.Store.FileDataStore("Tokens")
            });
            try
            {
                token = await flow.ExchangeCodeForTokenAsync("user", decodedCode, _redirectUri, CancellationToken.None);
                return token != null;
            }
            catch (TokenResponseException ex)
            {
                if (ex.Error?.ErrorDescription != null)
                    Console.WriteLine($"Detailed error: {ex.Error.ErrorDescription}");
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<string> CreateGoogleMeet(MeetRequest request)
        {
            if (token == null)
            {
                return "Unauthorized: You must authenticate first.";
            }

            var credential = new UserCredential(new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = _clientId,
                    ClientSecret = _clientSecret
                },
                Scopes = new[] { CalendarService.Scope.Calendar },
                DataStore = new Google.Apis.Util.Store.FileDataStore("Tokens")
            }), "user", token);

            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Meet API",
            });

            var newEvent = new Event()
            {
                Summary = request.Title,
                Start = new EventDateTime() { DateTime = request.StartTime, TimeZone = "Asia/Ho_Chi_Minh" },
                End = new EventDateTime() { DateTime = request.EndTime, TimeZone = "Asia/Ho_Chi_Minh" },
                ConferenceData = new ConferenceData()
                {
                    CreateRequest = new CreateConferenceRequest()
                    {
                        RequestId = Guid.NewGuid().ToString(),
                        ConferenceSolutionKey = new ConferenceSolutionKey() { Type = "hangoutsMeet" }
                    }
                }
            };

            var eventRequest = service.Events.Insert(newEvent, "primary");
            eventRequest.ConferenceDataVersion = 1;
            var createdEvent = await eventRequest.ExecuteAsync();

            return createdEvent.HangoutLink;
        }
    }
}