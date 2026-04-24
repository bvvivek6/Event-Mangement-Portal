
if not exists (select * from sys.databases where name = 'eventplatformdb')
begin
   create database eventplatformdb;
end
go

use eventplatformdb;
go

-- ============================================================
-- USERS, ADMINS, ORGANIZERS TABLES
-- ============================================================

create table users (
   id UNIQUEIDENTIFIER DEFAULT NEWID() primary key,
   firstname nvarchar(100),
   lastname nvarchar(100),
   email nvarchar(150) unique not null,
   profileimageurl nvarchar(max),
   password nvarchar(255) not null,
   phone nvarchar(20),
   isblocked bit default 0,
   isverified bit default 0,
   createdat datetime2 default getutcdate(),
   updatedat datetime2 default getutcdate(),
   bio nvarchar(max)
);

create table admins (
   id UNIQUEIDENTIFIER DEFAULT NEWID() primary key,
   firstname nvarchar(100),
   lastname nvarchar(100),
   email nvarchar(150) unique not null,
   password nvarchar(255) not null,
   phone nvarchar(20),
   createdat datetime2 default getutcdate(),
   updatedat datetime2 default getutcdate()
);

create table organizers (
   id UNIQUEIDENTIFIER DEFAULT NEWID() primary key,
   firstname nvarchar(100),
   lastname nvarchar(100),
   email nvarchar(150) unique not null,
   password nvarchar(255) not null,
   organizationname nvarchar(255),
   bio nvarchar(max),
   profileimageurl nvarchar(max),
   bannerimageurl nvarchar(max),
   contactphone nvarchar(20),
   addressline nvarchar(255),
   city nvarchar(100),
   state nvarchar(100),
   country nvarchar(100),
   pincode nvarchar(20),
   website nvarchar(255),
   facebookurl nvarchar(255),
   twitterurl nvarchar(255),
   instagramurl nvarchar(255),
   linkedinurl nvarchar(255),
   rating decimal(3,2) default 0.00,
   isblocked bit default 0,
   isverified bit default 0,
   createdat datetime2 default getutcdate(),
   updatedat datetime2 default getutcdate()
);

-- ============================================================
-- WALLET TABLES
-- ============================================================

create table userwallets (
   id UNIQUEIDENTIFIER DEFAULT NEWID() primary key,
   userid UNIQUEIDENTIFIER unique,
   bankname nvarchar(100),
   accountnumber nvarchar(50) unique,
   ifsccode nvarchar(50),
   balance decimal(18,2) default 10000.00,
   createdat datetime2 default getutcdate(),
   updatedat datetime2 default getutcdate(),
   foreign key (userid) references users(id) on delete cascade
);

create table adminwallets (
   id UNIQUEIDENTIFIER DEFAULT NEWID() primary key,
   adminid UNIQUEIDENTIFIER unique,
   bankname nvarchar(100),
   accountnumber nvarchar(50) unique,
   ifsccode nvarchar(50),
   balance decimal(18,2) default 0.00,
   createdat datetime2 default getutcdate(),
   updatedat datetime2 default getutcdate(),
   foreign key (adminid) references admins(id) on delete cascade
);

create table organizerwallets (
   id UNIQUEIDENTIFIER DEFAULT NEWID() primary key,
   organizerid UNIQUEIDENTIFIER unique,
   bankname nvarchar(100),
   accountnumber nvarchar(50) unique,
   ifsccode nvarchar(50),
   balance decimal(18,2) default 0.00,
   createdat datetime2 default getutcdate(),
   updatedat datetime2 default getutcdate(),
   foreign key (organizerid) references organizers(id) on delete cascade
);

-- ============================================================
-- CATEGORIES & EVENTS
-- ============================================================

create table categories (
   id int identity(1,1) primary key,
   name nvarchar(100) unique,
   iscustom bit default 0,
   createdby UNIQUEIDENTIFIER,
   foreign key (createdby) references organizers(id) on delete set null
);

insert into categories (name, iscustom) values
('music & concerts', 0);

