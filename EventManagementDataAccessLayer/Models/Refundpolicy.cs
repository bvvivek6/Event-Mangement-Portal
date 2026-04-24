using System;
using System.Collections.Generic;

namespace EventManagementDataAccessLayer.Models;

public partial class Refundpolicy
{
    public Guid Id { get; set; }

    public Guid? Eventid { get; set; }

    public int? Daysbefore { get; set; }

    public int? Refundpercentage { get; set; }

    public bool? IsApproved { get; set; }

    public virtual Event? Event { get; set; }
}
