﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace VirtualContestGenerator.Models.Json
{
    public class ProblemSet
    {
        [JsonPropertyName("contest_id")]
        public string ContestID { get; }
        [JsonPropertyName("problems")]
        public Problem[] Problems { get; }

        public ProblemSet(string contestID, params Problem[] problems)
        {
            ContestID = contestID;
            Problems = problems;
        }

        public ProblemSet(string contestID, params (string id, int? point)[] problems) 
            : this(contestID, problems.Select((p, index) => new Problem(p.id, p.point, index + 1)).ToArray()) { }

        public override string ToString() => string.Join(", ", Problems.OrderBy(p => p.Order).ToString());
    }

    public class Problem
    {
        [JsonPropertyName("id")]
        public string ID { get; }
        [JsonPropertyName("point")]
        public int? Point { get; }
        [JsonPropertyName("order")]
        public int Order { get; }

        public Problem(string id, int? point, int order)
        {
            ID = id;
            Point = point;
            Order = order;
        }

        public override string ToString() => ID;
    }
}