create table events (
   id UNIQUEIDENTIFIER DEFAULT NEWID() primary key,
   organizerid UNIQUEIDENTIFIER,
   title nvarchar(255),
   description nvarchar(max),
   categoryid int,
   location nvarchar(255),
   city nvarchar(100),
   state nvarchar(100),
   country nvarchar(100),
   eventdate datetime2,
   bookingstatus nvarchar(20) check (bookingstatus in ('active','paused','closed')) default 'active',
   approvalstatus nvarchar(20) check (approvalstatus in ('pending','approved','rejected')) default 'pending',
   totalcapacity int,
   istrending bit default 0,
   discountpercentage decimal(5,2) default 0.00,
   additionaldetails nvarchar(max) check (additionaldetails is null or isjson(additionaldetails) > 0),
   createdat datetime2 default getutcdate(),
   updatedat datetime2 default getutcdate(),
   isdeleted bit default 0,
   isPaused bit default 0,
   isRefundable bit default 0,
   refundPercentage decimal(5,2) default 0.00,
   foreign key (organizerid) references organizers(id) on delete cascade,
   foreign key (categoryid) references categories(id)
);

create index idx_event_search on events(city, eventdate, categoryid);
create index idx_events_organizer on events(organizerid);

-- ============================================================
-- EVENT IMAGES & TICKET TIERS
-- ============================================================

create table eventimages (
   id UNIQUEIDENTIFIER DEFAULT NEWID() primary key,
   eventid UNIQUEIDENTIFIER,
   imageurl nvarchar(max),
   isprimary bit default 0,
   displayorder int default 0,
   createdat datetime2 default getutcdate(),
   foreign key (eventid) references events(id) on delete cascade
);

create table tickettiers (
   id UNIQUEIDENTIFIER DEFAULT NEWID() primary key,
   eventid UNIQUEIDENTIFIER,
   name nvarchar(100),
   price decimal(10,2) check (price > 0),
   totalquantity int check (totalquantity > 0),
   updatedat datetime2 default getutcdate(),
   foreign key (eventid) references events(id) on delete cascade
);

create index idx_tickettiers_event on tickettiers(eventid);

-- ============================================================
-- BOOKINGS & BOOKING ITEMS (SIMPLIFIED - No agent_session_id needed)
-- ============================================================

create table bookings (
   id UNIQUEIDENTIFIER DEFAULT NEWID() primary key,
   userid UNIQUEIDENTIFIER,
   eventid UNIQUEIDENTIFIER,
   totalamount decimal(10,2),
   status nvarchar(20) check (status in ('pending','confirmed','cancelled','failed')) default 'pending',
   bookingreference nvarchar(100) unique,
   idempotencykey nvarchar(255) unique null,
   createdat datetime2 default getutcdate(),
   updatedat datetime2 default getutcdate(),
   isdeleted bit default 0,
   
   -- Agentic booking indicator (simplified)
   bookingsource nvarchar(50) default 'user_ui' check (bookingsource in ('user_ui', 'ai_agent')),
   
   foreign key (userid) references users(id) on delete cascade,
   foreign key (eventid) references events(id) on delete cascade
);

create index idx_booking_user on bookings(userid);
create index idx_bookings_status on bookings(status, createdat);
create index idx_bookings_event on bookings(eventid, status);
create index idx_bookings_source on bookings(bookingsource);

create table bookingitems (
   id UNIQUEIDENTIFIER DEFAULT NEWID() primary key,
   bookingid UNIQUEIDENTIFIER,
   tickettierid UNIQUEIDENTIFIER,
   quantity int,
   price decimal(10,2),
   foreign key (bookingid) references bookings(id) on delete cascade,
   foreign key (tickettierid) references tickettiers(id)
);

create index idx_bookingitems_booking on bookingitems(bookingid);

-- ============================================================
-- PAYMENTS & REFUNDS
-- ============================================================

create table payments (
   id UNIQUEIDENTIFIER DEFAULT NEWID() primary key,
   bookingid UNIQUEIDENTIFIER,
   paymentgateway nvarchar(50) default 'dummy_wallet',
   transactionid nvarchar(255) unique,
   amount decimal(10,2),
   platformfee decimal(10,2) default 0.00,
   organizeramount decimal(10,2) default 0.00,
   status nvarchar(20) check (status in ('success','failed','pending')),
   idempotencykey nvarchar(255) unique,
   paymentmethod nvarchar(50) default 'wallet',
   createdat datetime2 default getutcdate(),
   updatedat datetime2 default getutcdate(),
   foreign key (bookingid) references bookings(id) on delete cascade
);

create index idx_payment_booking on payments(bookingid);
create index idx_payments_booking_status on payments(bookingid, status);
create index idx_payments_transaction on payments(transactionid);

create table refunds (
   id UNIQUEIDENTIFIER DEFAULT NEWID() primary key,
   paymentid UNIQUEIDENTIFIER,
   amount decimal(10,2),
   status nvarchar(20) check (status in ('initiated','completed','failed')),
   createdat datetime2 default getutcdate(),
   foreign key (paymentid) references payments(id) on delete cascade
);

