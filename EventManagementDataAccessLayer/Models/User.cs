using System;
using System.Collections.Generic;

namespace EventManagementDataAccessLayer.Models;

public partial class User
{
    public Guid Id { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string Email { get; set; } = null!;

    public string? Profileimageurl { get; set; }

    public string Password { get; set; } = null!;

    public string? Phone { get; set; }

    public bool? Isblocked { get; set; }

    public bool? Isverified { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public string? Bio { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Userwallet? Userwallet { get; set; }
}
