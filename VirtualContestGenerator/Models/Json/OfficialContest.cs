using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace VirtualContestGenerator.Models.Json
{
    public class OfficialContest
    {
        [JsonPropertyName("id")]
        public string? ID { get; set; }
        [JsonPropertyName("start_epoch_second")]
        public long StartEpochSecond { get; set; }
        [JsonPropertyName("duration_second")]
        public long DurationSecond { get; set; }
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        [JsonPropertyName("rate_change")]
        public string? RateChange { get; set; }

        public override string ToString() => Title ?? string.Empty;
    }
}
