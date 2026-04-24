using System;
using System.Collections.Generic;

namespace EventManagementDataAccessLayer.Models;

public partial class Organizer
{
    public Guid Id { get; set; }

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Organizationname { get; set; }

    public string? Bio { get; set; }

    public string? Profileimageurl { get; set; }

    public string? Bannerimageurl { get; set; }

    public string? Contactphone { get; set; }

    public string? Addressline { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public string? Pincode { get; set; }

    public string? Website { get; set; }

    public string? Facebookurl { get; set; }

    public string? Twitterurl { get; set; }

    public string? Instagramurl { get; set; }

    public string? Linkedinurl { get; set; }

    public decimal? Rating { get; set; }

    public bool? Isblocked { get; set; }

    public bool? Isverified { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    public virtual Organizerwallet? Organizerwallet { get; set; }
}
