using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Nguu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    ColorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColorName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Colors__8DA7676D46814297", x => x.ColorID);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    MaterialID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Material__C5061317129D5857", x => x.MaterialID);
                });

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    PostID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Summary = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    CategoryID = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ProductID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Post__AA12603832CFDCAE", x => x.PostID);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Trademark = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductC__19093A2BB1696ED5", x => x.CategoryID);
                });

            migrationBuilder.CreateTable(
                name: "Promotion",
                columns: table => new
                {
                    PromotionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                    DiscountType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DiscountValue = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Promotion", x => x.PromotionID);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__8AFACE3AD721524A", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "Sizes",
                columns: table => new
                {
                    SizeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SizeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Sizes__83BD095A4656A287", x => x.SizeID);
                });

            migrationBuilder.CreateTable(
                name: "Staffs",
                columns: table => new
                {
                    StaffID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    Gender = table.Column<bool>(type: "bit", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HireDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdateAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    RoleID = table.Column<int>(type: "int", nullable: false, defaultValue: 2)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Staffs__96D4AAF74B920184", x => x.StaffID);
                });

            migrationBuilder.CreateTable(
                name: "Transport",
                columns: table => new
                {
                    TransportID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransportMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Transpor__19E9A17DB524B435", x => x.TransportID);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdateAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Products__B40CC6EDC32A859E", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK__Products__Catego__68487DD7",
                        column: x => x.CategoryID,
                        principalTable: "ProductCategories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: true, defaultValue: 3),
                    CreateAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Accounts__349DA5866CC4CC0D", x => x.AccountID);
                    table.ForeignKey(
                        name: "FK__Accounts__RoleID__123EB7A3",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProductDetails",
                columns: table => new
                {
                    ProductDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: true),
                    SizeID = table.Column<int>(type: "int", nullable: true),
                    ColorID = table.Column<int>(type: "int", nullable: true),
                    MaterialID = table.Column<int>(type: "int", nullable: true),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ProductD__3C8DD694357756D8", x => x.ProductDetailID);
                    table.ForeignKey(
                        name: "FK__ProductDe__Color__7E37BEF6",
                        column: x => x.ColorID,
                        principalTable: "Colors",
                        principalColumn: "ColorID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK__ProductDe__Mater__7F2BE32F",
                        column: x => x.MaterialID,
                        principalTable: "Materials",
                        principalColumn: "MaterialID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK__ProductDe__Produ__7C4F7684",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__ProductDe__SizeI__7D439ABD",
                        column: x => x.SizeID,
                        principalTable: "Sizes",
                        principalColumn: "SizeID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Phone = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    Gender = table.Column<bool>(type: "bit", nullable: false),
                    DOB = table.Column<DateOnly>(type: "date", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    UpdateAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    RankMember = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Point = table.Column<int>(type: "int", nullable: true),
                    AccountID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__A4AE64B8D6C914C3", x => x.CustomerID);
                    table.ForeignKey(
                        name: "FK_Customers_Accounts_AccountID",
                        column: x => x.AccountID,
                        principalTable: "Accounts",
                        principalColumn: "AccountID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    BillID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StaffID = table.Column<int>(type: "int", nullable: true),
                    CustomerID = table.Column<int>(type: "int", nullable: true),
                    TransportID = table.Column<int>(type: "int", nullable: true),
                    CreateAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "(getdate())"),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatRate = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    VatAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountReceived = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ChangeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Bills__11F2FC4A635723A7", x => x.BillID);
                    table.ForeignKey(
                        name: "FK_Bills_Customers",
                        column: x => x.CustomerID,
                        principalTable: "Customers",
                        principalColumn: "CustomerID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Bills_Staffs",
                        column: x => x.StaffID,
                        principalTable: "Staffs",
                        principalColumn: "StaffID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Bills_Transports",
                        column: x => x.TransportID,
                        principalTable: "Transport",
                        principalColumn: "TransportID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    CartID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Carts__5B65BF97D8A6F2E3", x => x.CartID);
                    table.ForeignKey(
                        name: "FK_Carts_Customers_CustomerId_New",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerID");
                });

            migrationBuilder.CreateTable(
                name: "BillDetails",
                columns: table => new
                {
                    BillDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BillID = table.Column<int>(type: "int", nullable: false),
                    ProductDetailID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(29,2)", nullable: true, computedColumnSql: "([Quantity]*[UnitPrice])", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BillDeta__793CAF7565BBFBEC", x => x.BillDetailID);
                    table.ForeignKey(
                        name: "FK__BillDetai__BillI__6E01572D",
                        column: x => x.BillID,
                        principalTable: "Bills",
                        principalColumn: "BillID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__BillDetai__Produ__6EF57B66",
                        column: x => x.ProductDetailID,
                        principalTable: "ProductDetails",
                        principalColumn: "ProductDetailID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CartDetails",
                columns: table => new
                {
                    CartDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartID = table.Column<int>(type: "int", nullable: false),
                    ProductDetailID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CartDeta__4E3E04AD8D7C2F1E", x => x.CartDetailID);
                    table.ForeignKey(
                        name: "FK__CartDeta__CartID__3C69FB99",
                        column: x => x.CartID,
                        principalTable: "Carts",
                        principalColumn: "CartID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__CartDeta__ProductDeta__3D5E1FD2",
                        column: x => x.ProductDetailID,
                        principalTable: "ProductDetails",
                        principalColumn: "ProductDetailID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "News",
                columns: new[] { "Id", "Author", "Category", "Content", "CreatedAt", "Date", "Image", "Status", "Summary", "Thumbnail", "Title" },
                values: new object[,]
                {
                    { 1, "Admin POLY", "tin-tuc", "Hè 2025 đánh dấu sự trở lại mạnh mẽ của POLY...", new DateTime(2025, 11, 19, 10, 30, 51, 376, DateTimeKind.Local).AddTicks(4522), new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://cdn.brvn.vn/editor/2023/07/A42_333084-fashion-show-6_1688631273.jpg", "draft", "Đón chào mùa hè với BST mới đầy năng động và trẻ trung từ POLY", null, "POLY ra mắt bộ sưu tập Hè 2025" },
                    { 2, "StylePOLY", "tin-tuc", "Phong cách Y2K đình đám những năm 2000 đang quay trở lại mạnh mẽ...", new DateTime(2025, 11, 19, 10, 30, 51, 376, DateTimeKind.Local).AddTicks(4532), new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://thieuhoa.com.vn/wp-content/uploads/2023/03/BfFzk1NZAuPK0bZMRFLFgS0vIfxHWls0Wne7C4PS.webp", "draft", "Xu hướng Y2K đang làm mưa làm gió trong làng thời trang", null, "Phong cách Y2K đang trở lại" },
                    { 3, "Creative Team", "tin-tuc", "POLY hợp tác với 5 nghệ sĩ graffiti nổi tiếng...", new DateTime(2025, 11, 19, 10, 30, 51, 376, DateTimeKind.Local).AddTicks(4535), new DateTime(2024, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://img.vietcetera.com/uploads/images/09-oct-2020/untitled-1.jpg", "draft", "Dự án hợp tác độc quyền với các nghệ sĩ đường phố Việt Nam", null, "POLY hợp tác với nghệ sĩ local" },
                    { 4, "Green Team", "tin-tuc", "POLY cam kết chuyển đổi 50% quy trình sản xuất...", new DateTime(2025, 11, 19, 10, 30, 51, 376, DateTimeKind.Local).AddTicks(4536), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://btnmt.1cdn.vn/2020/06/28/tui-vai-bao-ve-moi-truong.jpg", "draft", "Chuyển đổi quy trình sản xuất sang hướng thân thiện với môi trường", null, "POLY cam kết thời trang bền vững" },
                    { 5, "Marketing POLY", "tin-tuc", "Tiếp nối thành công của hai cửa hàng tại Đống Đa và Hà Đông...", new DateTime(2025, 11, 19, 10, 30, 51, 376, DateTimeKind.Local).AddTicks(4538), new DateTime(2023, 12, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "https://images.squarespace-cdn.com/content/v1/591fd77d29687fd09cca478b/1555546030336-YXVPG30KTCM92JW89UTL/AdobeStock_139559217.jpeg", "draft", "Cửa hàng thứ 3 của POLY sẽ khai trương vào tháng 7/2024", null, "Mở rộng cửa hàng POLY tại Cầu Giấy" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_RoleID",
                table: "Accounts",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "UQ__Accounts__536C85E4C3F5D1EA",
                table: "Accounts",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BillDetails_BillID",
                table: "BillDetails",
                column: "BillID");

            migrationBuilder.CreateIndex(
                name: "IX_BillDetails_ProductDetailID",
                table: "BillDetails",
                column: "ProductDetailID");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_CustomerID",
                table: "Bills",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_StaffID",
                table: "Bills",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "IX_Bills_TransportID",
                table: "Bills",
                column: "TransportID");

            migrationBuilder.CreateIndex(
                name: "IX_CartDetails_CartID",
                table: "CartDetails",
                column: "CartID");

            migrationBuilder.CreateIndex(
                name: "IX_CartDetails_ProductDetailID",
                table: "CartDetails",
                column: "ProductDetailID");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CustomerId",
                table: "Carts",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "UQ__Colors__C71A5A7BAD5758A9",
                table: "Colors",
                column: "ColorName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AccountID",
                table: "Customers",
                column: "AccountID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AccountID1",
                table: "Customers",
                column: "AccountID");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Phone",
                table: "Customers",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Customer__5C7E359E9B9944BC",
                table: "Customers",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Customer__A9D10534BCF9D50C",
                table: "Customers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Material__9C87053CEF6D0D8B",
                table: "Materials",
                column: "MaterialName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__ProductC__8517B2E0B01BA20C",
                table: "ProductCategories",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_ColorID",
                table: "ProductDetails",
                column: "ColorID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_MaterialID",
                table: "ProductDetails",
                column: "MaterialID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_ProductID",
                table: "ProductDetails",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_SizeID",
                table: "ProductDetails",
                column: "SizeID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryID",
                table: "Products",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "UQ__Roles__8A2B6160EFFD3336",
                table: "Roles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Sizes__619EFC3E5028ADBC",
                table: "Sizes",
                column: "SizeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Staffs__5C7E359ECB14BC4D",
                table: "Staffs",
                column: "Phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Staffs__A9D105342EA7D79F",
                table: "Staffs",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillDetails");

            migrationBuilder.DropTable(
                name: "CartDetails");

            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropTable(
                name: "Post");

            migrationBuilder.DropTable(
                name: "Promotion");

            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "ProductDetails");

            migrationBuilder.DropTable(
                name: "Staffs");

            migrationBuilder.DropTable(
                name: "Transport");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Sizes");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "ProductCategories");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