create table refundpolicies (
   id UNIQUEIDENTIFIER DEFAULT NEWID() primary key,
   eventid UNIQUEIDENTIFIER,
   daysbefore int,
   refundpercentage int check (refundpercentage between 0 and 100),
   isApproved bit default 0,
   foreign key (eventid) references events(id) on delete cascade
);

-- ============================================================
-- REVIEWS
-- ============================================================

create table reviews (
   id UNIQUEIDENTIFIER DEFAULT NEWID() primary key,
   userid UNIQUEIDENTIFIER,
   eventid UNIQUEIDENTIFIER,
   bookingid UNIQUEIDENTIFIER not null,
   rating int check (rating between 1 and 5),
   comment nvarchar(max),
   createdat datetime2 default getutcdate(),
   constraint uq_user_event unique (userid, eventid),
   foreign key (userid) references users(id) on delete cascade,
   foreign key (eventid) references events(id) on delete cascade,
   foreign key (bookingid) references bookings(id) on delete cascade
);

-- ============================================================
-- NOTIFICATIONS
-- ============================================================

create table usernotifications (
   id UNIQUEIDENTIFIER DEFAULT NEWID() primary key,
   userid UNIQUEIDENTIFIER,
   title nvarchar(255),
   message nvarchar(max),
   isread bit default 0,
   createdat datetime2 default getutcdate(),
   foreign key (userid) references users(id) on delete cascade
);

create table organizernotifications (
   id UNIQUEIDENTIFIER DEFAULT NEWID() primary key,
   organizerid UNIQUEIDENTIFIER,
   title nvarchar(255),
   message nvarchar(max),
   isread bit default 0,
   createdat datetime2 default getutcdate(),
   foreign key (organizerid) references organizers(id) on delete cascade
);

create table adminnotifications (
   id UNIQUEIDENTIFIER DEFAULT NEWID() primary key,
   adminid UNIQUEIDENTIFIER,
   title nvarchar(255),
   message nvarchar(max),
   isread bit default 0,
   createdat datetime2 default getutcdate(),
   foreign key (adminid) references admins(id) on delete cascade
);

create index idx_usernotifications_user on usernotifications(userid);
create index idx_usernotifications_read on usernotifications(userid, isread);
create index idx_organizernotifications_org on organizernotifications(organizerid);
create index idx_organizernotifications_read on organizernotifications(organizerid, isread);
create index idx_adminnotifications_admin on adminnotifications(adminid);

-- ============================================================
-- COMPLAINTS
-- ============================================================

create table complaints (
   id UNIQUEIDENTIFIER DEFAULT NEWID() primary key,
   organizationid UNIQUEIDENTIFIER,
   title nvarchar(150),
   description nvarchar(max),
   createdat datetime2 default getutcdate(),
   foreign key (organizationid) references organizers(id) on delete cascade
);

-- ============================================================
-- SIMPLIFIED AI LOGGING (One-shot booking only)
-- ============================================================

-- Track AI booking requests and results (not real-time conversation)
create table ai_booking_requests (
   id UNIQUEIDENTIFIER DEFAULT NEWID() primary key,
   userid UNIQUEIDENTIFIER not null,
   
   -- User's raw input message
   user_message nvarchar(max) not null,
   
   -- What LLM extracted
   extracted_eventname nvarchar(255),
   extracted_ticketcount int,
   extracted_eventdate varchar(10),
   extracted_city nvarchar(100),
   extracted_tickettier nvarchar(100),
   extraction_confidence decimal(3,2),
   
   -- Validation result
   validation_status nvarchar(50) check (validation_status in ('valid', 'invalid', 'error')),
   validation_errors nvarchar(max), -- JSON array of error messages
   matched_eventid UNIQUEIDENTIFIER,
   
   -- Booking result
   booking_status nvarchar(20) check (booking_status in ('success', 'payment_failed', 'booking_failed', 'validation_failed')),
   created_bookingid UNIQUEIDENTIFIER,
   created_bookingref nvarchar(100),
   payment_amount decimal(10,2),
   
   -- Timing
   processing_time_ms int,
   createdat datetime2 default getutcdate(),
   
   foreign key (userid) references users(id) on delete cascade,
   foreign key (matched_eventid) references events(id),
   foreign key (created_bookingid) references bookings(id) on delete set null
);

create index idx_ai_booking_user on ai_booking_requests(userid, createdat);
create index idx_ai_booking_status on ai_booking_requests(validation_status, booking_status);

