using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.ValueObjects
{
    public class TimeRange : BaseValueObject
    {
        public TimeSpan StartTime { get; private set; }
        public TimeSpan EndTime { get; private set; }

        private TimeRange() { }

        public TimeRange(TimeSpan startTime, TimeSpan endTime)
        {
            if (endTime < startTime)
                throw new ArgumentException("End time cannot be before start time", nameof(endTime));

            if (startTime < TimeSpan.Zero || startTime >= TimeSpan.FromDays(1))
                throw new ArgumentException("Start time must be within a 24-hour period", nameof(startTime));

            if (endTime < TimeSpan.Zero || endTime > TimeSpan.FromDays(1))
                throw new ArgumentException("End time must be within a 24-hour period", nameof(endTime));

            StartTime = startTime;
            EndTime = endTime;
        }

        public TimeSpan GetDuration()
        {
            return EndTime - StartTime;
        }

        public decimal GetHours()
        {
            return (decimal)GetDuration().TotalHours;
        }

        public bool Overlaps(TimeRange other)
        {
            if (other == null) return false;

            return StartTime < other.EndTime && EndTime > other.StartTime;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StartTime;
            yield return EndTime;
        }

        public override string ToString()
        {
            return $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
        }
    }
}
