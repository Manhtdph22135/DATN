using System;
using System.Collections.Generic;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Context;

public partial class DbContextShop : DbContext
{
    public DbContextShop()
    {
    }

    public DbContextShop(DbContextOptions<DbContextShop> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<BillDetail> BillDetails { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartDetail> CartDetails { get; set; }

    public virtual DbSet<Color> Colors { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductDetail> ProductDetails { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Size> Sizes { get; set; }

    public virtual DbSet<Staff> Staffs { get; set; }

    public virtual DbSet<Transport> Transports { get; set; }

    public virtual DbSet<News> News { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=DATN;Integrated Security=True;Encrypt=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("Accounts");
            entity.HasKey(e => e.AccountId).HasName("PK__Accounts__349DA5866CC4CC0D");

            entity.Property(e => e.CreateAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.RoleId).HasDefaultValue(3);

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Accounts__RoleID__123EB7A3");

            // Cấu hình quan hệ 1-1 với Customer
            entity.HasOne(d => d.Customer)
                .WithOne(p => p.Account)
                .HasForeignKey<Customer>(c => c.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.BillId).HasName("PK__Bills__11F2FC4A635723A7");

            entity.Property(e => e.CreateAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Customer).WithMany(p => p.Bills)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Bills_Customers");

            entity.HasOne(d => d.Staff).WithMany(p => p.Bills)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Bills_Staffs");

            entity.HasOne(d => d.Transport).WithMany(p => p.Bills)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Bills_Transports");
        });

        modelBuilder.Entity<BillDetail>(entity =>
        {
            entity.HasKey(e => e.BillDetailId).HasName("PK__BillDeta__793CAF7565BBFBEC");

            entity.Property(e => e.Total).HasComputedColumnSql("([Quantity]*[UnitPrice])", true);

            entity.HasOne(d => d.Bill).WithMany(p => p.BillDetails).HasConstraintName("FK__BillDetai__BillI__6E01572D");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.BillDetails).HasConstraintName("FK__BillDetai__Produ__6EF57B66");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.CartId).HasName("PK__Carts__5B65BF97D8A6F2E3");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Customer).WithMany(p => p.Carts).HasConstraintName("FK_Carts_Customers_CustomerId_New");
        });

        modelBuilder.Entity<CartDetail>(entity =>
        {
            entity.HasKey(e => e.CartDetailId).HasName("PK__CartDeta__4E3E04AD8D7C2F1E");

            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Cart).WithMany(p => p.CartDetails).HasConstraintName("FK__CartDeta__CartID__3C69FB99");

            entity.HasOne(d => d.ProductDetail).WithMany(p => p.CartDetails).HasConstraintName("FK__CartDeta__ProductDeta__3D5E1FD2");
        });

        modelBuilder.Entity<Color>(entity =>
        {
            entity.HasKey(e => e.ColorId).HasName("PK__Colors__8DA7676D46814297");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");
            entity.HasKey(e => e.CustomerId).HasName("PK__Customer__A4AE64B8D6C914C3");

            entity.Property(e => e.CreateAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdateAt).HasDefaultValueSql("(getdate())");

            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.Phone).IsUnique();
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.MaterialId).HasName("PK__Material__C5061317129D5857");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Post__AA12603832CFDCAE");

            entity.Property(e => e.PostId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Products__B40CC6EDC32A859E");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue(true);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Products__Catego__68487DD7");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__ProductC__19093A2BB1696ED5");
        });

        modelBuilder.Entity<ProductDetail>(entity =>
        {
            entity.HasKey(e => e.ProductDetailId).HasName("PK__ProductD__3C8DD694357756D8");

            entity.HasOne(d => d.Color).WithMany(p => p.ProductDetails)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__ProductDe__Color__7E37BEF6");

            entity.HasOne(d => d.Material).WithMany(p => p.ProductDetails)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__ProductDe__Mater__7F2BE32F");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductDetails)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__ProductDe__Produ__7C4F7684");

            entity.HasOne(d => d.Size).WithMany(p => p.ProductDetails)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__ProductDe__SizeI__7D439ABD");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.ToTable("Promotion");

            entity.Property(e => e.PromotionId)
                .HasColumnName("PromotionID")
                .UseIdentityColumn();


            entity.Property(e => e.DiscountType).HasMaxLength(50);

        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3AD721524A");
        });

        modelBuilder.Entity<Size>(entity =>
        {
            entity.HasKey(e => e.SizeId).HasName("PK__Sizes__83BD095A4656A287");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__Staffs__96D4AAF74B920184");

            entity.Property(e => e.CreateAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.RoleId).HasDefaultValue(2);
        });

        modelBuilder.Entity<Transport>(entity =>
        {
            entity.HasKey(e => e.TransportId).HasName("PK__Transpor__19E9A17DB524B435");
        });

        OnModelCreatingPartial(modelBuilder);

        modelBuilder.Entity<News>().HasData(
        new News { Id = 1, Title = "POLY ra mắt bộ sưu tập Hè 2025", Summary = "Đón chào mùa hè với BST mới đầy năng động và trẻ trung từ POLY", Content = "Hè 2025 đánh dấu sự trở lại mạnh mẽ của POLY...", Image = "https://cdn.brvn.vn/editor/2023/07/A42_333084-fashion-show-6_1688631273.jpg", Date = new DateTime(2024, 1, 15), Author = "Admin POLY" },
        new News { Id = 2, Title = "Phong cách Y2K đang trở lại", Summary = "Xu hướng Y2K đang làm mưa làm gió trong làng thời trang", Content = "Phong cách Y2K đình đám những năm 2000 đang quay trở lại mạnh mẽ...", Image = "https://thieuhoa.com.vn/wp-content/uploads/2023/03/BfFzk1NZAuPK0bZMRFLFgS0vIfxHWls0Wne7C4PS.webp", Date = new DateTime(2024, 1, 10), Author = "StylePOLY" },
        new News { Id = 3, Title = "POLY hợp tác với nghệ sĩ local", Summary = "Dự án hợp tác độc quyền với các nghệ sĩ đường phố Việt Nam", Content = "POLY hợp tác với 5 nghệ sĩ graffiti nổi tiếng...", Image = "https://img.vietcetera.com/uploads/images/09-oct-2020/untitled-1.jpg", Date = new DateTime(2024, 1, 5), Author = "Creative Team" },
        new News { Id = 4, Title = "POLY cam kết thời trang bền vững", Summary = "Chuyển đổi quy trình sản xuất sang hướng thân thiện với môi trường", Content = "POLY cam kết chuyển đổi 50% quy trình sản xuất...", Image = "https://btnmt.1cdn.vn/2020/06/28/tui-vai-bao-ve-moi-truong.jpg", Date = new DateTime(2024, 1, 1), Author = "Green Team" },
        new News { Id = 5, Title = "Mở rộng cửa hàng POLY tại Cầu Giấy", Summary = "Cửa hàng thứ 3 của POLY sẽ khai trương vào tháng 7/2024", Content = "Tiếp nối thành công của hai cửa hàng tại Đống Đa và Hà Đông...", Image = "https://images.squarespace-cdn.com/content/v1/591fd77d29687fd09cca478b/1555546030336-YXVPG30KTCM92JW89UTL/AdobeStock_139559217.jpeg", Date = new DateTime(2023, 12, 28), Author = "Marketing POLY" }
    );
    }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
