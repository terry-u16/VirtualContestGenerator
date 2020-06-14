using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VirtualContestGenerator.Models;

namespace VirtualContestGenerator.Services
{
    public class FetchingJsonService
    {
        private HttpClient _client;

        public FetchingJsonService()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            _client = new HttpClient(handler)
            {
                BaseAddress = new Uri("https://kenkoooo.com/")
            };
        }

        public async Task<IEnumerable<Problem>> GetProblemsAsync()
        {
            var response = await _client.GetAsync("/atcoder/resources/problems.json");
            response.EnsureSuccessStatusCode();
            var parsedJson = await JsonSerializer.DeserializeAsync<Models.Json.OfficialProblem[]>(await response.Content.ReadAsStreamAsync());
            return parsedJson.Select(pb => new Problem(pb));
        }

        public async Task<IEnumerable<Contest>> GetContestsAsync()
        {
            var response = await _client.GetAsync("/atcoder/resources/contests.json");
            response.EnsureSuccessStatusCode();
            var parsedJson = await JsonSerializer.DeserializeAsync<Models.Json.OfficialContest[]>(await response.Content.ReadAsStreamAsync());
            return parsedJson.Select(con => new Contest(con));
        }

        public async Task<IEnumerable<DifficultyInfo>> GetDifficultyInfoAsync()
        {
            var response = await _client.GetAsync("/atcoder/resources/problem-models.json");
            response.EnsureSuccessStatusCode();
            var parsedJson = await JsonSerializer.DeserializeAsync<Dictionary<string, Models.Json.DifficultyInfo>>(await response.Content.ReadAsStreamAsync());
            return parsedJson.Select(p => new DifficultyInfo(p.Key, p.Value));
        }
    }
}
