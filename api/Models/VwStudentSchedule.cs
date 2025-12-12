using System;
using System.Collections.Generic;

namespace api.Models;

public partial class VwStudentSchedule
{
    public string StudentId { get; set; } = null!;

    public string CourseCode { get; set; } = null!;

    public string CourseTitle { get; set; } = null!;

    public string? DayOfWeek { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public string? Venue { get; set; }

    public string DepartmentName { get; set; } = null!;

    public int TotalCredits { get; set; }
}
