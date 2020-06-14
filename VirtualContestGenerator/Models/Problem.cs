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
        public bool HasSelected { get; set; }

        public string ContestID { get; private set; }
        public Contest Contest { get; set; } = null!;

        public string DifficultyInfoID { get; set; }
        public DifficultyInfo? DifficultyInfo { get; set; }

        public Problem(string id, string title, string contestID, bool hasSelected)
        {
            ID = id;
            Title = title;
            ContestID = contestID;
            DifficultyInfoID = id;
            HasSelected = hasSelected;
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
            HasSelected = false;
        }

        public override string ToString() => Title;
    }
}
