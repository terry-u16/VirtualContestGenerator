using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace VirtualContestGenerator.Models.Json
{
    public class VirtualContestID
    {
        [JsonPropertyName("contest_id")]
        public string? ContestID { get; set; }

        public override string ToString() => ContestID;
    }
}
