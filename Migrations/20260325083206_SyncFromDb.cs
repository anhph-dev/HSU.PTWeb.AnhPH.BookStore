using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HSU.PTWeb.AnhPH.BookStore.Migrations
{
    /// <inheritdoc />
    public partial class SyncFromDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'City' AND Object_ID = Object_ID(N'Orders'))
                BEGIN
                    ALTER TABLE Orders DROP COLUMN City
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'Ward' AND Object_ID = Object_ID(N'Orders'))
                BEGIN
                    ALTER TABLE Orders DROP COLUMN Ward
                END
            ");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('Orders', 'AppUserId') IS NULL
                BEGIN
                    ALTER TABLE [Orders] ADD [AppUserId] int NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('Orders', 'Channel') IS NULL
                BEGIN
                    ALTER TABLE [Orders] ADD [Channel] nvarchar(50) NOT NULL CONSTRAINT [DF_Orders_Channel] DEFAULT N'Online';
                END
            ");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('Orders', 'CityId') IS NULL
                BEGIN
                    ALTER TABLE [Orders] ADD [CityId] int NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF COL_LENGTH('Orders', 'WardId') IS NULL
                BEGIN
                    ALTER TABLE [Orders] ADD [WardId] int NULL;
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = N'IX_Orders_AppUserId'
                      AND object_id = OBJECT_ID(N'Orders')
                )
                BEGIN
                    CREATE INDEX [IX_Orders_AppUserId] ON [Orders] ([AppUserId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = N'IX_Orders_CityId'
                      AND object_id = OBJECT_ID(N'Orders')
                )
                BEGIN
                    CREATE INDEX [IX_Orders_CityId] ON [Orders] ([CityId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = N'IX_Orders_Status_OrderDate'
                      AND object_id = OBJECT_ID(N'Orders')
                )
                BEGIN
                    CREATE INDEX [IX_Orders_Status_OrderDate] ON [Orders] ([Status] ASC, [OrderDate] DESC);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = N'IX_Orders_WardId'
                      AND object_id = OBJECT_ID(N'Orders')
                )
                BEGIN
                    CREATE INDEX [IX_Orders_WardId] ON [Orders] ([WardId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = N'FK_Orders_AppUser'
                )
                BEGIN
                    ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_AppUser]
                    FOREIGN KEY ([AppUserId]) REFERENCES [Users] ([UserId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = N'FK_Orders_Cities'
                )
                BEGIN
                    ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_Cities]
                    FOREIGN KEY ([CityId]) REFERENCES [Cities] ([CityId]);
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.foreign_keys
                    WHERE name = N'FK_Orders_Wards'
                )
                BEGIN
                    ALTER TABLE [Orders] ADD CONSTRAINT [FK_Orders_Wards]
                    FOREIGN KEY ([WardId]) REFERENCES [Wards] ([WardId]);
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AppUser",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Cities",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Wards",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_AppUserId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_CityId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_Status_OrderDate",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_WardId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "WardId",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Ward",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
