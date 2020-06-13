using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using AngleSharp.Html.Parser;
using System.Linq;
using System.IO;
using VirtualContestGenerator.Models.Json;
using System.Text.Json;

namespace VirtualContestGenerator.Services
{
    public class VirtualContestService
    {
        private bool _hasLoggedIn = false;
        private HttpClient _client = default!;

        public async Task LoginAsync(string githubID, string githubPassword)
        {
            if (_hasLoggedIn)
            {
                throw new InvalidOperationException("ログイン済みです。");
            }

            var handler = new HttpClientHandler
            {
                UseCookies = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            _client = new HttpClient(handler) { BaseAddress = new Uri("https://kenkoooo.com/atcoder/") };
            _client.DefaultRequestHeaders.Referrer = new Uri("https://github.com/");

            var loginFormResponse = await GetLoginFormAsync();
            loginFormResponse.EnsureSuccessStatusCode();
            var redirectFormResponse = await PostLoginAsync(await loginFormResponse.Content.ReadAsStreamAsync(), githubID, githubPassword);
            redirectFormResponse.EnsureSuccessStatusCode();
            var redirectedReponse = await RedirectAsync(await redirectFormResponse.Content.ReadAsStreamAsync());
            redirectedReponse.EnsureSuccessStatusCode();

            _hasLoggedIn = true;
            _client.DefaultRequestHeaders.Referrer = new Uri("https://kenkoooo.com/atcoder/");
        }

        private async Task<HttpResponseMessage> GetLoginFormAsync()
        {
            const string loginUri = "https://github.com/login/oauth/authorize?client_id=162a5276634fc8b970f7";
            return await _client.GetAsync(loginUri);
        }

        private async Task<HttpResponseMessage> PostLoginAsync(Stream loginFormContent, string githubID, string githubPassword)
        {
            const string loginUri = "https://github.com/sessio";
            const string redirectUri = "/login/oauth/authorize?client_id=162a5276634fc8b970f7";
            var parser = new HtmlParser();
            var html = await parser.ParseDocumentAsync(loginFormContent);

            var authenticityToken = html.GetElementsByName("authenticity_token").FirstOrDefault().Attributes["value"].Value;
            var timestamp = html.GetElementsByName("timestamp").FirstOrDefault().Attributes["value"].Value;
            var timestampSecret = html.GetElementsByName("timestamp_secret").FirstOrDefault().Attributes["value"].Value;

            var parameters = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "commit", "Sign in" },
                { "authenticity_token", authenticityToken },
                { "login", githubID },
                { "password", githubPassword },
                { "webauthn-support", "supported" },
                { "webauthn-iuvpaa-support", "supported" },
                { "return_to", redirectUri },
                { "timestamp", timestamp },
                { "timestamp_secret", timestampSecret }
            });

            return await _client.PostAsync(loginUri, parameters);
        }

        private async Task<HttpResponseMessage> RedirectAsync(Stream redirectPageContent)
        {
            var parser = new HtmlParser();
            var html = await parser.ParseDocumentAsync(redirectPageContent);
            var redirectUri = html.GetElementById("redirect").Attributes["href"].Value;
            return await _client.GetAsync(redirectUri);
        }

        public async Task<VirtualContestID> CreateVirtualContestAsync(VirtualContest contest)
        {
            if (!_hasLoggedIn)
            {
                throw new InvalidOperationException("ログインしてください。");
            }

            var json = JsonSerializer.Serialize(contest);
            var response = await _client.PostAsync("/atcoder/internal-api/contest/create", new StringContent(json, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            var contestID = await JsonSerializer.DeserializeAsync<VirtualContestID>(await response.Content.ReadAsStreamAsync());
            return contestID;
        }

        public async Task UpdateVirtualContestAsync(ProblemSet problemSet)
        {
            if (!_hasLoggedIn)
            {
                throw new InvalidOperationException("ログインしてください。");
            }

            var json = JsonSerializer.Serialize(problemSet);
            var response = await _client.PostAsync("/atcoder/internal-api/contest/item/update", new StringContent(json, Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
        }
    }
}
