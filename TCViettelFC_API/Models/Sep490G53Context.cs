using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TCViettelFC_API.Models;

public partial class Sep490G53Context : DbContext
{
    public Sep490G53Context()
    {
    }

    public Sep490G53Context(DbContextOptions<Sep490G53Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Area> Areas { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Match> Matches { get; set; }

    public virtual DbSet<MatchAreaTicket> MatchAreaTickets { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<NewsCategory> NewsCategories { get; set; }

    public virtual DbSet<OrderProduct> OrderProducts { get; set; }

    public virtual DbSet<OrderProductDetail> OrderProductDetails { get; set; }

    public virtual DbSet<OrderedSuppItem> OrderedSuppItems { get; set; }

    public virtual DbSet<OrderedTicket> OrderedTickets { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductFile> ProductFiles { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Season> Seasons { get; set; }

    public virtual DbSet<Shipment> Shipments { get; set; }

    public virtual DbSet<SupplementaryItem> SupplementaryItems { get; set; }

    public virtual DbSet<TicketOrder> TicketOrders { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
        var config = new ConfigurationBuilder()
          .AddJsonFile("appsettings.json")
          .Build();

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(config.GetConnectionString("value"));
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__Address__CAA247C87582076E");

            entity.ToTable("Address");

            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DetailedAddress).HasColumnName("detailed_address");
            entity.Property(e => e.District)
                .HasMaxLength(100)
                .HasColumnName("district");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Ward)
                .HasMaxLength(100)
                .HasColumnName("ward");

            entity.HasOne(d => d.Customer).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Address__custome__403A8C7D");
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__areas__3213E83FCBFB1B16");

            entity.ToTable("areas");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Floor)
                .HasMaxLength(70)
                .HasColumnName("floor");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Section)
                .HasMaxLength(70)
                .HasColumnName("section");
            entity.Property(e => e.Stands)
                .HasMaxLength(70)
                .HasColumnName("stands");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__CD65CB851BEBD72A");

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(256)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__BDBE9EF9CF21D838");

            entity.ToTable("Discount");

            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.DiscountName)
                .HasMaxLength(255)
                .HasColumnName("discount_name");
            entity.Property(e => e.DiscountPercent).HasColumnName("discount_percent");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.ValidFrom)
                .HasColumnType("datetime")
                .HasColumnName("valid_from");
            entity.Property(e => e.ValidUntil)
                .HasColumnType("datetime")
                .HasColumnName("valid_until");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Feedback__3214EC270790CFD2");

