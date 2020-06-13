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

        public VirtualContest(string title, DateTimeOffset startTime, TimeSpan duration) : this(title, startTime, duration, string.Empty, false) { }

        public VirtualContest(string title, DateTimeOffset startTime, TimeSpan duration, string memo, bool isLockout)
        {
            Title = title;
            Memo = memo;
            StartEpochSecond = startTime.ToUnixTimeSeconds();
            DurationSecond = (long)(duration.TotalSeconds + 0.5);
            Mode = isLockout ? "lockout" : null;
        }

        public override string ToString() => Title;
    }
}
