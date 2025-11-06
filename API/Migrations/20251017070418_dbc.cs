    using Microsoft.EntityFrameworkCore.Migrations;

    #nullable disable

    namespace API.Migrations
    {
        /// <inheritdoc />
        public partial class dbc : Migration
        {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Drop các FK hiện có (tên thật trên DB)
            migrationBuilder.DropForeignKey(
                name: "FK__Bills__CustomerI__619B8048",
                table: "Bills");

            migrationBuilder.DropForeignKey(
                name: "FK__Bills__StaffID__60A75C0F",
                table: "Bills");

            migrationBuilder.DropForeignKey(
                name: "FK__Bills__Transport__628FA481",
                table: "Bills");

            // 2) (Tuỳ chọn) đảm bảo 3 cột FK cho phép null nếu bạn dùng SET NULL
            migrationBuilder.AlterColumn<int>(
                name: "CustomerID",
                table: "Bills",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StaffID",
                table: "Bills",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TransportID",
                table: "Bills",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // 3) Tạo index cho các FK nếu chưa có
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Bills_CustomerID' AND object_id = OBJECT_ID(N'[dbo].[Bills]'))
    CREATE INDEX [IX_Bills_CustomerID] ON [dbo].[Bills]([CustomerID]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Bills_StaffID' AND object_id = OBJECT_ID(N'[dbo].[Bills]'))
    CREATE INDEX [IX_Bills_StaffID] ON [dbo].[Bills]([StaffID]);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Bills_TransportID' AND object_id = OBJECT_ID(N'[dbo].[Bills]'))
    CREATE INDEX [IX_Bills_TransportID] ON [dbo].[Bills]([TransportID]);
");

            // 4) Add lại 3 FK chuẩn, tên “đẹp”, ON DELETE SET NULL
            migrationBuilder.AddForeignKey(
                name: "FK_Bills_Customers",
                table: "Bills",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_Staffs",
                table: "Bills",
                column: "StaffID",
                principalTable: "Staffs",
                principalColumn: "StaffID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Bills_Transports",
                table: "Bills",
                column: "TransportID",
                principalTable: "Transport",  // đổi thành "Transports" nếu bảng bạn là số nhiều
                principalColumn: "TransportID",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Gỡ 3 FK “đẹp”
            migrationBuilder.DropForeignKey(name: "FK_Bills_Customers", table: "Bills");
            migrationBuilder.DropForeignKey(name: "FK_Bills_Staffs", table: "Bills");
            migrationBuilder.DropForeignKey(name: "FK_Bills_Transports", table: "Bills");

            // Khôi phục lại 3 FK tên cũ (đúng như DB của bạn trước đó)
            migrationBuilder.AddForeignKey(
                name: "FK__Bills__CustomerI__619B8048",
                table: "Bills",
                column: "CustomerID",
                principalTable: "Customers",
                principalColumn: "CustomerID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK__Bills__StaffID__60A75C0F",
                table: "Bills",
                column: "StaffID",
                principalTable: "Staffs",
                principalColumn: "StaffID",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK__Bills__Transport__628FA481",
                table: "Bills",
                column: "TransportID",
                principalTable: "Transport", // đổi cho đúng tên bảng thật
                principalColumn: "TransportID",
                onDelete: ReferentialAction.SetNull);
        }
    }
    }
