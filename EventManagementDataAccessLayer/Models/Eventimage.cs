using System;
using System.Collections.Generic;

namespace EventManagementDataAccessLayer.Models;

public partial class Eventimage
{
    public Guid Id { get; set; }

    public Guid? Eventid { get; set; }

    public string? Imageurl { get; set; }

    public bool? Isprimary { get; set; }

    public int? Displayorder { get; set; }

    public DateTime? Createdat { get; set; }

    public virtual Event? Event { get; set; }
}
