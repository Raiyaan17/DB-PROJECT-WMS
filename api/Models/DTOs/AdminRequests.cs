using System;

namespace api.Models.DTOs
{
    public class UpdateStudentRequest
    {
        public string? FName { get; set; }
        public string? LName { get; set; }
        public string? Email { get; set; }
        public string? SchoolId { get; set; }
        public string? DepartmentId { get; set; }
        public short? GraduationYear { get; set; }
        public string? CurrentAcademicYear { get; set; }
    }

    public class UpdateInstructorRequest
    {
        public string? FName { get; set; }
        public string? LName { get; set; }
        public string? Email { get; set; }
        public string? DepartmentId { get; set; }
    }

    public class UpdateCourseRequest
    {
        public string? CourseTitle { get; set; }
        public byte? TotalCredits { get; set; }
        public short? Capacity { get; set; }
        public string? DepartmentId { get; set; }
        public string? InstructorId { get; set; }
        public string? Venue { get; set; }
        public string? DayOfWeek { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public bool? IsActive { get; set; }
    }
}
