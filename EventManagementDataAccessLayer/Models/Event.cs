using System;
using System.Collections.Generic;

namespace EventManagementDataAccessLayer.Models;

public partial class Event
{
    public Guid Id { get; set; }

    public Guid? Organizerid { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public int? Categoryid { get; set; }

    public string? Location { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public DateTime? Eventdate { get; set; }

    public string? Bookingstatus { get; set; }

    public string? Approvalstatus { get; set; }

    public int? Totalcapacity { get; set; }

    public bool? Istrending { get; set; }

    public decimal? Discountpercentage { get; set; }

    public string? Additionaldetails { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public bool? Isdeleted { get; set; }

    public bool? IsPaused { get; set; }

    public bool? IsRefundable { get; set; }

    public decimal? RefundPercentage { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Category? Category { get; set; }

    public virtual ICollection<Eventimage> Eventimages { get; set; } = new List<Eventimage>();

    public virtual Organizer? Organizer { get; set; }

    public virtual ICollection<Refundpolicy> Refundpolicies { get; set; } = new List<Refundpolicy>();

    public virtual ICollection<Tickettier> Tickettiers { get; set; } = new List<Tickettier>();
}
