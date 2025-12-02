using System;
using System.Collections.Generic;

namespace api.Models;

public partial class VwFullPrerequisiteList
{
    public string CourseCode { get; set; } = null!;

    public string CourseTitle { get; set; } = null!;

    public string PrerequisiteCode { get; set; } = null!;

    public string PrerequisiteTitle { get; set; } = null!;
}
