using System;
using System.Collections.Generic;

namespace EventManagementDataAccessLayer.Models;

public partial class VwTickettierCapacity
{
    public Guid Tickettierid { get; set; }

    public Guid? Eventid { get; set; }

    public int? Totalquantity { get; set; }

    public int? Totalbooked { get; set; }

    public int? Availablequantity { get; set; }
}
