﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OKR.Models.Context;

#nullable disable

namespace OKR.Models.Migrations
{
    [DbContext(typeof(OKRDBContext))]
    partial class OKRDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("OKR.Models.Entity.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<Guid?>("DepartmentId")
                        .HasColumnType("char(36)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("OKR.Models.Entity.Department", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("Level")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Modifiedby")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<Guid?>("ParentDepartmentId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("ParentDepartmentId");

                    b.ToTable("Department");
                });

            modelBuilder.Entity("OKR.Models.Entity.DepartmentObjectives", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid>("DepartmentId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Modifiedby")
                        .HasColumnType("longtext");

                    b.Property<Guid>("ObjectivesId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentId");

                    b.HasIndex("ObjectivesId");

                    b.ToTable("DepartmentObjectives");
                });

            modelBuilder.Entity("OKR.Models.Entity.DepartmentProgressApproval", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<int>("AddedPoints")
                        .HasColumnType("int");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid>("KeyResultsId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Modifiedby")
                        .HasColumnType("longtext");

                    b.Property<string>("Note")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("KeyResultsId");

                    b.ToTable("DepartmentProgressApproval");
                });

            modelBuilder.Entity("OKR.Models.Entity.EvaluateTarget", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<Guid?>("DepartmentObjectivesId")
                        .HasColumnType("char(36)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Modifiedby")
                        .HasColumnType("longtext");

                    b.Property<Guid?>("UserObjectivesId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("DepartmentObjectivesId");

                    b.HasIndex("UserObjectivesId");

                    b.ToTable("EvaluateTarget");
                });

            modelBuilder.Entity("OKR.Models.Entity.KeyResults", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<bool>("Active")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("CurrentPoint")
                        .HasColumnType("int");

                    b.Property<DateTime>("Deadline")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("MaximunPoint")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Modifiedby")
                        .HasColumnType("longtext");

                    b.Property<Guid>("ObjectivesId")
                        .HasColumnType("char(36)");

                    b.Property<int?>("Unit")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ObjectivesId");

                    b.ToTable("KeyResults");
                });

            modelBuilder.Entity("OKR.Models.Entity.Objectives", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("Deadline")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Modifiedby")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime>("StartDay")
                        .HasColumnType("datetime(6)");

                    b.Property<int>("TargetType")
                        .HasColumnType("int");

                    b.Property<int>("status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Objectives");
                });

            modelBuilder.Entity("OKR.Models.Entity.ProgressUpdates", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid>("KeyResultId")
                        .HasColumnType("char(36)");

                    b.Property<int>("KeyresultCompletionRate")
                        .HasColumnType("int");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Modifiedby")
                        .HasColumnType("longtext");

                    b.Property<int?>("NewPoint")
                        .HasColumnType("int");

                    b.Property<string>("Note")
                        .HasColumnType("longtext");

                    b.Property<int>("ObjectivesCompletionRate")
                        .HasColumnType("int");

                    b.Property<int?>("OldPoint")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("KeyResultId");

                    b.ToTable("ProgressUpdates");
                });

            modelBuilder.Entity("OKR.Models.Entity.Sidequests", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<Guid>("KeyResultsId")
                        .HasColumnType("char(36)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Modifiedby")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("Status")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.HasIndex("KeyResultsId");

                    b.ToTable("Sidequests");
                });

            modelBuilder.Entity("OKR.Models.Entity.UserObjectives", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)");

                    b.Property<string>("ApplicationUserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("CreatedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTime?>("ModifiedOn")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Modifiedby")
                        .HasColumnType("longtext");

                    b.Property<Guid>("ObjectivesId")
                        .HasColumnType("char(36)");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("ObjectivesId");

                    b.ToTable("UserObjectives");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("OKR.Models.Entity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("OKR.Models.Entity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("OKR.Models.Entity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("OKR.Models.Entity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OKR.Models.Entity.ApplicationUser", b =>
                {
                    b.HasOne("OKR.Models.Entity.Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Department");
                });

            modelBuilder.Entity("OKR.Models.Entity.Department", b =>
                {
                    b.HasOne("OKR.Models.Entity.Department", "ParentDepartment")
                        .WithMany()
                        .HasForeignKey("ParentDepartmentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("ParentDepartment");
                });

            modelBuilder.Entity("OKR.Models.Entity.DepartmentObjectives", b =>
                {
                    b.HasOne("OKR.Models.Entity.Department", "Department")
                        .WithMany()
                        .HasForeignKey("DepartmentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("OKR.Models.Entity.Objectives", "Objectives")
                        .WithMany("DepartmentObjectives")
                        .HasForeignKey("ObjectivesId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Department");

                    b.Navigation("Objectives");
                });

            modelBuilder.Entity("OKR.Models.Entity.DepartmentProgressApproval", b =>
                {
                    b.HasOne("OKR.Models.Entity.KeyResults", "KeyResults")
                        .WithMany()
                        .HasForeignKey("KeyResultsId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("KeyResults");
                });

            modelBuilder.Entity("OKR.Models.Entity.EvaluateTarget", b =>
                {
                    b.HasOne("OKR.Models.Entity.DepartmentObjectives", "DepartmentObjectives")
                        .WithMany()
                        .HasForeignKey("DepartmentObjectivesId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("OKR.Models.Entity.UserObjectives", "UserObjectives")
                        .WithMany()
                        .HasForeignKey("UserObjectivesId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("DepartmentObjectives");

                    b.Navigation("UserObjectives");
                });

            modelBuilder.Entity("OKR.Models.Entity.KeyResults", b =>
                {
                    b.HasOne("OKR.Models.Entity.Objectives", "Objectives")
                        .WithMany()
                        .HasForeignKey("ObjectivesId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Objectives");
                });

            modelBuilder.Entity("OKR.Models.Entity.ProgressUpdates", b =>
                {
                    b.HasOne("OKR.Models.Entity.KeyResults", "KeyResults")
                        .WithMany()
                        .HasForeignKey("KeyResultId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("KeyResults");
                });

            modelBuilder.Entity("OKR.Models.Entity.Sidequests", b =>
                {
                    b.HasOne("OKR.Models.Entity.KeyResults", "KeyResults")
                        .WithMany()
                        .HasForeignKey("KeyResultsId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("KeyResults");
                });

            modelBuilder.Entity("OKR.Models.Entity.UserObjectives", b =>
                {
                    b.HasOne("OKR.Models.Entity.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("OKR.Models.Entity.Objectives", "Objectives")
                        .WithMany("UserObjectives")
                        .HasForeignKey("ObjectivesId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ApplicationUser");

                    b.Navigation("Objectives");
                });

            modelBuilder.Entity("OKR.Models.Entity.Objectives", b =>
                {
                    b.Navigation("DepartmentObjectives");

                    b.Navigation("UserObjectives");
                });
#pragma warning restore 612, 618
        }
    }
}
