using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Course
{
    public string CourseCode { get; set; } = null!;

    public string CourseTitle { get; set; } = null!;

    public byte TotalCredits { get; set; }

    public short Capacity { get; set; }

    public string? Venue { get; set; }

    public string? DayOfWeek { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public int EnrolledCount { get; set; }

    public int RemainingSeats { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<CourseCart> CourseCarts { get; set; } = new List<CourseCart>();

    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    public virtual ICollection<Waitlist> Waitlists { get; set; } = new List<Waitlist>();

    public virtual ICollection<Course> IsPrerequisiteFor { get; set; } = new List<Course>();

    public virtual ICollection<Department> CoreForDepartments { get; set; } = new List<Department>();

    public virtual ICollection<Department> ElectiveForDepartments { get; set; } = new List<Department>();

    public virtual ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();

    public virtual ICollection<Course> Prerequisites { get; set; } = new List<Course>();
}
