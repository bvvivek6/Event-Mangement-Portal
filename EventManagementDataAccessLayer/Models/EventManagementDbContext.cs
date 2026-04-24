using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EventManagementDataAccessLayer.Models;

public partial class EventManagementDbContext : DbContext
{
    public EventManagementDbContext()
    {
    }

    public EventManagementDbContext(DbContextOptions<EventManagementDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Adminwallet> Adminwallets { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Bookingitem> Bookingitems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Eventimage> Eventimages { get; set; }

    public virtual DbSet<Organizer> Organizers { get; set; }

    public virtual DbSet<Organizerwallet> Organizerwallets { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Refund> Refunds { get; set; }

    public virtual DbSet<Refundpolicy> Refundpolicies { get; set; }

    public virtual DbSet<Tickettier> Tickettiers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userwallet> Userwallets { get; set; }

    public virtual DbSet<VwEventCapacity> VwEventCapacities { get; set; }

    public virtual DbSet<VwTickettierCapacity> VwTickettierCapacities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.\\SQLEXPRESS;Database=eventplatformdb;Integrated Security=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__admins__3213E83FD29A95C2");

            entity.ToTable("admins");

            entity.HasIndex(e => e.Email, "UQ__admins__AB6E6164EC676BB8").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("createdat");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(100)
                .HasColumnName("firstname");
            entity.Property(e => e.Lastname)
                .HasMaxLength(100)
                .HasColumnName("lastname");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("updatedat");
        });

        modelBuilder.Entity<Adminwallet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__adminwal__3213E83FCAAC85D1");

            entity.ToTable("adminwallets");

            entity.HasIndex(e => e.Adminid, "UQ__adminwal__AD040D7FE8AA67FA").IsUnique();

            entity.HasIndex(e => e.Accountnumber, "UQ__adminwal__E762EC1A6D616502").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Accountnumber)
                .HasMaxLength(50)
                .HasColumnName("accountnumber");
            entity.Property(e => e.Adminid).HasColumnName("adminid");
            entity.Property(e => e.Balance)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("balance");
            entity.Property(e => e.Bankname)
                .HasMaxLength(100)
                .HasColumnName("bankname");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("createdat");
            entity.Property(e => e.Ifsccode)
                .HasMaxLength(50)
                .HasColumnName("ifsccode");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Admin).WithOne(p => p.Adminwallet)
                .HasForeignKey<Adminwallet>(d => d.Adminid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__adminwall__admin__571DF1D5");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__bookings__3213E83FAC6C7AC1");

            entity.ToTable("bookings");

            entity.HasIndex(e => e.Bookingreference, "UQ__bookings__5082087F16AD9D8A").IsUnique();

            entity.HasIndex(e => e.Idempotencykey, "UQ__bookings__E007A4E4B227E962").IsUnique();

            entity.HasIndex(e => e.Userid, "idx_booking_user");

            entity.HasIndex(e => new { e.Eventid, e.Status }, "idx_bookings_event");

            entity.HasIndex(e => e.Bookingsource, "idx_bookings_source");

            entity.HasIndex(e => new { e.Status, e.Createdat }, "idx_bookings_status");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bookingreference)
                .HasMaxLength(100)
                .HasColumnName("bookingreference");
            entity.Property(e => e.Bookingsource)
                .HasMaxLength(50)
                .HasDefaultValue("user_ui")
                .HasColumnName("bookingsource");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("createdat");
            entity.Property(e => e.Eventid).HasColumnName("eventid");
            entity.Property(e => e.Idempotencykey)
                .HasMaxLength(255)
                .HasColumnName("idempotencykey");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("pending")
                .HasColumnName("status");
            entity.Property(e => e.Totalamount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("totalamount");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("updatedat");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Event).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.Eventid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__bookings__eventi__0B91BA14");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__bookings__userid__0A9D95DB");
        });

        modelBuilder.Entity<Bookingitem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__bookingi__3213E83FE1CF9952");

            entity.ToTable("bookingitems");

            entity.HasIndex(e => e.Bookingid, "idx_bookingitems_booking");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bookingid).HasColumnName("bookingid");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Tickettierid).HasColumnName("tickettierid");

            entity.HasOne(d => d.Booking).WithMany(p => p.Bookingitems)
                .HasForeignKey(d => d.Bookingid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__bookingit__booki__0E6E26BF");

            entity.HasOne(d => d.Tickettier).WithMany(p => p.Bookingitems)
                .HasForeignKey(d => d.Tickettierid)
                .HasConstraintName("FK__bookingit__ticke__0F624AF8");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__categori__3213E83FB887420B");

            entity.ToTable("categories");

            entity.HasIndex(e => e.Name, "UQ__categori__72E12F1BC544CD0A").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdby).HasColumnName("createdby");
            entity.Property(e => e.Iscustom)
                .HasDefaultValue(false)
                .HasColumnName("iscustom");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");

            entity.HasOne(d => d.CreatedbyNavigation).WithMany(p => p.Categories)
                .HasForeignKey(d => d.Createdby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__categorie__creat__6383C8BA");
        });

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__events__3213E83FEAEEB38D");

            entity.ToTable("events");

            entity.HasIndex(e => new { e.City, e.Eventdate, e.Categoryid }, "idx_event_search");

            entity.HasIndex(e => e.Organizerid, "idx_events_organizer");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Additionaldetails).HasColumnName("additionaldetails");
            entity.Property(e => e.Approvalstatus)
                .HasMaxLength(20)
                .HasDefaultValue("pending")
                .HasColumnName("approvalstatus");
            entity.Property(e => e.Bookingstatus)
                .HasMaxLength(20)
                .HasDefaultValue("active")
                .HasColumnName("bookingstatus");
            entity.Property(e => e.Categoryid).HasColumnName("categoryid");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("createdat");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Discountpercentage)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("discountpercentage");
            entity.Property(e => e.Eventdate).HasColumnName("eventdate");
            entity.Property(e => e.IsPaused)
                .HasDefaultValue(false)
                .HasColumnName("isPaused");
            entity.Property(e => e.IsRefundable)
                .HasDefaultValue(false)
                .HasColumnName("isRefundable");
            entity.Property(e => e.Isdeleted)
                .HasDefaultValue(false)
                .HasColumnName("isdeleted");
            entity.Property(e => e.Istrending)
                .HasDefaultValue(false)
                .HasColumnName("istrending");
            entity.Property(e => e.Location)
                .HasMaxLength(255)
                .HasColumnName("location");
            entity.Property(e => e.Organizerid).HasColumnName("organizerid");
            entity.Property(e => e.RefundPercentage)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("refundPercentage");
            entity.Property(e => e.State)
                .HasMaxLength(100)
                .HasColumnName("state");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Totalcapacity).HasColumnName("totalcapacity");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Category).WithMany(p => p.Events)
                .HasForeignKey(d => d.Categoryid)
                .HasConstraintName("FK__events__category__73BA3083");

            entity.HasOne(d => d.Organizer).WithMany(p => p.Events)
                .HasForeignKey(d => d.Organizerid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__events__organize__72C60C4A");
        });

        modelBuilder.Entity<Eventimage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__eventima__3213E83F8B736D03");

            entity.ToTable("eventimages");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("createdat");
            entity.Property(e => e.Displayorder)
                .HasDefaultValue(0)
                .HasColumnName("displayorder");
            entity.Property(e => e.Eventid).HasColumnName("eventid");
            entity.Property(e => e.Imageurl).HasColumnName("imageurl");
            entity.Property(e => e.Isprimary)
                .HasDefaultValue(false)
                .HasColumnName("isprimary");

            entity.HasOne(d => d.Event).WithMany(p => p.Eventimages)
                .HasForeignKey(d => d.Eventid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__eventimag__event__797309D9");
        });

        modelBuilder.Entity<Organizer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__organize__3213E83FDEE8D72B");

            entity.ToTable("organizers");

            entity.HasIndex(e => e.Email, "UQ__organize__AB6E6164CBE7BE3B").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Addressline)
                .HasMaxLength(255)
                .HasColumnName("addressline");
            entity.Property(e => e.Bannerimageurl).HasColumnName("bannerimageurl");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Contactphone)
                .HasMaxLength(20)
                .HasColumnName("contactphone");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("createdat");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Facebookurl)
                .HasMaxLength(255)
                .HasColumnName("facebookurl");
            entity.Property(e => e.Firstname)
                .HasMaxLength(100)
                .HasColumnName("firstname");
            entity.Property(e => e.Instagramurl)
                .HasMaxLength(255)
                .HasColumnName("instagramurl");
            entity.Property(e => e.Isblocked)
                .HasDefaultValue(false)
                .HasColumnName("isblocked");
            entity.Property(e => e.Isverified)
                .HasDefaultValue(false)
                .HasColumnName("isverified");
            entity.Property(e => e.Lastname)
                .HasMaxLength(100)
                .HasColumnName("lastname");
            entity.Property(e => e.Linkedinurl)
                .HasMaxLength(255)
                .HasColumnName("linkedinurl");
            entity.Property(e => e.Organizationname)
                .HasMaxLength(255)
                .HasColumnName("organizationname");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Pincode)
                .HasMaxLength(20)
                .HasColumnName("pincode");
            entity.Property(e => e.Profileimageurl).HasColumnName("profileimageurl");
            entity.Property(e => e.Rating)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("rating");
            entity.Property(e => e.State)
                .HasMaxLength(100)
                .HasColumnName("state");
            entity.Property(e => e.Twitterurl)
                .HasMaxLength(255)
                .HasColumnName("twitterurl");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("updatedat");
            entity.Property(e => e.Website)
                .HasMaxLength(255)
                .HasColumnName("website");
        });

        modelBuilder.Entity<Organizerwallet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__organize__3213E83FEBFFC961");

            entity.ToTable("organizerwallets");

            entity.HasIndex(e => e.Organizerid, "UQ__organize__8BE1664531546D6C").IsUnique();

            entity.HasIndex(e => e.Accountnumber, "UQ__organize__E762EC1A38733ED0").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Accountnumber)
                .HasMaxLength(50)
                .HasColumnName("accountnumber");
            entity.Property(e => e.Balance)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("balance");
            entity.Property(e => e.Bankname)
                .HasMaxLength(100)
                .HasColumnName("bankname");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("createdat");
            entity.Property(e => e.Ifsccode)
                .HasMaxLength(50)
                .HasColumnName("ifsccode");
            entity.Property(e => e.Organizerid).HasColumnName("organizerid");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Organizer).WithOne(p => p.Organizerwallet)
                .HasForeignKey<Organizerwallet>(d => d.Organizerid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__organizer__organ__5EBF139D");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__payments__3213E83F7FD2A669");

            entity.ToTable("payments");

            entity.HasIndex(e => e.Transactionid, "UQ__payments__9B52C2FB557E8D17").IsUnique();

            entity.HasIndex(e => e.Idempotencykey, "UQ__payments__E007A4E424F68F1B").IsUnique();

            entity.HasIndex(e => e.Bookingid, "idx_payment_booking");

            entity.HasIndex(e => new { e.Bookingid, e.Status }, "idx_payments_booking_status");

            entity.HasIndex(e => e.Transactionid, "idx_payments_transaction");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.Bookingid).HasColumnName("bookingid");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("createdat");
            entity.Property(e => e.Idempotencykey)
                .HasMaxLength(255)
                .HasColumnName("idempotencykey");
            entity.Property(e => e.Organizeramount)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("organizeramount");
            entity.Property(e => e.Paymentgateway)
                .HasMaxLength(50)
                .HasDefaultValue("dummy_wallet")
                .HasColumnName("paymentgateway");
            entity.Property(e => e.Paymentmethod)
                .HasMaxLength(50)
                .HasDefaultValue("wallet")
                .HasColumnName("paymentmethod");
            entity.Property(e => e.Platformfee)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("platformfee");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");
            entity.Property(e => e.Transactionid)
                .HasMaxLength(255)
                .HasColumnName("transactionid");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .HasForeignKey(d => d.Bookingid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__payments__bookin__1AD3FDA4");
        });

        modelBuilder.Entity<Refund>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__refunds__3213E83FD63F492F");

            entity.ToTable("refunds");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("createdat");
            entity.Property(e => e.Paymentid).HasColumnName("paymentid");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.Payment).WithMany(p => p.Refunds)
                .HasForeignKey(d => d.Paymentid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__refunds__payment__1F98B2C1");
        });

        modelBuilder.Entity<Refundpolicy>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__refundpo__3213E83F05DD0F18");

            entity.ToTable("refundpolicies");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Daysbefore).HasColumnName("daysbefore");
            entity.Property(e => e.Eventid).HasColumnName("eventid");
            entity.Property(e => e.IsApproved)
                .HasDefaultValue(false)
                .HasColumnName("isApproved");
            entity.Property(e => e.Refundpercentage).HasColumnName("refundpercentage");

            entity.HasOne(d => d.Event).WithMany(p => p.Refundpolicies)
                .HasForeignKey(d => d.Eventid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__refundpol__event__245D67DE");
        });

        modelBuilder.Entity<Tickettier>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ticketti__3213E83FBCE004F9");

            entity.ToTable("tickettiers");

            entity.HasIndex(e => e.Eventid, "idx_tickettiers_event");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Eventid).HasColumnName("eventid");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Totalquantity).HasColumnName("totalquantity");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("updatedat");

            entity.HasOne(d => d.Event).WithMany(p => p.Tickettiers)
                .HasForeignKey(d => d.Eventid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__tickettie__event__7F2BE32F");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83FBC18AEDE");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "UQ__users__AB6E61642AFBEECC").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Bio).HasColumnName("bio");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("createdat");
            entity.Property(e => e.Email)
                .HasMaxLength(150)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(100)
                .HasColumnName("firstname");
            entity.Property(e => e.Isblocked)
                .HasDefaultValue(false)
                .HasColumnName("isblocked");
            entity.Property(e => e.Isverified)
                .HasDefaultValue(false)
                .HasColumnName("isverified");
            entity.Property(e => e.Lastname)
                .HasMaxLength(100)
                .HasColumnName("lastname");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Profileimageurl).HasColumnName("profileimageurl");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("updatedat");
        });

        modelBuilder.Entity<Userwallet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__userwall__3213E83F72D3A3D7");

            entity.ToTable("userwallets");

            entity.HasIndex(e => e.Userid, "UQ__userwall__CBA1B256123853A2").IsUnique();

            entity.HasIndex(e => e.Accountnumber, "UQ__userwall__E762EC1A5497C38E").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Accountnumber)
                .HasMaxLength(50)
                .HasColumnName("accountnumber");
            entity.Property(e => e.Balance)
                .HasDefaultValue(10000.00m)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("balance");
            entity.Property(e => e.Bankname)
                .HasMaxLength(100)
                .HasColumnName("bankname");
            entity.Property(e => e.Createdat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("createdat");
            entity.Property(e => e.Ifsccode)
                .HasMaxLength(50)
                .HasColumnName("ifsccode");
            entity.Property(e => e.Updatedat)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("updatedat");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.User).WithOne(p => p.Userwallet)
                .HasForeignKey<Userwallet>(d => d.Userid)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__userwalle__useri__4F7CD00D");
        });

        modelBuilder.Entity<VwEventCapacity>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_event_capacities");

            entity.Property(e => e.Availablecapacity).HasColumnName("availablecapacity");
            entity.Property(e => e.Eventid).HasColumnName("eventid");
            entity.Property(e => e.Totalbooked).HasColumnName("totalbooked");
            entity.Property(e => e.Totalcapacity).HasColumnName("totalcapacity");
        });

        modelBuilder.Entity<VwTickettierCapacity>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("vw_tickettier_capacities");

            entity.Property(e => e.Availablequantity).HasColumnName("availablequantity");
            entity.Property(e => e.Eventid).HasColumnName("eventid");
            entity.Property(e => e.Tickettierid).HasColumnName("tickettierid");
            entity.Property(e => e.Totalbooked).HasColumnName("totalbooked");
            entity.Property(e => e.Totalquantity).HasColumnName("totalquantity");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
