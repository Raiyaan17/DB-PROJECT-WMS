using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Waitlist
{
    public string CourseCode { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public short GraduationYear { get; set; }

    public DateTime WaitlistTimestamp { get; set; }

    public virtual Course CourseCodeNavigation { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
