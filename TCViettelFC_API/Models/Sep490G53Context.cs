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

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<CustomerTicket> CustomerTickets { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Match> Matches { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<NewsCategory> NewsCategories { get; set; }

    public virtual DbSet<OrderProduct> OrderProducts { get; set; }

    public virtual DbSet<OrderProductDetail> OrderProductDetails { get; set; }

    public virtual DbSet<OrderTicket> OrderTickets { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductDiscount> ProductDiscounts { get; set; }

    public virtual DbSet<ProductFile> ProductFiles { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Shipment> Shipments { get; set; }

    public virtual DbSet<Ticket> Tickets { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=SEP490_G53;User ID=sa;Password=sa;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Address__3214EC2767583937");

            entity.ToTable("Address");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.DetailedAddress).HasColumnName("detailed_address");
            entity.Property(e => e.District)
                .HasMaxLength(100)
                .HasColumnName("district");
            entity.Property(e => e.Fullname).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.Ward)
                .HasMaxLength(255)
                .HasColumnName("ward");

            entity.HasOne(d => d.User).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Address__UserID__3C69FB99");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC274B6F7E74");

            entity.ToTable("Category");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .HasColumnName("Category_name");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<CustomerTicket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC27A9D02253");

            entity.ToTable("Customer_Ticket");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Discount__3214EC275F233346");

            entity.ToTable("Discount");

            entity.Property(e => e.Id).HasColumnName("ID");
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
            entity.HasKey(e => e.Id).HasName("PK__Feedback__3214EC270DD43C91");

            entity.ToTable("Feedback");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Feedback__UserID__45F365D3");
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Matches__3214EC2790582D29");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.IsHome).HasColumnName("Is_home");
            entity.Property(e => e.LogoUrl).HasColumnName("Logo_Url");
            entity.Property(e => e.MatchDate)
                .HasColumnType("datetime")
                .HasColumnName("match_date");
            entity.Property(e => e.OpponentName)
                .HasMaxLength(255)
                .HasColumnName("opponent_name");
            entity.Property(e => e.StadiumName)
                .HasMaxLength(255)
                .HasColumnName("Stadium_name");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__News__3214EC27CA80DA7B");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.NewsCategoryId).HasColumnName("News_categoryID");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.NewsCategory).WithMany(p => p.News)
                .HasForeignKey(d => d.NewsCategoryId)
                .HasConstraintName("FK__News__News_categ__4316F928");

            entity.HasOne(d => d.User).WithMany(p => p.News)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__News__UserID__4222D4EF");
        });

        modelBuilder.Entity<NewsCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__News_Cat__3214EC27B9BBF044");

            entity.ToTable("News_Categories");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(255)
                .HasColumnName("Category_name");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("Created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.NewsCategories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__News_Cate__UserI__3F466844");
        });

        modelBuilder.Entity<OrderProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order_pr__3214EC27AB0F165F");

            entity.ToTable("Order_product");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AddressId).HasColumnName("AddressID");
            entity.Property(e => e.OrderCode).HasMaxLength(255);
            entity.Property(e => e.OrderDate)
                .HasColumnType("datetime")
                .HasColumnName("order_date");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_price");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Address).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK__Order_pro__Addre__571DF1D5");

            entity.HasOne(d => d.User).WithMany(p => p.OrderProducts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Order_pro__UserI__5629CD9C");
        });

        modelBuilder.Entity<OrderProductDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order_pr__3214EC27C136DF69");

            entity.ToTable("Order_product_details");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.OrderProductId).HasColumnName("Order_productId");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.ProductId).HasColumnName("Product_id");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.OrderProduct).WithMany(p => p.OrderProductDetails)
                .HasForeignKey(d => d.OrderProductId)
                .HasConstraintName("FK__Order_pro__Order__59FA5E80");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderProductDetails)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Order_pro__Produ__5AEE82B9");
        });

        modelBuilder.Entity<OrderTicket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order_ti__3214EC2783E2203F");

            entity.ToTable("Order_ticket");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Customer).WithMany(p => p.OrderTickets)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Order_tic__custo__628FA481");

            entity.HasOne(d => d.User).WithMany(p => p.OrderTickets)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Order_tic__UserI__619B8048");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payments__3214EC2727B5A2D1");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.OrderProductId).HasColumnName("Order_productId");
            entity.Property(e => e.OrderTicketId).HasColumnName("Order_ticketId");
            entity.Property(e => e.PaymentGateway)
                .HasMaxLength(250)
                .HasColumnName("payment_gateway");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_amount");

            entity.HasOne(d => d.OrderTicket).WithMany(p => p.Payments)
                .HasForeignKey(d => d.OrderTicketId)
                .HasConstraintName("FK__Payments__Order___693CA210");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product__3214EC27441DB830");

            entity.ToTable("Product");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Avartar).HasMaxLength(255);
            entity.Property(e => e.CategoryId).HasColumnName("category_ID");
            entity.Property(e => e.Color)
                .HasMaxLength(255)
                .HasColumnName("color");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Material)
                .HasMaxLength(255)
                .HasColumnName("material");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ProductName)
                .HasMaxLength(255)
                .HasColumnName("Product_name");
            entity.Property(e => e.Size)
                .HasMaxLength(255)
                .HasColumnName("size");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Product__categor__4AB81AF0");
        });

        modelBuilder.Entity<ProductDiscount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product___3214EC27866CB246");

            entity.ToTable("Product_discount");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DiscountId).HasColumnName("Discount_id");
            entity.Property(e => e.ProductId).HasColumnName("Product_id");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Discount).WithMany(p => p.ProductDiscounts)
                .HasForeignKey(d => d.DiscountId)
                .HasConstraintName("FK__Product_d__Disco__534D60F1");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductDiscounts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Product_d__Produ__52593CB8");
        });

        modelBuilder.Entity<ProductFile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product___3214EC2767AD5EE5");

            entity.ToTable("Product_files");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.FileName)
                .HasMaxLength(50)
                .HasColumnName("File_name");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .HasColumnName("File_path");
            entity.Property(e => e.ProductId).HasColumnName("Product_id");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductFiles)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Product_f__Produ__4D94879B");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3214EC27E6A455F3");

            entity.ToTable("Role");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.RoleName)
                .HasMaxLength(255)
                .HasColumnName("role_name");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<Shipment>(entity =>
        {
            entity.HasKey(e => e.ShipmentId).HasName("PK__Shipment__41466E5901E1A08C");

            entity.Property(e => e.ShipmentId).HasColumnName("shipment_id");
            entity.Property(e => e.DeliveryDate)
                .HasColumnType("datetime")
                .HasColumnName("delivery_date");
            entity.Property(e => e.OrderProductId).HasColumnName("Order_productId");
            entity.Property(e => e.ShipmentDate)
                .HasColumnType("datetime")
                .HasColumnName("shipment_date");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.OrderProduct).WithMany(p => p.Shipments)
                .HasForeignKey(d => d.OrderProductId)
                .HasConstraintName("FK__Shipments__Order__6C190EBB");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Ticket__3214EC274F398BAB");

            entity.ToTable("Ticket");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.MatchId).HasColumnName("Match_id");
            entity.Property(e => e.OrderTicketId).HasColumnName("Order_ticketID");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.SeatNumber)
                .HasMaxLength(50)
                .HasColumnName("seat_number");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Match).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.MatchId)
                .HasConstraintName("FK__Ticket__Match_id__656C112C");

            entity.HasOne(d => d.OrderTicket).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.OrderTicketId)
                .HasConstraintName("FK__Ticket__Order_ti__66603565");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCACC47829AB");

            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Password)
                .HasMaxLength(265)
                .IsUnicode(false);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.RoleId).HasColumnName("Role_id");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UserName).HasMaxLength(255);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Users__Role_id__398D8EEE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
