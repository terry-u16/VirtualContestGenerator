using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace VirtualContestGenerator.Models.Json
{
    public class OfficialProblem
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }
        [JsonPropertyName("contest_id")]
        public string? ContestID { get; set; }
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        public override string ToString() => Id ?? string.Empty;
    }
}
