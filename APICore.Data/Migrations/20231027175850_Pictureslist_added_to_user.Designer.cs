﻿// <auto-generated />
using System;
using APICore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace APICore.Data.Migrations
{
    [DbContext(typeof(CoreDbContext))]
    [Migration("20231027175850_Pictureslist_added_to_user")]
    partial class Pictureslist_added_to_user
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("APICore.Data.Entities.BlockedUsers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime>("BlockDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("BlockedUserId")
                        .HasColumnType("int");

                    b.Property<int>("BlockerUserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BlockedUserId");

                    b.HasIndex("BlockerUserId");

                    b.ToTable("BlockedUsers");
                });

            modelBuilder.Entity("APICore.Data.Entities.Log", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("App")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("EventType")
                        .HasColumnType("int");

                    b.Property<int>("LogType")
                        .HasColumnType("int");

                    b.Property<string>("Module")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Log");
                });

            modelBuilder.Entity("APICore.Data.Entities.ReportedUsers", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Coment")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ReporStatus")
                        .HasColumnType("int");

                    b.Property<DateTime>("ReportDateTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("ReportedUserId")
                        .HasColumnType("int");

                    b.Property<int>("ReporterUserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ReportedUserId");

                    b.HasIndex("ReporterUserId");

                    b.ToTable("ReportedUsers");
                });

            modelBuilder.Entity("APICore.Data.Entities.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Setting");
                });

            modelBuilder.Entity("APICore.Data.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Avatar")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("AvatarMimeType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedCode")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<string>("Identity")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsEmailVerified")
                        .HasColumnType("bit");

                    b.Property<bool>("IsGenderVisible")
                        .HasColumnType("bit");

                    b.Property<bool>("IsGeneratedPassChanged")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPhoneVerified")
                        .HasColumnType("bit");

                    b.Property<bool>("IsSexualityVisible")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LastLoggedIn")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Pictures")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("SexualOrientation")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("VerificationCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("APICore.Data.Entities.UserToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("AccessToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("AccessTokenExpiresDateTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("ClientName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClientType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClientVersion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DeviceBrand")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DeviceModel")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OS")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OSPlatform")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OSVersion")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RefreshToken")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTimeOffset>("RefreshTokenExpiresDateTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserToken");
                });

            modelBuilder.Entity("APICore.Data.Entities.BlockedUsers", b =>
                {
                    b.HasOne("APICore.Data.Entities.User", "BlockedUser")
                        .WithMany("Blockeds")
                        .HasForeignKey("BlockedUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("APICore.Data.Entities.User", "BlockerUser")
                        .WithMany("Blockers")
                        .HasForeignKey("BlockerUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("BlockedUser");

                    b.Navigation("BlockerUser");
                });

            modelBuilder.Entity("APICore.Data.Entities.ReportedUsers", b =>
                {
                    b.HasOne("APICore.Data.Entities.User", "ReportedUser")
                        .WithMany("Reporteds")
                        .HasForeignKey("ReportedUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("APICore.Data.Entities.User", "ReporterUser")
                        .WithMany("Reporters")
                        .HasForeignKey("ReporterUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("ReportedUser");

                    b.Navigation("ReporterUser");
                });

            modelBuilder.Entity("APICore.Data.Entities.UserToken", b =>
                {
                    b.HasOne("APICore.Data.Entities.User", "User")
                        .WithMany("UserTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("APICore.Data.Entities.User", b =>
                {
                    b.Navigation("Blockeds");

                    b.Navigation("Blockers");

                    b.Navigation("Reporteds");

                    b.Navigation("Reporters");

                    b.Navigation("UserTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
