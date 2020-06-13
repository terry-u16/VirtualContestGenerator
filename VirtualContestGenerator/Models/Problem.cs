using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VirtualContestGenerator.Models
{
    public class Problem
    {
        public string ID { get; private set; }
        public string Title { get; private set; }

        public string ContestID { get; private set; }
        public Contest Contest { get; set; } = null!;

        public string DifficultyInfoID { get; set; }
        public DifficultyInfo? DifficultyInfo { get; set; }

        public Problem(string id, string title, string contestID)
        {
            ID = id;
            Title = title;
            ContestID = contestID;
            DifficultyInfoID = id;
        }

        public Problem(Json.OfficialProblem problem)
        {
            if (problem.ID == null || problem.ContestID == null || problem.Title == null)
            {
                throw new ArgumentException();
            }

            ID = problem.ID;
            Title = problem.Title;
            ContestID = ContestID;
            DifficultyInfoID = ID;
        }

        public override string ToString() => Title;
    }
}
