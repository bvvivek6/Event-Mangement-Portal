using System;
using System.Collections.Generic;

namespace EventManagementDataAccessLayer.Models;

public partial class Refund
{
    public Guid Id { get; set; }

    public Guid? Paymentid { get; set; }

    public decimal? Amount { get; set; }

    public string? Status { get; set; }

    public DateTime? Createdat { get; set; }

    public virtual Payment? Payment { get; set; }
}
