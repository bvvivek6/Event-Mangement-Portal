using System;
using System.Collections.Generic;

namespace EventManagementDataAccessLayer.Models;

public partial class Category
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public bool? Iscustom { get; set; }

    public Guid? Createdby { get; set; }

    public virtual Organizer? CreatedbyNavigation { get; set; }

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
