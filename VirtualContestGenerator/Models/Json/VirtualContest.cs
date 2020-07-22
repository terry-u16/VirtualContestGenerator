using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace VirtualContestGenerator.Models.Json
{
    public class VirtualContest
    {
        [JsonPropertyName("title")]
        public string Title { get; }
        [JsonPropertyName("memo")]
        public string Memo { get; }
        [JsonPropertyName("start_epoch_second")]
        public long StartEpochSecond { get; }
        [JsonPropertyName("duration_second")]
        public long DurationSecond { get; }
        [JsonPropertyName("mode")]
        public string? Mode { get; }
        [JsonPropertyName("penalty_second")]
        public int PenaltySecond { get; }

        public VirtualContest(string title, DateTimeOffset startTime, TimeSpan duration) : this(title, startTime, duration, string.Empty, false, 300) { }

        public VirtualContest(string title, DateTimeOffset startTime, TimeSpan duration, string memo, bool isLockout, int penaltySecond)
        {
            Title = title;
            Memo = memo;
            StartEpochSecond = startTime.ToUnixTimeSeconds();
            DurationSecond = (long)(duration.TotalSeconds + 0.5);
            Mode = isLockout ? "lockout" : null;
            PenaltySecond = penaltySecond;
        }

        public override string ToString() => Title;
    }
}
