using System;
using System.Collections.Generic;

namespace api.Models;

public partial class VwAvailableCourse
{
    public string CourseCode { get; set; } = null!;

    public string CourseTitle { get; set; } = null!;

    public byte TotalCredits { get; set; }

    public string DepartmentId { get; set; } = null!;

    public string? Venue { get; set; }

    public string? DayOfWeek { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public int EnrolledCount { get; set; }

    public short Capacity { get; set; }

    public int RemainingSeats { get; set; }

    public string DepartmentName { get; set; } = null!;

    public string? InstructorName { get; set; }
}
