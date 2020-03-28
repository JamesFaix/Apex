﻿// <auto-generated />
using System;
using Apex.Api.Db.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Apex.Api.Db.Model.Migrations
{
    [DbContext(typeof(ApexDbContext))]
    [Migration("20200328021518_0.15")]
    partial class _015
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Apex.Api.Db.Model.EventKindSqlModel", b =>
                {
                    b.Property<byte>("Id")
                        .HasColumnName("EventKindId")
                        .HasColumnType("tinyint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)")
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.ToTable("EventKinds");
                });

            modelBuilder.Entity("Apex.Api.Db.Model.EventSqlModel", b =>
                {
                    b.Property<int>("EventId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ActingPlayerId")
                        .HasColumnType("int");

                    b.Property<int>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("EffectsJson")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte>("EventKindId")
                        .HasColumnType("tinyint");

                    b.Property<int>("GameId")
                        .HasColumnType("int");

                    b.HasKey("EventId");

                    b.HasIndex("ActingPlayerId");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("EventKindId");

                    b.HasIndex("GameId");

                    b.ToTable("Events");
                });

            modelBuilder.Entity("Apex.Api.Db.Model.GameSqlModel", b =>
                {
                    b.Property<int>("GameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("AllowGuests")
                        .HasColumnType("bit");

                    b.Property<int>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("CurrentTurnJson")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte>("GameStatusId")
                        .HasColumnType("tinyint");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("bit");

                    b.Property<string>("PiecesJson")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte>("RegionCount")
                        .HasColumnType("tinyint");

                    b.Property<string>("TurnCycleJson")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("GameId");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("GameStatusId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("Apex.Api.Db.Model.GameStatusSqlModel", b =>
                {
                    b.Property<byte>("Id")
                        .HasColumnName("GameStatusId")
                        .HasColumnType("tinyint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)")
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.ToTable("GameStatuses");
                });

            modelBuilder.Entity("Apex.Api.Db.Model.NeutralPlayerNameSqlModel", b =>
                {
                    b.Property<int>("NeutralPlayerNameId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)")
                        .HasMaxLength(20);

                    b.HasKey("NeutralPlayerNameId");

                    b.ToTable("NeutralPlayerNames");
                });

            modelBuilder.Entity("Apex.Api.Db.Model.PlayerKindSqlModel", b =>
                {
                    b.Property<byte>("Id")
                        .HasColumnName("PlayerKindId")
                        .HasColumnType("tinyint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)")
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.ToTable("PlayerKinds");
                });

            modelBuilder.Entity("Apex.Api.Db.Model.PlayerSqlModel", b =>
                {
                    b.Property<int>("PlayerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte?>("ColorId")
                        .HasColumnType("tinyint");

                    b.Property<int>("GameId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)")
                        .HasMaxLength(20);

                    b.Property<byte>("PlayerKindId")
                        .HasColumnType("tinyint");

                    b.Property<byte>("PlayerStatusId")
                        .HasColumnType("tinyint");

                    b.Property<byte?>("StartingRegion")
                        .HasColumnType("tinyint");

                    b.Property<byte?>("StartingTurnNumber")
                        .HasColumnType("tinyint");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("PlayerId");

                    b.HasIndex("GameId");

                    b.HasIndex("PlayerKindId");

                    b.HasIndex("PlayerStatusId");

                    b.HasIndex("UserId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("Apex.Api.Db.Model.PlayerStatusSqlModel", b =>
                {
                    b.Property<byte>("Id")
                        .HasColumnName("PlayerStatusId")
                        .HasColumnType("tinyint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)")
                        .HasMaxLength(20);

                    b.HasKey("Id");

                    b.ToTable("PlayerStatuses");
                });

            modelBuilder.Entity("Apex.Api.Db.Model.PrivilegeSqlModel", b =>
                {
                    b.Property<byte>("Id")
                        .HasColumnName("PrivilegeId")
                        .HasColumnType("tinyint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Privileges");
                });

            modelBuilder.Entity("Apex.Api.Db.Model.SessionSqlModel", b =>
                {
                    b.Property<int>("SessionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ExpiresOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("SessionId");

                    b.HasIndex("UserId");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("Apex.Api.Db.Model.SnapshotSqlModel", b =>
                {
                    b.Property<int>("SnapshotId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("CreatedByUserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(50)")
                        .HasMaxLength(50);

                    b.Property<int>("GameId")
                        .HasColumnType("int");

                    b.Property<string>("SnapshotJson")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("SnapshotId");

                    b.HasIndex("CreatedByUserId");

                    b.HasIndex("GameId");

                    b.ToTable("Snapshots");
                });

            modelBuilder.Entity("Apex.Api.Db.Model.UserPrivilegeSqlModel", b =>
                {
                    b.Property<int>("UserPrivilegeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<byte>("PrivilegeId")
                        .HasColumnType("tinyint");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("UserPrivilegeId");

                    b.HasIndex("PrivilegeId");

                    b.HasIndex("UserId");

                    b.ToTable("UserPrivileges");
                });

            modelBuilder.Entity("Apex.Api.Db.Model.UserSqlModel", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedOn")
                        .HasColumnType("datetime2");

                    b.Property<byte>("FailedLoginAttempts")
                        .HasColumnType("tinyint");

                    b.Property<DateTime?>("LastFailedLoginAttemptOn")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(20)")
                        .HasMaxLength(20);

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Apex.Api.Db.Model.EventSqlModel", b =>
                {
                    b.HasOne("Apex.Api.Db.Model.PlayerSqlModel", "ActingPlayer")
                        .WithMany()
                        .HasForeignKey("ActingPlayerId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Apex.Api.Db.Model.UserSqlModel", "CreatedByUser")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Apex.Api.Db.Model.EventKindSqlModel", "EventKind")
                        .WithMany()
                        .HasForeignKey("EventKindId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Apex.Api.Db.Model.GameSqlModel", "Game")
                        .WithMany("Events")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Apex.Api.Db.Model.GameSqlModel", b =>
                {
                    b.HasOne("Apex.Api.Db.Model.UserSqlModel", "CreatedByUser")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Apex.Api.Db.Model.GameStatusSqlModel", "GameStatus")
                        .WithMany()
                        .HasForeignKey("GameStatusId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Apex.Api.Db.Model.PlayerSqlModel", b =>
                {
                    b.HasOne("Apex.Api.Db.Model.GameSqlModel", "Game")
                        .WithMany("Players")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Apex.Api.Db.Model.PlayerKindSqlModel", "PlayerKind")
                        .WithMany()
                        .HasForeignKey("PlayerKindId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Apex.Api.Db.Model.PlayerStatusSqlModel", "PlayerStatus")
                        .WithMany()
                        .HasForeignKey("PlayerStatusId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Apex.Api.Db.Model.UserSqlModel", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Apex.Api.Db.Model.SessionSqlModel", b =>
                {
                    b.HasOne("Apex.Api.Db.Model.UserSqlModel", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Apex.Api.Db.Model.SnapshotSqlModel", b =>
                {
                    b.HasOne("Apex.Api.Db.Model.UserSqlModel", "CreatedByUser")
                        .WithMany()
                        .HasForeignKey("CreatedByUserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Apex.Api.Db.Model.GameSqlModel", "Game")
                        .WithMany()
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Apex.Api.Db.Model.UserPrivilegeSqlModel", b =>
                {
                    b.HasOne("Apex.Api.Db.Model.PrivilegeSqlModel", "Privilege")
                        .WithMany()
                        .HasForeignKey("PrivilegeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Apex.Api.Db.Model.UserSqlModel", "User")
                        .WithMany("UserPrivileges")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
