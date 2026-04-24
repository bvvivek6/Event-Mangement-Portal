using System;
using System.Collections.Generic;

namespace EventManagementDataAccessLayer.Models;

public partial class VwEventCapacity
{
    public Guid Eventid { get; set; }

    public int? Totalcapacity { get; set; }

    public int? Totalbooked { get; set; }

    public int? Availablecapacity { get; set; }
}
