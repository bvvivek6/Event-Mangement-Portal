using System;
using System.Collections.Generic;

namespace EventManagementDataAccessLayer.Models;

public partial class Booking
{
    public Guid Id { get; set; }

    public Guid? Userid { get; set; }

    public Guid? Eventid { get; set; }

    public decimal? Totalamount { get; set; }

    public string? Status { get; set; }

    public string? Bookingreference { get; set; }

    public string? Idempotencykey { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public bool? Isdeleted { get; set; }

    public string? Bookingsource { get; set; }

    public virtual ICollection<Bookingitem> Bookingitems { get; set; } = new List<Bookingitem>();

    public virtual Event? Event { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User? User { get; set; }
}
