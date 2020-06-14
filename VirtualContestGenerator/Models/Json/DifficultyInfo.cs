using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace VirtualContestGenerator.Models.Json
{
    public class DifficultyInfo
    {
        [JsonPropertyName("difficulty")]
        public double? InnerDifficulty { get; set; }
        [JsonPropertyName("is_experimental")]
        public bool IsExperimental { get; set; }

        public override string ToString() => InnerDifficulty != null ? Math.Round(InnerDifficulty.Value).ToString() : "unavailable";
    }
}
