using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Xóa Khóa ngoại cũ (Liên kết sai với bảng Roles)
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Roles",
                table: "Customers");

            // 2. Xóa Index cũ
            migrationBuilder.DropIndex(
                name: "IX_Customers_RoleID", // Tên index cũ bạn đang đặt
                table: "Customers");

            // 3. Sửa cột AccountID: Bỏ giá trị mặc định (DefaultValue: 3)
            migrationBuilder.AlterColumn<int>(
                name: "AccountID",
                table: "Customers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 3);

            // 4. Tạo Index mới: Unique (Quan hệ 1-1)
            migrationBuilder.CreateIndex(
                name: "IX_Customers_AccountID",
                table: "Customers",
                column: "AccountID",
                unique: true);

            // 5. Tạo Khóa ngoại mới (Liên kết đúng với bảng Accounts)
            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Accounts",
                table: "Customers",
                column: "AccountID",
                principalTable: "Accounts",
                principalColumn: "AccountID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // --- ROLLBACK (Quay lại trạng thái cũ nếu lỗi) ---

            migrationBuilder.DropForeignKey(
                name: "FK_Customers_Accounts",
                table: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Customers_AccountID",
                table: "Customers");

            // Trả lại giá trị mặc định là 3
            migrationBuilder.AlterColumn<int>(
                name: "AccountID",
                table: "Customers",
                type: "int",
                nullable: false,
                defaultValue: 3,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_RoleID",
                table: "Customers",
                column: "AccountID");

            migrationBuilder.AddForeignKey(
                name: "FK_Customers_Roles",
                table: "Customers",
                column: "AccountID",
                principalTable: "Roles",
                principalColumn: "RoleID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
