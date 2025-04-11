using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClientNexus.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "ClientNexusSchema");

            migrationBuilder.CreateTable(
                name: "AccessLevels",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessLevels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DCategories",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Signature = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    ReferenceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentGateway = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "char(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    PaymentType = table.Column<string>(type: "char(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceProviderTypes",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProviderTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "States",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                    table.ForeignKey(
                        name: "FK_States_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmergencyCategories",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ServiceProviderTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmergencyCategories_ServiceProviderTypes_ServiceProviderTypeId",
                        column: x => x.ServiceProviderTypeId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "ServiceProviderTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Specializations",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ServiceProviderTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Specializations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Specializations_ServiceProviderTypes_ServiceProviderTypeId",
                        column: x => x.ServiceProviderTypeId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "ServiceProviderTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Abbreviation = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Countries_CountryId",
                        column: x => x.CountryId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cities_States_StateId",
                        column: x => x.StateId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    BaseUserId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DetailedAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Neighborhood = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MapUrl = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => new { x.BaseUserId, x.Id });
                    table.ForeignKey(
                        name: "FK_Addresses_Cities_CityId",
                        column: x => x.CityId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Admins",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ApprovedById = table.Column<int>(type: "int", nullable: true),
                    AccessLevelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admins_AccessLevels_AccessLevelId",
                        column: x => x.AccessLevelId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "AccessLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Admins_Admins_ApprovedById",
                        column: x => x.ApprovedById,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Admins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BaseUsers",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    UserType = table.Column<string>(type: "char(1)", nullable: false),
                    NotificationToken = table.Column<string>(type: "varchar(1000)", nullable: true),
                    BlockedById = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "varchar(20)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BaseUsers_Admins_BlockedById",
                        column: x => x.BlockedById,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Admins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", nullable: false),
                    Url = table.Column<string>(type: "varchar(500)", nullable: false),
                    DocumentTypeId = table.Column<int>(type: "int", nullable: false),
                    UploadedById = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Admins_UploadedById",
                        column: x => x.UploadedById,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Admins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_DocumentTypes_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "DocumentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_BaseUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "BaseUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_BaseUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "BaseUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_BaseUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "BaseUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_BaseUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "BaseUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<float>(type: "real", nullable: false, defaultValue: 0f)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Clients_BaseUsers_Id",
                        column: x => x.Id,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "BaseUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PhoneNumbers",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    BaseUserId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "varchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhoneNumbers", x => new { x.BaseUserId, x.Id });
                    table.ForeignKey(
                        name: "FK_PhoneNumbers_BaseUsers_BaseUserId",
                        column: x => x.BaseUserId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "BaseUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceProviders",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CurrentLocation = table.Column<string>(type: "nvarchar(500)", nullable: true),
                    MainImage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Rate = table.Column<float>(type: "real", nullable: false, defaultValue: 0f),
                    IsFeatured = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    IsAvailableForEmergency = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    ImageIDUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageNationalIDUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubType = table.Column<string>(type: "char(1)", nullable: false),
                    SubscriptionStatus = table.Column<string>(type: "char(1)", nullable: false),
                    SubscriptionExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    ApprovedById = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProviders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceProviders_Admins_ApprovedById",
                        column: x => x.ApprovedById,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Admins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceProviders_BaseUsers_Id",
                        column: x => x.Id,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "BaseUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceProviders_ServiceProviderTypes_TypeId",
                        column: x => x.TypeId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "ServiceProviderTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentCategories",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    DocumentId = table.Column<int>(type: "int", nullable: false),
                    DCategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentCategories", x => new { x.DocumentId, x.DCategoryId });
                    table.ForeignKey(
                        name: "FK_DocumentCategories_DCategories_DCategoryId",
                        column: x => x.DCategoryId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "DCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentCategories_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentCosts",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    AppointmentType = table.Column<string>(type: "char(1)", nullable: false),
                    ServiceProviderId = table.Column<int>(type: "int", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentCosts", x => new { x.ServiceProviderId, x.AppointmentType });
                    table.ForeignKey(
                        name: "FK_AppointmentCosts_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientServiceProviderFeedbacks",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    ServiceProviderId = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<float>(type: "real", nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(1000)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientServiceProviderFeedbacks", x => new { x.ServiceProviderId, x.ClientId });
                    table.ForeignKey(
                        name: "FK_ClientServiceProviderFeedbacks_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientServiceProviderFeedbacks_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OfficeImageUrls",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    ServiceProviderId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeImageUrls", x => new { x.ServiceProviderId, x.Id });
                    table.ForeignKey(
                        name: "FK_OfficeImageUrls_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceProviderSpecializations",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    ServiceProviderId = table.Column<int>(type: "int", nullable: false),
                    SpecializationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceProviderSpecializations", x => new { x.ServiceProviderId, x.SpecializationId });
                    table.ForeignKey(
                        name: "FK_ServiceProviderSpecializations_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceProviderSpecializations_Specializations_SpecializationId",
                        column: x => x.SpecializationId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Specializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "char(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    ServiceType = table.Column<string>(type: "char(1)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    ServiceProviderId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Services_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Services_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Slots",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceProviderId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "char(1)", nullable: false),
                    SlotType = table.Column<string>(type: "char(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Slots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Slots_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPayments",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SubscriptionType = table.Column<string>(type: "char(1)", nullable: false),
                    ServiceProviderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionPayments_Payments_Id",
                        column: x => x.Id,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionPayments_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConsultationCases",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultationCases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsultationCases_Services_Id",
                        column: x => x.Id,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmergencyCases",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    TimeForArrival = table.Column<int>(type: "int", nullable: true),
                    MeetingLongitude = table.Column<double>(type: "float", nullable: false),
                    MeetingLatitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmergencyCases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmergencyCases_Services_Id",
                        column: x => x.Id,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Problems",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(1000)", nullable: false),
                    Status = table.Column<string>(type: "char(1)", nullable: false),
                    ReportedBy = table.Column<string>(type: "char(1)", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    ServiceProviderId = table.Column<int>(type: "int", nullable: false),
                    SolvingAdminId = table.Column<int>(type: "int", nullable: true),
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Problems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Problems_Admins_SolvingAdminId",
                        column: x => x.SolvingAdminId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Admins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Problems_Clients_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Problems_ServiceProviders_ServiceProviderId",
                        column: x => x.ServiceProviderId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "ServiceProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Problems_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Visibility = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Services_Id",
                        column: x => x.Id,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServicePayments",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServicePayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServicePayments_Payments_Id",
                        column: x => x.Id,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServicePayments_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CheckInTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancellationReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CancellationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReminderSent = table.Column<bool>(type: "bit", nullable: false),
                    ReminderSentTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SlotId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_Services_Id",
                        column: x => x.Id,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Slots_SlotId",
                        column: x => x.SlotId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "Slots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CaseFiles",
                schema: "ClientNexusSchema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ConsultCaseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseFiles_ConsultationCases_ConsultCaseId",
                        column: x => x.ConsultCaseId,
                        principalSchema: "ClientNexusSchema",
                        principalTable: "ConsultationCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "ClientNexusSchema",
                table: "AccessLevels",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { -2, "Customer Service" },
                    { -1, "Super Admin" }
                });

            migrationBuilder.InsertData(
                schema: "ClientNexusSchema",
                table: "DocumentTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { -3, "Other" },
                    { -2, "Template" },
                    { -1, "Article" }
                });

            migrationBuilder.InsertData(
                schema: "ClientNexusSchema",
                table: "ServiceProviderTypes",
                columns: new[] { "Id", "Name" },
                values: new object[] { -1, "Lawyer" });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CityId",
                schema: "ClientNexusSchema",
                table: "Addresses",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Admins_AccessLevelId",
                schema: "ClientNexusSchema",
                table: "Admins",
                column: "AccessLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Admins_ApprovedById",
                schema: "ClientNexusSchema",
                table: "Admins",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_SlotId",
                schema: "ClientNexusSchema",
                table: "Appointments",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "ClientNexusSchema",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "ClientNexusSchema",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "ClientNexusSchema",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "ClientNexusSchema",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "ClientNexusSchema",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "ClientNexusSchema",
                table: "BaseUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_BaseUsers_BlockedById",
                schema: "ClientNexusSchema",
                table: "BaseUsers",
                column: "BlockedById");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "ClientNexusSchema",
                table: "BaseUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CaseFiles_ConsultCaseId",
                schema: "ClientNexusSchema",
                table: "CaseFiles",
                column: "ConsultCaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                schema: "ClientNexusSchema",
                table: "Cities",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_StateId",
                schema: "ClientNexusSchema",
                table: "Cities",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientServiceProviderFeedbacks_ClientId",
                schema: "ClientNexusSchema",
                table: "ClientServiceProviderFeedbacks",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentCategories_DCategoryId",
                schema: "ClientNexusSchema",
                table: "DocumentCategories",
                column: "DCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_DocumentTypeId",
                schema: "ClientNexusSchema",
                table: "Documents",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UploadedById",
                schema: "ClientNexusSchema",
                table: "Documents",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyCategories_ServiceProviderTypeId",
                schema: "ClientNexusSchema",
                table: "EmergencyCategories",
                column: "ServiceProviderTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_ClientId",
                schema: "ClientNexusSchema",
                table: "Problems",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_ServiceId",
                schema: "ClientNexusSchema",
                table: "Problems",
                column: "ServiceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Problems_ServiceProviderId",
                schema: "ClientNexusSchema",
                table: "Problems",
                column: "ServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Problems_SolvingAdminId",
                schema: "ClientNexusSchema",
                table: "Problems",
                column: "SolvingAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_ServicePayments_ServiceId",
                schema: "ClientNexusSchema",
                table: "ServicePayments",
                column: "ServiceId",
                unique: true,
                filter: "[ServiceId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviders_ApprovedById",
                schema: "ClientNexusSchema",
                table: "ServiceProviders",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviders_TypeId",
                schema: "ClientNexusSchema",
                table: "ServiceProviders",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceProviderSpecializations_SpecializationId",
                schema: "ClientNexusSchema",
                table: "ServiceProviderSpecializations",
                column: "SpecializationId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ClientId",
                schema: "ClientNexusSchema",
                table: "Services",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_ServiceProviderId",
                schema: "ClientNexusSchema",
                table: "Services",
                column: "ServiceProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Slots_ServiceProviderId_Date",
                schema: "ClientNexusSchema",
                table: "Slots",
                columns: new[] { "ServiceProviderId", "Date" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Specializations_ServiceProviderTypeId",
                schema: "ClientNexusSchema",
                table: "Specializations",
                column: "ServiceProviderTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_States_CountryId",
                schema: "ClientNexusSchema",
                table: "States",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionPayments_ServiceProviderId",
                schema: "ClientNexusSchema",
                table: "SubscriptionPayments",
                column: "ServiceProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_BaseUsers_BaseUserId",
                schema: "ClientNexusSchema",
                table: "Addresses",
                column: "BaseUserId",
                principalSchema: "ClientNexusSchema",
                principalTable: "BaseUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Admins_BaseUsers_Id",
                schema: "ClientNexusSchema",
                table: "Admins",
                column: "Id",
                principalSchema: "ClientNexusSchema",
                principalTable: "BaseUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Admins_BaseUsers_Id",
                schema: "ClientNexusSchema",
                table: "Admins");

            migrationBuilder.DropTable(
                name: "Addresses",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "AppointmentCosts",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "Appointments",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "CaseFiles",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "ClientServiceProviderFeedbacks",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "DocumentCategories",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "EmergencyCases",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "EmergencyCategories",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "OfficeImageUrls",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "PhoneNumbers",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "Problems",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "Questions",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "ServicePayments",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "ServiceProviderSpecializations",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "SubscriptionPayments",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "Cities",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "Slots",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "ConsultationCases",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "DCategories",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "Documents",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "Specializations",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "Payments",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "States",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "Services",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "DocumentTypes",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "Countries",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "Clients",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "ServiceProviders",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "ServiceProviderTypes",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "BaseUsers",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "Admins",
                schema: "ClientNexusSchema");

            migrationBuilder.DropTable(
                name: "AccessLevels",
                schema: "ClientNexusSchema");
        }
    }
}
