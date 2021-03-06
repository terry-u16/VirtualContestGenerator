﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace VirtualContestGenerator.Models
{
    public class Contest
    {
        public string Id { get; private set; }
        public string Title { get; private set; }
        public DateTimeOffset StartTime { get; private set; }
        public int? MinRated { get; private set; }
        public int? MaxRated { get; private set; }

        public ICollection<Problem> Problems { get; set; } = default!;

        public Contest(string id, string title, DateTimeOffset startTime, int? minRated, int? maxRated)
        {
            Id = id;
            Title = title;
            StartTime = startTime;
            MinRated = minRated;
            MaxRated = maxRated;
        }

        public Contest(Json.OfficialContest contest)
        {
            if (contest.Id == null || contest.Title == null || contest.RateChange == null)
            {
                throw new ArgumentException();
            }

            Id = contest.Id;
            Title = contest.Title.Trim();
            StartTime = DateTimeOffset.FromUnixTimeSeconds(contest.StartEpochSecond);
            (MinRated, MaxRated) = GetRated(contest.RateChange);
        }

        private (int? minRated, int? maxRated) GetRated(string rateChange)
        {
            if (rateChange == "All")
            {
                return (0, int.MaxValue);
            }

            var regex = new Regex(@"^(?<min>[0-9]*) ~ (?<max>[0-9]*)$");
            var match = regex.Match(rateChange);

            if (match.Success)
            {
                var hasMin = int.TryParse(match.Groups["min"].Value, out var minRated);
                var hasMax = int.TryParse(match.Groups["max"].Value, out var maxRated);
                return (hasMin ? minRated : 0, hasMax ? maxRated : int.MaxValue);
            }
            else
            {
                return (null, null);
            }
        }

        public override string ToString() => Title;
    }
}
