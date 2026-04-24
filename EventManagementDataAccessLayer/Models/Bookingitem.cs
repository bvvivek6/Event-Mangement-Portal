using System;
using System.Collections.Generic;

namespace EventManagementDataAccessLayer.Models;

public partial class Bookingitem
{
    public Guid Id { get; set; }

    public Guid? Bookingid { get; set; }

    public Guid? Tickettierid { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual Tickettier? Tickettier { get; set; }
}
