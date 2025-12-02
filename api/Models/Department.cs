using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Department
{
    public string DepartmentId { get; set; } = null!;

    public string DepartmentName { get; set; } = null!;

    public virtual ICollection<Course1> Course1s { get; set; } = new List<Course1>();

    public virtual InstructorIdSequence? InstructorIdSequence { get; set; }

    public virtual ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();

    public virtual ICollection<StudentIdSequence> StudentIdSequences { get; set; } = new List<StudentIdSequence>();

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Course> CourseCodes { get; set; } = new List<Course>();

    public virtual ICollection<Course> CourseCodesNavigation { get; set; } = new List<Course>();

    public virtual ICollection<School> Schools { get; set; } = new List<School>();
}
