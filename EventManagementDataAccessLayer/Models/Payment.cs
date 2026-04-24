using System;
using System.Collections.Generic;

namespace EventManagementDataAccessLayer.Models;

public partial class Payment
{
    public Guid Id { get; set; }

    public Guid? Bookingid { get; set; }

    public string? Paymentgateway { get; set; }

    public string? Transactionid { get; set; }

    public decimal? Amount { get; set; }

    public decimal? Platformfee { get; set; }

    public decimal? Organizeramount { get; set; }

    public string? Status { get; set; }

    public string? Idempotencykey { get; set; }

    public string? Paymentmethod { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual Booking? Booking { get; set; }

    public virtual ICollection<Refund> Refunds { get; set; } = new List<Refund>();
}
