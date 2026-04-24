using System;
using System.Collections.Generic;

namespace EventManagementDataAccessLayer.Models;

public partial class Tickettier
{
    public Guid Id { get; set; }

    public Guid? Eventid { get; set; }

    public string? Name { get; set; }

    public decimal? Price { get; set; }

    public int? Totalquantity { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual ICollection<Bookingitem> Bookingitems { get; set; } = new List<Bookingitem>();

    public virtual Event? Event { get; set; }
}
