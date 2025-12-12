using System;
using System.Collections.Generic;

namespace api.Models;

public partial class School
{
    public string SchoolId { get; set; } = null!;

    public string SchoolName { get; set; } = null!;

    public virtual ICollection<Student> Students { get; set; } = new List<Student>();

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();
}
