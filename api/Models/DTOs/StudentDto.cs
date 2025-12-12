namespace api.Models.DTOs
{
    public class StudentDto
    {
        public string StudentId { get; set; } = null!;
        public string Fname { get; set; } = null!;
        public string Lname { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string DepartmentId { get; set; } = null!;
        public short GraduationYear { get; set; }
        public string SchoolName { get; set; } = null!;
    }
}