-- ============================================================
-- VIEWS (Dynamic Capacity Calculations)
-- ============================================================

-- VIEW: Event Capacities
go
create view vw_event_capacities as
select 
   e.id as eventid,
   e.totalcapacity,
   coalesce(sum(bi.quantity), 0) as totalbooked,
   (e.totalcapacity - coalesce(sum(bi.quantity), 0)) as availablecapacity
from events e
left join bookings b on e.id = b.eventid and b.status in ('pending', 'confirmed')
left join bookingitems bi on b.id = bi.bookingid
group by e.id, e.totalcapacity;
go

-- VIEW: Ticket Tier Capacities
create view vw_tickettier_capacities as
select 
   t.id as tickettierid,
   t.eventid,
   t.totalquantity,
   coalesce(sum(bi.quantity), 0) as totalbooked,
   (t.totalquantity - coalesce(sum(bi.quantity), 0)) as availablequantity
from tickettiers t
left join bookingitems bi on t.id = bi.tickettierid
left join bookings b on bi.bookingid = b.id and b.status in ('pending', 'confirmed')
group by t.id, t.eventid, t.totalquantity;
go

-- ============================================================
-- STORED PROCEDURE: Book Tickets (With Race Condition Protection)
-- ============================================================

create procedure sp_BookTickets
   @UserId UNIQUEIDENTIFIER,
   @EventId UNIQUEIDENTIFIER,
   @TicketTierId UNIQUEIDENTIFIER,
   @QuantityToBook int,
   @TotalAmount decimal(10,2),
   @BookingReference nvarchar(100),
   @IdempotencyKey nvarchar(255) = null,
   @BookingSource nvarchar(50) = 'user_ui'
as
begin
   set nocount on;
   set xact_abort on;

   begin try
       begin transaction;
           -- 1) Acquire an UPDLOCK exclusively on the specific Ticket Tier row
           declare @TotalAllowed int;
           select @TotalAllowed = totalquantity 
           from tickettiers with (updlock, rowlock)
           where id = @TicketTierId;

           if @TotalAllowed is null
           begin
               raiserror('Ticket tier not found.', 16, 1);
               rollback transaction;
               return;
           end

           -- 2) Calculate how many tickets are currently reserved
           declare @CurrentlyBooked int;
           select @CurrentlyBooked = coalesce(sum(bi.quantity), 0)
           from bookingitems bi
           inner join bookings b on bi.bookingid = b.id
           where bi.tickettierid = @TicketTierId and b.status in ('pending', 'confirmed');

           -- 3) Check if requested quantity fits within dynamic remaining capacity
           if (@TotalAllowed - @CurrentlyBooked) < @QuantityToBook
           begin
               raiserror('Insufficient tickets available for this tier.', 16, 1);
               rollback transaction;
               return;
           end

           -- 4) Insert the root booking
           declare @NewBookingId UNIQUEIDENTIFIER;
           insert into bookings (
               userid, eventid, totalamount, status, bookingreference, idempotencykey, bookingsource
           )
           values (
               @UserId, @EventId, @TotalAmount, 'pending', @BookingReference, @IdempotencyKey, @BookingSource
           );
           set @NewBookingId = scope_identity();

           -- 5) Insert the connected booking item
           declare @TierPrice decimal(10,2);
           select @TierPrice = price from tickettiers where id = @TicketTierId;

           insert into bookingitems (
               bookingid, tickettierid, quantity, price
           )
           values (
               @NewBookingId, @TicketTierId, @QuantityToBook, @TierPrice
           );

       commit transaction;
       
       -- Return the booking ID
       select @NewBookingId as BookingId;
   end try
   begin catch
       if @@trancount > 0 rollback transaction;
       
       declare @ErrMsg nvarchar(4000) = error_message();
       declare @ErrSeverity int = error_severity();
       declare @ErrState int = error_state();
       
       raiserror(@ErrMsg, @ErrSeverity, @ErrState);
   end catch
end;
go

-- ============================================================
-- STORED PROCEDURE: Get Event with Capacities
-- ============================================================

create procedure sp_GetEventWithCapacity
   @EventId UNIQUEIDENTIFIER
as
begin
   set nocount on;

   select 
       e.id,
       e.title,
       e.description,
       e.city,
       e.eventdate,
       e.totalcapacity,
       ec.availablecapacity,
       ec.totalbooked
   from events e
   left join vw_event_capacities ec on e.id = ec.eventid
   where e.id = @EventId;
