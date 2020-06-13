using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace VirtualContestGenerator.Models
{
    public class DifficultyInfo
    {
        public string ID { get; set; }
        public double? Difficulty { get; set; }
        public bool IsExperimental { get; set; }

        public string QuestionID { get; set; }
        public Problem? Question { get; set; }

        public DifficultyInfo(string id, double? difficulty, bool isExperimental)
        {
            ID = id;
            QuestionID = id;
            Difficulty = difficulty;
            IsExperimental = isExperimental;
        }

        public DifficultyInfo(string id, Json.DifficultyInfo difficultyInfo)
        {
            ID = id;
            QuestionID = id;
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
