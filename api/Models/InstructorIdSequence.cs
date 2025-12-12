using System;
using System.Collections.Generic;

namespace api.Models;

public partial class InstructorIdSequence
{
    public string DepartmentId { get; set; } = null!;

    public int LastNumber { get; set; }

    public virtual Department Department { get; set; } = null!;
}
