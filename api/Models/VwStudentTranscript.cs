using System;
using System.Collections.Generic;

namespace api.Models;

public partial class VwStudentTranscript
{
    public string StudentId { get; set; } = null!;

    public string CourseCode { get; set; } = null!;

    public string CourseTitle { get; set; } = null!;

    public byte TotalCredits { get; set; }

    public bool Completed { get; set; }

    public DateTime EnrollmentDate { get; set; }
}
