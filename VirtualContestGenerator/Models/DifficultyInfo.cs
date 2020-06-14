using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace VirtualContestGenerator.Models
{
    public class DifficultyInfo
    {
        public string Id { get; set; }
        public int? Difficulty { get; set; }
        public bool IsExperimental { get; set; }

        public string ProblemId { get; set; } = default!;
        public Problem Problem { get; set; } = default!;

        public DifficultyInfo(string id, int? difficulty, bool isExperimental)
        {
            Id = id;
            ProblemId = id;
            Difficulty = difficulty;
            IsExperimental = isExperimental;
        }

        public DifficultyInfo(string id, Json.DifficultyInfo difficultyInfo)
        {
            Id = id;
            ProblemId = id;
            Difficulty = GetDifficultyFrom(difficultyInfo.InnerDifficulty);
            IsExperimental = difficultyInfo.IsExperimental;
        }

        private int? GetDifficultyFrom(double? innerDifficulty)
        {
            if (innerDifficulty == null)
            {
                return null;
            }
            else if(innerDifficulty >= 400)
            {
                return (int)(innerDifficulty + 0.5);
            }
            else
            {
                return (int)(400.0 / Math.Pow(Math.E, (400.0 - innerDifficulty.Value) / 400.0) + 0.5);
            }
        }

        public override string ToString() => $"{Id} | diff: {Difficulty}";
    }
}
