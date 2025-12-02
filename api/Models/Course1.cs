using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Course1
{
    public string CourseCode { get; set; } = null!;

    public string DepartmentId { get; set; } = null!;

    public virtual Department Department { get; set; } = null!;
}