end;
go

-- ============================================================
-- STORED PROCEDURE: Search Events
-- ============================================================

create procedure sp_SearchEvents
   @Title nvarchar(255) = null,
   @City nvarchar(100) = null,
   @EventDate datetime2 = null,
   @CategoryId int = null
as
begin
   set nocount on;

   select 
       e.id,
       e.title,
       e.description,
       e.city,
       e.eventdate,
       e.totalcapacity,
       ec.availablecapacity,
       c.name as categoryname
   from events e
   left join vw_event_capacities ec on e.id = ec.eventid
   left join categories c on e.categoryid = c.id
   where 
       e.bookingstatus = 'active'
       and e.approvalstatus = 'approved'
       and e.isdeleted = 0
       and (@Title is null or e.title like '%' + @Title + '%')
       and (@City is null or e.city = @City)
       and (@EventDate is null or cast(e.eventdate as date) = cast(@EventDate as date))
       and (@CategoryId is null or e.categoryid = @CategoryId)
   order by e.eventdate asc;
end;
go

-- ============================================================
-- STORED PROCEDURE: Create Payment
-- ============================================================

create procedure sp_CreatePayment
   @BookingId UNIQUEIDENTIFIER,
   @Amount decimal(10,2),
   @TransactionId nvarchar(255),
   @IdempotencyKey nvarchar(255),
   @PaymentMethod nvarchar(50) = 'wallet'
as
begin
   set nocount on;
   set xact_abort on;

   begin try
       begin transaction;

           -- Calculate fees (10% platform fee, 90% to organizer)
           declare @PlatformFee decimal(10,2) = @Amount * 0.10;
           declare @OrganizerAmount decimal(10,2) = @Amount * 0.90;

           insert into payments (
               bookingid, 
               transactionid, 
               amount, 
               platformfee, 
               organizeramount,
               status,
               idempotencykey,
               paymentmethod
           )
           values (
               @BookingId,
               @TransactionId,
               @Amount,
               @PlatformFee,
               @OrganizerAmount,
               'success',
               @IdempotencyKey,
               @PaymentMethod
           );

           -- Update booking status to confirmed
           update bookings 
           set status = 'confirmed', updatedat = getutcdate()
           where id = @BookingId;

       commit transaction;
   end try
   begin catch
       if @@trancount > 0 rollback transaction;
       
       declare @ErrMsg nvarchar(4000) = error_message();
       declare @ErrSeverity int = error_severity();
       declare @ErrState int = error_state();
       
       raiserror(@ErrMsg, @ErrSeverity, @ErrState);
   end catch
end;
go

-- ============================================================
-- STORED PROCEDURE: Log AI Booking Request
-- ============================================================

create procedure sp_LogAIBookingRequest
   @UserId UNIQUEIDENTIFIER,
   @UserMessage nvarchar(max),
   @ExtractedEventName nvarchar(255),
   @ExtractedTicketCount int,
   @ExtractedEventDate varchar(10),
   @ExtractedCity nvarchar(100),
   @ExtractedTicketTier nvarchar(100),
   @ExtractionConfidence decimal(3,2),
   @ValidationStatus nvarchar(50),
   @ValidationErrors nvarchar(max),
   @MatchedEventId UNIQUEIDENTIFIER,
   @BookingStatus nvarchar(20),
   @CreatedBookingId UNIQUEIDENTIFIER,
   @CreatedBookingRef nvarchar(100),
   @PaymentAmount decimal(10,2),
   @ProcessingTimeMs int
as
begin
   set nocount on;

   insert into ai_booking_requests (
       userid,
       user_message,
       extracted_eventname,
       extracted_ticketcount,
       extracted_eventdate,
       extracted_city,
       extracted_tickettier,
       extraction_confidence,
       validation_status,
       validation_errors,
       matched_eventid,
       booking_status,
       created_bookingid,
       created_bookingref,
       payment_amount,
       processing_time_ms
   )
   values (
       @UserId,
       @UserMessage,
       @ExtractedEventName,
       @ExtractedTicketCount,
       @ExtractedEventDate,
       @ExtractedCity,
       @ExtractedTicketTier,
       @ExtractionConfidence,
       @ValidationStatus,
       @ValidationErrors,
       @MatchedEventId,
       @BookingStatus,
       @CreatedBookingId,
       @CreatedBookingRef,
       @PaymentAmount,
       @ProcessingTimeMs
   );
end;
go

-- ============================================================
-- Database initialization complete
-- ============================================================


select * from users