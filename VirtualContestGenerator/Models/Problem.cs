using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VirtualContestGenerator.Models
{
    public class Problem
    {
        public string Id { get; private set; }
        public string Title { get; private set; }
        public bool HasSelected { get; set; }

        public string ContestId { get; private set; }
        public Contest Contest { get; set; } = null!;

        public DifficultyInfo? DifficultyInfo { get; set; }

        public Problem(string id, string title, string contestId, bool hasSelected)
        {
            Id = id;
            Title = title;
            ContestId = contestId;
            HasSelected = hasSelected;
        }

        public Problem(Json.OfficialProblem problem)
        {
            if (problem.Id == null || problem.ContestID == null || problem.Title == null)
            {
                throw new ArgumentException();
            }

            Id = problem.Id;
            Title = problem.Title.Trim();
            ContestId = ContestId;
            HasSelected = false;
        }

        public override string ToString() => Title;
    }
}
