using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Domain.ValueObjects
{
    public class DateRange : BaseValueObject
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        private DateRange() { }

        public DateRange(DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date cannot be before start date", nameof(endDate));

            StartDate = startDate.Date; // Remove time component
            EndDate = endDate.Date;
        }

        public int GetDays()
        {
            return (EndDate - StartDate).Days + 1; // Inclusive
        }

        public bool Contains(DateTime date)
        {
            return date.Date >= StartDate && date.Date <= EndDate;
        }

        public bool Overlaps(DateRange other)
        {
            if (other == null) return false;

            return StartDate <= other.EndDate && EndDate >= other.StartDate;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StartDate;
            yield return EndDate;
        }

        public override string ToString()
        {
            return $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}";
        }
    }
}
