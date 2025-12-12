namespace api.Models.DTOs
{
    public class StudentSummaryDto
    {
        public string StudentId { get; set; } = null!;
        public string FName { get; set; } = null!;
        public string LName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string SchoolId { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
        public short GraduationYear { get; set; }
        public string? CurrentAcademicYear { get; set; }
    }

    public class InstructorSummaryDto
    {
        public string InstructorId { get; set; } = null!;
        public string FName { get; set; } = null!;
        public string LName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
    }
}
