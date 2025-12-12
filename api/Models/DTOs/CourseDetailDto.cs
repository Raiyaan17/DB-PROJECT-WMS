using System.Collections.Generic;

namespace api.Models.DTOs
{
    public class CourseDetailDto
    {
        public string? CourseCode { get; set; }
        public string? CourseTitle { get; set; }
        public byte TotalCredits { get; set; }
        public short Capacity { get; set; }
        public int EnrolledCount { get; set; }
        public int RemainingSeats { get; set; }
        public string? Venue { get; set; }
        public string? DayOfWeek { get; set; }
        public System.TimeOnly? StartTime { get; set; }
        public System.TimeOnly? EndTime { get; set; }
        public bool IsActive { get; set; }
        public string BllMode { get; set; } = "Unknown";
        public string DataSource { get; set; } = "Unknown";
        public List<PrerequisiteDto> Prerequisites { get; set; } = new List<PrerequisiteDto>();
    }
}
