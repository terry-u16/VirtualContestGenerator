using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace VirtualContestGenerator.Models.Json
{
    public class VirtualContestID
    {
        [JsonPropertyName("contest_id")]
        public string ContestId { get; set; } = default!;

        public override string ToString() => ContestId ?? "";
    }
}