            entity.ToTable("Feedback");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatorId).HasColumnName("creator_id");
            entity.Property(e => e.ResponderId).HasColumnName("responder_id");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Creator).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.CreatorId)
                .HasConstraintName("FK__Feedback__creato__5070F446");

            entity.HasOne(d => d.Responder).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.ResponderId)
                .HasConstraintName("FK__Feedback__respon__5165187F");
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Matches__3213E83F41A0443D");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsHome).HasColumnName("is_home");
            entity.Property(e => e.LogoUrl).HasColumnName("logo_url");
            entity.Property(e => e.MatchDate)
                .HasColumnType("datetime")
                .HasColumnName("match_date");
            entity.Property(e => e.OpponentName)
                .HasMaxLength(255)
                .HasColumnName("opponent_name");
            entity.Property(e => e.StadiumName)
                .HasMaxLength(255)
                .HasColumnName("stadium_name");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<MatchAreaTicket>(entity =>
        {
            entity.HasKey(e => new { e.MatchId, e.AreaId }).HasName("PK__Match_Ar__34FA1D759EB49A54");

            entity.ToTable("Match_Area_Tickets");

            entity.Property(e => e.MatchId).HasColumnName("match_id");
            entity.Property(e => e.AreaId).HasColumnName("area_id");
            entity.Property(e => e.AvailableSeats).HasColumnName("available_seats");

            entity.HasOne(d => d.Area).WithMany(p => p.MatchAreaTickets)
                .HasForeignKey(d => d.AreaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Match_Are__area___74AE54BC");

            entity.HasOne(d => d.Match).WithMany(p => p.MatchAreaTickets)
                .HasForeignKey(d => d.MatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Match_Are__match__73BA3083");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__News__3213E83FF0CB112F");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatorId).HasColumnName("creator_id");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.NewsCategoryId).HasColumnName("news_category_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");

            entity.HasOne(d => d.Creator).WithMany(p => p.News)
                .HasForeignKey(d => d.CreatorId)
                .HasConstraintName("FK__News__creator_id__4BAC3F29");

            entity.HasOne(d => d.NewsCategory).WithMany(p => p.News)
                .HasForeignKey(d => d.NewsCategoryId)
                .HasConstraintName("FK__News__news_categ__4CA06362");
        });

        modelBuilder.Entity<NewsCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__News_Cat__3213E83F3EB930FB");

            entity.ToTable("News_Categories");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .HasColumnName("category_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatorId).HasColumnName("creator_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Creator).WithMany(p => p.NewsCategories)
                .HasForeignKey(d => d.CreatorId)
                .HasConstraintName("FK__News_Cate__creat__47DBAE45");
        });

        modelBuilder.Entity<OrderProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order_Pr__3213E83F4B3E49B2");

            entity.ToTable("Order_Product");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.OrderCode)
                .HasMaxLength(255)
                .HasColumnName("order_code");
            entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_price");

            entity.HasOne(d => d.Address).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK__Order_Pro__addre__6C190EBB");

            entity.HasOne(d => d.Customer).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Order_Pro__custo__6B24EA82");
        });

        modelBuilder.Entity<OrderProductDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order_Pr__3213E83F739DBAF9");

            entity.ToTable("Order_Product_Details");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderProductId).HasColumnName("order_product_id");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Size)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("size");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.OrderProduct).WithMany(p => p.OrderProductDetails)
                .HasForeignKey(d => d.OrderProductId)
                .HasConstraintName("FK__Order_Pro__order__6FE99F9F");

            entity.HasOne(d => d.Player).WithMany(p => p.OrderProductDetails)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("FK_Order_Product_Details_Players");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderProductDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Order_Pro__produ__70DDC3D8");
        });

        modelBuilder.Entity<OrderedSuppItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ordered___3213E83F67A811DA");

            entity.ToTable("Ordered_supp_item");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Item).WithMany(p => p.OrderedSuppItems)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("FK__Ordered_s__item___01142BA1");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderedSuppItems)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Ordered_s__order__02084FDA");
        });

        modelBuilder.Entity<OrderedTicket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ordered___3213E83FBE03B041");

            entity.ToTable("Ordered_ticket");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AreaId).HasColumnName("area_id");
            entity.Property(e => e.MatchId).HasColumnName("match_id");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Area).WithMany(p => p.OrderedTickets)
                .HasForeignKey(d => d.AreaId)
                .HasConstraintName("FK__Ordered_t__area___7C4F7684");

            entity.HasOne(d => d.Match).WithMany(p => p.OrderedTickets)
                .HasForeignKey(d => d.MatchId)
                .HasConstraintName("FK__Ordered_t__match__7B5B524B");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderedTickets)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Ordered_t__order__7A672E12");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payments__3213E83F12CDFE47");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderProductId).HasColumnName("order_product_id");
            entity.Property(e => e.OrderTicketId).HasColumnName("order_ticket_id");
            entity.Property(e => e.PaymentGateway)
                .HasMaxLength(250)
                .HasColumnName("payment_gateway");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_amount");

            entity.HasOne(d => d.OrderProduct).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderProductId)
                .HasConstraintName("FK__Payments__order___05D8E0BE");

            entity.HasOne(d => d.OrderTicket).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderTicketId)
                .HasConstraintName("FK__Payments__order___04E4BC85");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.PlayerId).HasName("PK__Players__44DA120C0E2EFD39");

            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.JoinDate)
                .HasColumnType("datetime")
                .HasColumnName("join_date");
            entity.Property(e => e.OutDate)
                .HasColumnType("datetime")
                .HasColumnName("out_date");
            entity.Property(e => e.Position)
                .HasMaxLength(100)
                .HasColumnName("position");
            entity.Property(e => e.SeasonId).HasColumnName("season_id");
            entity.Property(e => e.ShirtNumber).HasColumnName("shirt_number");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Season).WithMany(p => p.Players)
                .HasForeignKey(d => d.SeasonId)
                .HasConstraintName("FK__Players__season___5629CD9C");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__47027DF5D5FB8F15");

            entity.ToTable("Product");

            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .HasColumnName("avatar");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Color)
                .HasMaxLength(255)
                .HasColumnName("color");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.Material)
                .HasMaxLength(255)
                .HasColumnName("material");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductName)
                .HasMaxLength(255)
                .HasColumnName("product_name");
            entity.Property(e => e.SeasonId).HasColumnName("season_id");
            entity.Property(e => e.Size)
                .HasMaxLength(255)
                .HasColumnName("size");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Product__categor__6383C8BA");

            entity.HasOne(d => d.Discount).WithMany(p => p.Products)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("FK__Product__discoun__628FA481");

            entity.HasOne(d => d.Season).WithMany(p => p.Products)
                .HasForeignKey(d => d.SeasonId)
                .HasConstraintName("FK__Product__season___6477ECF3");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__product___D54EE9B4C988CF71");

            entity.ToTable("product_category");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .HasColumnName("category_name");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<ProductFile>(entity =>
        {
            entity.HasKey(e => e.FileId).HasName("PK__Product___07D884C67364BC94");

            entity.ToTable("Product_file");

            entity.Property(e => e.FileId).HasColumnName("file_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.FileName)
                .HasMaxLength(50)
                .HasColumnName("file_name");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .HasColumnName("file_path");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductFiles)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Product_f__produ__68487DD7");
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__profile__AEBB701F08F59FD1");

            entity.ToTable("profile");

            entity.Property(e => e.ProfileId).HasColumnName("profile_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DateOfBirth)
                .HasColumnType("datetime")
                .HasColumnName("date_of_birth");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(50)
                .HasColumnName("gender");
            entity.Property(e => e.Note).HasColumnName("note");

            entity.HasOne(d => d.Customer).WithMany(p => p.Profiles)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__profile__custome__440B1D61");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__760965CCDDF787BE");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.RoleName)
                .HasMaxLength(255)
                .HasColumnName("role_name");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Season>(entity =>
        {
            entity.HasKey(e => e.SeasonId).HasName("PK__Seasons__0A99E331AB3AA34C");

            entity.Property(e => e.SeasonId).HasColumnName("season_id");
            entity.Property(e => e.EndYear)
                .HasColumnType("datetime")
                .HasColumnName("end_year");
            entity.Property(e => e.SeasonName)
                .HasMaxLength(255)
                .HasColumnName("season_name");
            entity.Property(e => e.StartYear)
                .HasColumnType("datetime")
                .HasColumnName("start_year");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.HasKey(e => e.ShipmentId).HasName("PK__Shipment__41466E598FF9013F");

            entity.Property(e => e.ShipmentId).HasColumnName("shipment_id");
            entity.Property(e => e.DeliveryDate)
                .HasColumnType("datetime")
                .HasColumnName("delivery_date");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ShipmentDate)
                .HasColumnType("datetime")
                .HasColumnName("shipment_date");
            entity.Property(e => e.ShipmentTrackingCode)
                .HasMaxLength(255)
                .IsFixedLength()
                .HasColumnName("shipment_tracking_code");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Order).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Shipments__order__08B54D69");
        });

        modelBuilder.Entity<SupplementaryItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Suppleme__52020FDD07C9FDD2");

            entity.ToTable("Supplementary_Items");

            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.ItemName)
                .HasMaxLength(255)
                .HasColumnName("item_name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<TicketOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ticket_O__3213E83F54D498C8");

            entity.ToTable("Ticket_Orders");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_amount");

            entity.HasOne(d => d.Customer).WithMany(p => p.TicketOrders)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Ticket_Or__custo__778AC167");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370F4945AB46");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E61649ADF4554").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.Password)
                .HasMaxLength(265)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__role_id__3B75D760");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
