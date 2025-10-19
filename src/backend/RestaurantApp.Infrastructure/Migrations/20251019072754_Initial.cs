using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestaurantApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "orders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    table_id = table.Column<int>(type: "integer", nullable: false),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    total_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    confirmed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_orders", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "payments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    transaction_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    failure_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_payments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "products",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    price_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false),
                    allergens = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_available = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_products", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tables",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    active_session_id = table.Column<Guid>(type: "uuid", nullable: true),
                    session_started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    session_ended_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tables", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "order_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    unit_price_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    unit_price_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    subtotal_amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    subtotal_currency = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_order_lines", x => x.id);
                    table.ForeignKey(
                        name: "FK_order_lines_orders_order_id",
                        column: x => x.order_id,
                        principalTable: "orders",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_categories_is_active",
                table: "categories",
                column: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_categories_name",
                table: "categories",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_order_lines_order_id",
                table: "order_lines",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_order_lines_product_id",
                table: "order_lines",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_created_at",
                table: "orders",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_orders_session_id",
                table: "orders",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "ix_orders_status",
                table: "orders",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_orders_table_id",
                table: "orders",
                column: "table_id");

            migrationBuilder.CreateIndex(
                name: "ix_payments_created_at",
                table: "payments",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_payments_order_id",
                table: "payments",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "ix_payments_status",
                table: "payments",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_payments_transaction_id",
                table: "payments",
                column: "transaction_id");

            migrationBuilder.CreateIndex(
                name: "ix_products_category_id",
                table: "products",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "ix_products_is_available",
                table: "products",
                column: "is_available");

            migrationBuilder.CreateIndex(
                name: "ix_products_name",
                table: "products",
                column: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "categories");

            migrationBuilder.DropTable(
                name: "order_lines");

            migrationBuilder.DropTable(
                name: "payments");

            migrationBuilder.DropTable(
                name: "products");

            migrationBuilder.DropTable(
                name: "tables");

            migrationBuilder.DropTable(
                name: "orders");
        }
    }
}
