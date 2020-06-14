using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualContestGenerator
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; } = default!;
        public GitHubUserInfomation GitHubUserInfomation { get; set; } = default!;
        public int[] DifficultyGroups { get; set; } = default!;
        public VirtualContestTitle VirtualContestTitle { get; set; } = default!;
        public VirtualContestDescription VirtualContestDescription { get; set; } = default!;
        public VirtualContestTime VirtualContestTime { get; set; } = default!;
    }

    public class ConnectionStrings
    {
        public string AtCoderProblemsContext { get; set; } = default!;
    }

    public class GitHubUserInfomation
    {
        public string Id { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public class VirtualContestTitle
    {
        public string Normal { get; set; } = default!;
        public string Another { get; set; } = default!;
    }

    public class VirtualContestDescription
    {
        public string Normal { get; set; } = default!;
        public string Another { get; set; } = default!;
    }

    public class VirtualContestTime
    {
        public DateTimeOffset Since { get; set; }
        public int BeginHour { get; set; }
        public int DurationMinutes { get; set; }
    }

}
