using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Enrollment
{
    public string StudentId { get; set; } = null!;

    public short GraduationYear { get; set; }

    public string CourseCode { get; set; } = null!;

    public bool Completed { get; set; }

    public bool IsForced { get; set; }

    public DateTime EnrollmentDate { get; set; }

    public virtual Course CourseCodeNavigation { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
