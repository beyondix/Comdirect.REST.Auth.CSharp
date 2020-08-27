using Comdirect.Auth.CSharp.Dtos;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Comdirect.Auth.CSharp.Services
{
    public class ComdirectAuthService : IComdirectAuthService
    {
        private const string _baseUrl = "https://api.comdirect.de";
        private readonly ComdirectCredentials _comdirectCredentials;

        public string SessionId { get; set; }

        public ComdirectAuthService(ComdirectCredentials comdirectCredentials)
        {
            _comdirectCredentials = comdirectCredentials;
        }

        public async Task<ValidComdirectToken> RunInitial()
        {
            var getTokenResponse = await GetToken();
            var getSessionStatusResponse = await GetSessionStatus(getTokenResponse.AccessToken);
            var validateSessionResponse = await ValidateSession(getTokenResponse.AccessToken, getSessionStatusResponse.Identifier);

            Thread.Sleep(TimeSpan.FromMinutes(1));

            await ActivateTan(getTokenResponse.AccessToken, getSessionStatusResponse.Identifier, validateSessionResponse.Id);
            var validComdirectToken = await RunSecondaryFlow(getTokenResponse.AccessToken);

            return validComdirectToken;
        }

        public async Task<ValidComdirectToken> RunRefreshTokenFlow(string refreshToken)
        {
            string tokenEndpoint = $"{_baseUrl}/oauth/token";

            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var requestParameters = new Dictionary<string, string>
            {
                { "client_id", _comdirectCredentials.ClientId },
                { "client_secret", _comdirectCredentials.ClientSecret },
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint) { Content = new FormUrlEncodedContent(requestParameters) };

            var result = await httpClient.SendAsync(request);

            return JsonConvert.DeserializeObject<ValidComdirectToken>(await result.Content.ReadAsStringAsync());
        }

        private async Task<ComdirectGetTokenResponse> GetToken()
        {
            string tokenEndpoint = $"{_baseUrl}/oauth/token";

            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var comdirectGetTokenRequest = new ComdirectGetTokenRequest
            {
                ClientId = _comdirectCredentials.ClientId,
                ClientSecret = _comdirectCredentials.ClientSecret,
                Password = _comdirectCredentials.Pin,
                Username = _comdirectCredentials.Username,
                GrantType = "password"
            };

            var requestParameters = new Dictionary<string, string>
            {
                { "client_id", comdirectGetTokenRequest.ClientId },
                { "client_secret", comdirectGetTokenRequest.ClientSecret },
                { "grant_type", comdirectGetTokenRequest.GrantType },
                { "username", comdirectGetTokenRequest.Username },
                { "password", comdirectGetTokenRequest.Password }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint) { Content = new FormUrlEncodedContent(requestParameters) };

            var result = await httpClient.SendAsync(request);

            return JsonConvert.DeserializeObject<ComdirectGetTokenResponse>(await result.Content.ReadAsStringAsync());
        }

        private async Task<ComdirectGetSessionStatusResponse> GetSessionStatus(string accessToken)
        {
            string sessionStatusEndpoint = $"{_baseUrl}/api/session/clients/user/v1/sessions";

            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders
               .Accept
               .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

            var httpRequestInfo = new { clientRequestId = new { sessionId = SessionId, requestId = Guid.NewGuid() } };
            var serializedHttpRequestInfo = JsonConvert.SerializeObject(httpRequestInfo);

            httpClient.DefaultRequestHeaders.Add("x-http-request-info", serializedHttpRequestInfo);

            var request = new HttpRequestMessage(HttpMethod.Get, sessionStatusEndpoint);

            var result = await httpClient.SendAsync(request);

            var comdirectSessionStatus = JsonConvert.DeserializeObject<List<ComdirectGetSessionStatusResponse>>(await result.Content.ReadAsStringAsync());
            return comdirectSessionStatus[0];
        }

        private async Task<ComdirectValidateSessionResponse> ValidateSession(string accessToken, string sessionIdentifier)
        {
            string validateSessionEndpoint = $"{_baseUrl}/api/session/clients/{_comdirectCredentials.ClientId}/v1/sessions/{sessionIdentifier}/validate";

            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

            httpClient.DefaultRequestHeaders
              .Accept
              .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var comdirectValidateSessionRequest = new ComdirectValidateSessionRequest { Identifier = sessionIdentifier };

            using var content = new StringContent(JsonConvert.SerializeObject(comdirectValidateSessionRequest), System.Text.Encoding.UTF8, "application/json");

            var httpRequestInfo = new { clientRequestId = new { sessionId = SessionId, requestId = Guid.NewGuid() } };
            var serializedHttpRequestInfo = JsonConvert.SerializeObject(httpRequestInfo);

            httpClient.DefaultRequestHeaders.Add("x-http-request-info", serializedHttpRequestInfo);

            var result = await httpClient.PostAsync(validateSessionEndpoint, content);

            var responseAuthHeader = result.Headers.GetValues("x-once-authentication-info").First();
            var comdirectValidateSessionResponse = JsonConvert.DeserializeObject<ComdirectValidateSessionResponse>(responseAuthHeader);

            return comdirectValidateSessionResponse;
        }

        private async Task ActivateTan(string accessToken, string sessionIdentifier, string authHeaderId)
        {
            string activateTanEndpoint = $"{_baseUrl}/api/session/clients/{_comdirectCredentials.ClientId}/v1/sessions/{sessionIdentifier}";

            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", accessToken);

            httpClient.DefaultRequestHeaders
               .Accept
               .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var httpRequestInfo = new { clientRequestId = new { sessionId = SessionId, requestId = Guid.NewGuid() } };
            var serializedHttpRequestInfo = JsonConvert.SerializeObject(httpRequestInfo);

            httpClient.DefaultRequestHeaders.Add("x-http-request-info", serializedHttpRequestInfo);
            httpClient.DefaultRequestHeaders.Add("x-once-authentication-info", JsonConvert.SerializeObject(new { id = authHeaderId }));

            var comdirectActivateTanRequest = new ComdirectActivateTanRequest { Identifier = sessionIdentifier };
            using var content = new StringContent(JsonConvert.SerializeObject(comdirectActivateTanRequest), System.Text.Encoding.UTF8, "application/json");

            await httpClient.PatchAsync(activateTanEndpoint, content);
        }

        private async Task<ValidComdirectToken> RunSecondaryFlow(string accessToken)
        {
            string tokenEndpoint = $"{_baseUrl}/oauth/token";

            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders
               .Accept
               .Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var requestParameters = new Dictionary<string, string>
            {
                { "client_id", _comdirectCredentials.ClientId },
                { "client_secret", _comdirectCredentials.ClientSecret },
                { "grant_type", "cd_secondary" },
                { "token", accessToken }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint) { Content = new FormUrlEncodedContent(requestParameters) };

            var result = await httpClient.SendAsync(request);
            var comdirectValidToken = JsonConvert.DeserializeObject<ValidComdirectToken>(await result.Content.ReadAsStringAsync());

            return comdirectValidToken;
        }
    }
}
