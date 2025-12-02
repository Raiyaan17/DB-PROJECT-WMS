using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Instructor
{
    public string InstructorId { get; set; } = null!;

    public string Fname { get; set; } = null!;

    public string Lname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string DepartmentId { get; set; } = null!;

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<Course> TaughtCourses { get; set; } = new List<Course>();
}
