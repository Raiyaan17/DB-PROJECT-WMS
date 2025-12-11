using System;

namespace api.Models.DTOs
{
    public class CourseDetailSpResult
    {
        public string CourseCode { get; set; } = null!;
        public string CourseTitle { get; set; } = null!;
        public byte TotalCredits { get; set; }
        public short Capacity { get; set; }
        public int EnrolledCount { get; set; }
        public int RemainingSeats { get; set; }
        public string? Venue { get; set; }
        public string? DayOfWeek { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public bool IsActive { get; set; }
        public string? PrerequisiteCode { get; set; }
        public string? PrerequisiteTitle { get; set; }
    }
}
