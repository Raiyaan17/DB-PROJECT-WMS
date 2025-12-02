using System;
using System.Collections.Generic;

namespace api.Models;

public partial class VwAdminDashboardStat
{
    public string DepartmentName { get; set; } = null!;

    public int? TotalStudents { get; set; }

    public int? TotalInstructors { get; set; }

    public int? TotalCourses { get; set; }
}
