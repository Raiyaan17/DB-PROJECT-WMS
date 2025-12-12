using System;
using System.Collections.Generic;

namespace api.Models;

public partial class VDepartmentStudentCount
{
    public string DepartmentId { get; set; } = null!;

    public string DepartmentName { get; set; } = null!;

    public int? NumberOfStudents { get; set; }
}
