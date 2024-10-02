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

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<NewsCategory> NewsCategories { get; set; }

    public virtual DbSet<OrderProduct> OrderProducts { get; set; }

    public virtual DbSet<OrderProductDetail> OrderProductDetails { get; set; }

    public virtual DbSet<OrderTicket> OrderTickets { get; set; }

    public virtual DbSet<OrderTicketConcession> OrderTicketConcessions { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Player> Players { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductDiscount> ProductDiscounts { get; set; }

    public virtual DbSet<ProductFile> ProductFiles { get; set; }

    public virtual DbSet<Profile> Profiles { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Season> Seasons { get; set; }

    public virtual DbSet<Shipment> Shipments { get; set; }

    public virtual DbSet<SupplementaryItem> SupplementaryItems { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=QUAN;Initial Catalog=SEP490_G53;User ID=sa;Password=11122002;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.AddressId).HasName("PK__Address__CAA247C89AADDB01");

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

            entity.HasOne(d => d.Customer).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Address__custome__403A8C7D");
        });

        modelBuilder.Entity<Area>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__areas__3213E83F76740103");

            entity.ToTable("areas");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NumberOfChair).HasColumnName("number_of_chair");
            entity.Property(e => e.NumberOfTicket).HasColumnName("number_of_ticket");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Rows)
                .HasMaxLength(70)
                .HasColumnName("rows");
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
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__CD65CB85B9FE2896");

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.DiscountId).HasName("PK__Discount__BDBE9EF9A709CAD5");

            entity.ToTable("Discount");

            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.DiscountName)
                .HasMaxLength(255)
                .HasColumnName("discount_name");
            entity.Property(e => e.DiscountPercent)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("discount_percent");
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
            entity.HasKey(e => e.Id).HasName("PK__Feedback__3214EC272B36910F");

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
            entity.HasKey(e => e.Id).HasName("PK__Matches__3213E83FD9A8EC21");

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

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__News__3213E83FF8907A67");

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
            entity.HasKey(e => e.Id).HasName("PK__News_Cat__3213E83F7F422F86");

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
            entity.HasKey(e => e.Id).HasName("PK__Order_Pr__3213E83FE584A9E2");

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
                .HasConstraintName("FK__Order_Pro__addre__6EF57B66");

            entity.HasOne(d => d.Customer).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Order_Pro__custo__6E01572D");
        });

        modelBuilder.Entity<OrderProductDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order_Pr__3213E83FFBA2A1C1");

            entity.ToTable("Order_Product_Details");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.OrderProductId).HasColumnName("order_product_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.OrderProduct).WithMany(p => p.OrderProductDetails)
                .HasForeignKey(d => d.OrderProductId)
                .HasConstraintName("FK__Order_Pro__order__71D1E811");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderProductDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Order_Pro__produ__72C60C4A");
        });

        modelBuilder.Entity<OrderTicket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order_ti__3213E83F4A1684E9");

            entity.ToTable("Order_ticket");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_amount");

            entity.HasOne(d => d.Customer).WithMany(p => p.OrderTickets)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Order_tic__custo__797309D9");

            entity.HasOne(d => d.Ticket).WithMany(p => p.OrderTickets)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("FK__Order_tic__ticke__7A672E12");
        });

        modelBuilder.Entity<OrderTicketConcession>(entity =>
        {
            entity.HasKey(e => new { e.OrderTicketId, e.ConcessionId }).HasName("PK__Order_Ti__84BB91841CD61C65");

            entity.ToTable("Order_Ticket_Concessions");

            entity.Property(e => e.OrderTicketId).HasColumnName("order_ticket_id");
            entity.Property(e => e.ConcessionId).HasColumnName("concession_id");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Concession).WithMany(p => p.OrderTicketConcessions)
                .HasForeignKey(d => d.ConcessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order_Tic__conce__00200768");

            entity.HasOne(d => d.OrderTicket).WithMany(p => p.OrderTicketConcessions)
                .HasForeignKey(d => d.OrderTicketId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order_Tic__order__7F2BE32F");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payments__3213E83FFC80EB1E");

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
                .HasConstraintName("FK__Payments__order___03F0984C");

            entity.HasOne(d => d.OrderTicket).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderTicketId)
                .HasConstraintName("FK__Payments__order___02FC7413");
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.PlayerId).HasName("PK__Players__44DA120C00F152A6");

            entity.Property(e => e.PlayerId).HasColumnName("player_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.JoinDate)
                .HasColumnType("datetime")
                .HasColumnName("join_date");
            entity.Property(e => e.Position)
                .HasMaxLength(100)
                .HasColumnName("position");
            entity.Property(e => e.ShirtNumber).HasColumnName("shirt_number");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__47027DF56E51C183");

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
            entity.Property(e => e.Material)
                .HasMaxLength(255)
                .HasColumnName("material");
            entity.Property(e => e.PlayerId).HasColumnName("player_id");
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
                .HasConstraintName("FK__Product__categor__60A75C0F");

            entity.HasOne(d => d.Player).WithMany(p => p.Products)
                .HasForeignKey(d => d.PlayerId)
                .HasConstraintName("FK__Product__player___5FB337D6");

            entity.HasOne(d => d.Season).WithMany(p => p.Products)
                .HasForeignKey(d => d.SeasonId)
                .HasConstraintName("FK__Product__season___619B8048");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__product___D54EE9B4DB2883C6");

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

        modelBuilder.Entity<ProductDiscount>(entity =>
        {
            entity.HasKey(e => e.ProductDiscountId).HasName("PK__Product___79012509465B3BCA");

            entity.ToTable("Product_discount");

            entity.Property(e => e.ProductDiscountId).HasColumnName("product_discount_id");
            entity.Property(e => e.DiscountId).HasColumnName("discount_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Discount).WithMany(p => p.ProductDiscounts)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("FK__Product_d__disco__6B24EA82");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductDiscounts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Product_d__produ__6A30C649");
        });

        modelBuilder.Entity<ProductFile>(entity =>
        {
            entity.HasKey(e => e.FileId).HasName("PK__Product___07D884C614B135D1");

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
                .HasConstraintName("FK__Product_f__produ__656C112C");
        });

        modelBuilder.Entity<Profile>(entity =>
        {
            entity.HasKey(e => e.ProfileId).HasName("PK__profile__AEBB701FDA165CC8");

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
            entity.HasKey(e => e.RoleId).HasName("PK__Role__760965CCDD7215E4");

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
            entity.HasKey(e => e.SeasonId).HasName("PK__Seasons__0A99E331DB598266");

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
            entity.HasKey(e => e.ShipmentId).HasName("PK__Shipment__41466E5907775C85");

            entity.Property(e => e.ShipmentId).HasColumnName("shipment_id");
            entity.Property(e => e.DeliveryDate)
                .HasColumnType("datetime")
                .HasColumnName("delivery_date");
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.ShipmentDate)
                .HasColumnType("datetime")
                .HasColumnName("shipment_date");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Order).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.OrderId)
                .HasConstraintName("FK__Shipments__order__06CD04F7");
        });

        modelBuilder.Entity<SupplementaryItem>(entity =>
        {
            entity.HasKey(e => e.ItemId).HasName("PK__Suppleme__52020FDDD9945C9D");

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

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.TicketId).HasName("PK__Ticket__D596F96BE00CE823");

            entity.ToTable("Ticket");

            entity.Property(e => e.TicketId).HasColumnName("ticket_id");
            entity.Property(e => e.AreaId).HasColumnName("area_id");
            entity.Property(e => e.MatchId).HasColumnName("match_id");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Area).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.AreaId)
                .HasConstraintName("FK__Ticket__area_id__76969D2E");

            entity.HasOne(d => d.Match).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.MatchId)
                .HasConstraintName("FK__Ticket__match_id__75A278F5");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370F30ABC93C");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E6164AC53C4A1").IsUnique();

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
