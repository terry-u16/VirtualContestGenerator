﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace VirtualContestGenerator.Models
{
    public class DifficultyInfo
    {
        public string Id { get; set; }
        public double? Difficulty { get; set; }
        public bool IsExperimental { get; set; }

        public string ProblemId { get; set; } = default!;
        public Problem Problem { get; set; } = default!;

        public DifficultyInfo(string id, double? difficulty, bool isExperimental)
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

        private double GetDifficultyFrom(double innerDifficulty)
        {
            if (innerDifficulty >= 400)
            {
                return innerDifficulty;
            }
            else
            {
                return 400.0 / Math.Pow(Math.E, (400.0 - innerDifficulty) / 400.0);
            }
        }
    }
}
