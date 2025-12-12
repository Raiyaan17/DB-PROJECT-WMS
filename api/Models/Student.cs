using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Student
{
    public string StudentId { get; set; } = null!;

    public string Fname { get; set; } = null!;

    public string Lname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public string SchoolId { get; set; } = null!;

    public string DepartmentId { get; set; } = null!;

    public short GraduationYear { get; set; }

    public string? CurrentAcademicYear { get; set; }

    public virtual ICollection<CourseCart> CourseCarts { get; set; } = new List<CourseCart>();

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual School School { get; set; } = null!;

    public virtual ICollection<Waitlist> Waitlists { get; set; } = new List<Waitlist>();

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string SchoolName { get; set; } = null!;
}
