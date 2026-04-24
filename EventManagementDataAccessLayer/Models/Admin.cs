using System;
using System.Collections.Generic;

namespace EventManagementDataAccessLayer.Models;

public partial class Admin
{
    public Guid Id { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Phone { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Adminwallet? Adminwallet { get; set; }
}
