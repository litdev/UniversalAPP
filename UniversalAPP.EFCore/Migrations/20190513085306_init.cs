using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UniversalAPP.EFCore.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppVersons",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Platforms = table.Column<byte>(nullable: false),
                    APPType = table.Column<byte>(nullable: false),
                    MD5 = table.Column<string>(maxLength: 100, nullable: true),
                    Size = table.Column<long>(nullable: false),
                    Version = table.Column<string>(maxLength: 20, nullable: false),
                    VersionCode = table.Column<int>(nullable: false),
                    LogoImg = table.Column<string>(maxLength: 255, nullable: true),
                    DownUrl = table.Column<string>(maxLength: 255, nullable: true),
                    LinkUrl = table.Column<string>(maxLength: 500, nullable: true),
                    Content = table.Column<string>(maxLength: 500, nullable: true),
                    AddTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppVersons", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CusCategorys",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 30, nullable: false),
                    PID = table.Column<int>(nullable: true),
                    Depth = table.Column<int>(nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    Weight = table.Column<int>(nullable: false),
                    AddTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CusCategorys", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CusCategorys_CusCategorys_PID",
                        column: x => x.PID,
                        principalTable: "CusCategorys",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SysLogExceptions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Message = table.Column<string>(maxLength: 255, nullable: false),
                    StackTrace = table.Column<string>(nullable: true),
                    AddTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysLogExceptions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SysRoles",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleName = table.Column<string>(maxLength: 30, nullable: false),
                    IsAdmin = table.Column<bool>(nullable: false),
                    RoleDesc = table.Column<string>(maxLength: 255, nullable: false),
                    AddTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysRoles", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SysRoutes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Tag = table.Column<string>(maxLength: 30, nullable: true),
                    IsPost = table.Column<bool>(nullable: false),
                    Route = table.Column<string>(maxLength: 30, nullable: true),
                    Desc = table.Column<string>(maxLength: 30, nullable: true),
                    AddTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysRoutes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SysUser",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(maxLength: 20, nullable: false),
                    NickName = table.Column<string>(maxLength: 30, nullable: false),
                    Gender = table.Column<byte>(nullable: false),
                    Password = table.Column<string>(maxLength: 255, nullable: false),
                    Status = table.Column<bool>(nullable: false),
                    Avatar = table.Column<string>(nullable: true),
                    SysRoleID = table.Column<int>(nullable: false),
                    RegTime = table.Column<DateTime>(nullable: false),
                    LastLoginTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysUser", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SysUser_SysRoles_SysRoleID",
                        column: x => x.SysRoleID,
                        principalTable: "SysRoles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SysRoleRoutes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SysRoleID = table.Column<int>(nullable: false),
                    SysRouteID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysRoleRoutes", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SysRoleRoutes_SysRoles_SysRoleID",
                        column: x => x.SysRoleID,
                        principalTable: "SysRoles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SysRoleRoutes_SysRoutes_SysRouteID",
                        column: x => x.SysRouteID,
                        principalTable: "SysRoutes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Demos",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AddTime = table.Column<DateTime>(nullable: false),
                    AddUserID = table.Column<int>(nullable: true),
                    LastUpdateTime = table.Column<DateTime>(nullable: false),
                    LastUpdateUserID = table.Column<int>(nullable: true),
                    Title = table.Column<string>(maxLength: 50, nullable: false),
                    Telphone = table.Column<string>(maxLength: 15, nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    Ran = table.Column<int>(nullable: false),
                    Num = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demos", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Demos_SysUser_AddUserID",
                        column: x => x.AddUserID,
                        principalTable: "SysUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Demos_SysUser_LastUpdateUserID",
                        column: x => x.LastUpdateUserID,
                        principalTable: "SysUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SysLogMethods",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SysUserID = table.Column<int>(nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    Detail = table.Column<string>(maxLength: 500, nullable: false),
                    AddTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysLogMethods", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SysLogMethods_SysUser_SysUserID",
                        column: x => x.SysUserID,
                        principalTable: "SysUser",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DemoAlbums",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DemoID = table.Column<int>(nullable: false),
                    ImgUrl = table.Column<string>(nullable: false),
                    Weight = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemoAlbums", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DemoAlbums_Demos_DemoID",
                        column: x => x.DemoID,
                        principalTable: "Demos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DemoDepts",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DemoID = table.Column<int>(nullable: false),
                    Title = table.Column<string>(maxLength: 255, nullable: false),
                    ImgUrl = table.Column<string>(maxLength: 255, nullable: false),
                    Num = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemoDepts", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DemoDepts_Demos_DemoID",
                        column: x => x.DemoID,
                        principalTable: "Demos",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CusCategorys_PID",
                table: "CusCategorys",
                column: "PID");

            migrationBuilder.CreateIndex(
                name: "IX_DemoAlbums_DemoID",
                table: "DemoAlbums",
                column: "DemoID");

            migrationBuilder.CreateIndex(
                name: "IX_DemoDepts_DemoID",
                table: "DemoDepts",
                column: "DemoID");

            migrationBuilder.CreateIndex(
                name: "IX_Demos_AddUserID",
                table: "Demos",
                column: "AddUserID");

            migrationBuilder.CreateIndex(
                name: "IX_Demos_LastUpdateUserID",
                table: "Demos",
                column: "LastUpdateUserID");

            migrationBuilder.CreateIndex(
                name: "IX_SysLogMethods_SysUserID",
                table: "SysLogMethods",
                column: "SysUserID");

            migrationBuilder.CreateIndex(
                name: "IX_SysRoleRoutes_SysRoleID",
                table: "SysRoleRoutes",
                column: "SysRoleID");

            migrationBuilder.CreateIndex(
                name: "IX_SysRoleRoutes_SysRouteID",
                table: "SysRoleRoutes",
                column: "SysRouteID");

            migrationBuilder.CreateIndex(
                name: "IX_SysUser_SysRoleID",
                table: "SysUser",
                column: "SysRoleID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppVersons");

            migrationBuilder.DropTable(
                name: "CusCategorys");

            migrationBuilder.DropTable(
                name: "DemoAlbums");

            migrationBuilder.DropTable(
                name: "DemoDepts");

            migrationBuilder.DropTable(
                name: "SysLogExceptions");

            migrationBuilder.DropTable(
                name: "SysLogMethods");

            migrationBuilder.DropTable(
                name: "SysRoleRoutes");

            migrationBuilder.DropTable(
                name: "Demos");

            migrationBuilder.DropTable(
                name: "SysRoutes");

            migrationBuilder.DropTable(
                name: "SysUser");

            migrationBuilder.DropTable(
                name: "SysRoles");
        }
    }
}
