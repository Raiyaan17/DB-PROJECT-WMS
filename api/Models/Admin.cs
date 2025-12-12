using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Admin
{
    public string AdminId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;
}
