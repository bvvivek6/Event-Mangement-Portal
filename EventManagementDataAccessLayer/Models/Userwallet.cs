using System;
using System.Collections.Generic;

namespace EventManagementDataAccessLayer.Models;

public partial class Userwallet
{
    public Guid Id { get; set; }

    public Guid? Userid { get; set; }

    public string? Bankname { get; set; }

    public string? Accountnumber { get; set; }

    public string? Ifsccode { get; set; }

    public decimal? Balance { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual User? User { get; set; }
}
