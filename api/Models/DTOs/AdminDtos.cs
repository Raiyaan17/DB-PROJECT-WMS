using System;

namespace api.Models.DTOs
{
    public class AddStudentRequest
    {
        public string StudentId { get; set; } = null!;
        public string FName { get; set; } = null!;
        public string LName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string SchoolId { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
        public short GraduationYear { get; set; }
        public string CurrentAcademicYear { get; set; } = null!;
    }

    public class AddInstructorRequest
    {
        public string? InstructorId { get; set; }
        public string FName { get; set; } = null!;
        public string LName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
    }

    public class AddCourseRequest
    {
        public string CourseCode { get; set; } = null!;
        public string CourseTitle { get; set; } = null!;
        public byte TotalCredits { get; set; }
        public short Capacity { get; set; }
        public string DepartmentId { get; set; } = null!;
        public string? InstructorId { get; set; }
        public string? Venue { get; set; }
        public string? DayOfWeek { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class AdminWaitlistRequest
    {
        public string StudentId { get; set; } = null!;
        public string CourseCode { get; set; } = null!;
    }

    public class ForceEnrollRequest
    {
        public string StudentId { get; set; } = null!;
        public string CourseCode { get; set; } = null!;
    }

    public class UpdateCapacityRequest
    {
        public short Capacity { get; set; }
    }

    public class UpdateCourseStatusRequest
    {
        public bool IsActive { get; set; }
    }

    public class UpdateEnrollmentCompletionRequest
    {
        public string StudentId { get; set; } = null!;
        public string CourseCode { get; set; } = null!;
        public bool Completed { get; set; }
    }
}
