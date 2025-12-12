using System;
using System.Collections.Generic;

namespace api.Models;

public partial class AuditLog
{
    public int AuditId { get; set; }

    public string AdminId { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public string CourseCode { get; set; } = null!;

    public DateTime Timestamp { get; set; }

    public string ActionDescription { get; set; } = null!;
}
