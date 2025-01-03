﻿// <auto-generated />
using System;
using Implificator.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Implificator.DAL.Migrations
{
    [DbContext(typeof(UserContext))]
    partial class UserContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Implificator.DAL.Entities.QRMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsPublish")
                        .HasColumnType("boolean");

                    b.Property<long>("SharedUserId")
                        .HasColumnType("bigint");

                    b.Property<string>("URL")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("SharedUserId");

                    b.HasIndex("URL");

                    b.HasIndex("UserId");

                    b.ToTable("QRMessages");
                });

            modelBuilder.Entity("Implificator.DAL.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("CountMail")
                        .HasColumnType("bigint");

                    b.Property<long>("CountSubscribe")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsBlocked")
                        .HasColumnType("boolean");

                    b.Property<long>("TgId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("TgId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Implificator.DAL.Entities.VIP", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateTime>("ClosedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsClosed")
                        .HasColumnType("boolean");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("VIPs");
                });

            modelBuilder.Entity("Implificator.DAL.Entities.QRMessage", b =>
                {
                    b.HasOne("Implificator.DAL.Entities.User", "SharedUser")
                        .WithMany()
                        .HasForeignKey("SharedUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Implificator.DAL.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SharedUser");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Implificator.DAL.Entities.VIP", b =>
                {
                    b.HasOne("Implificator.DAL.Entities.User", "User")
                        .WithMany("VIP")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Implificator.DAL.Entities.User", b =>
                {
                    b.Navigation("VIP");
                });
#pragma warning restore 612, 618
        }
    }
}
