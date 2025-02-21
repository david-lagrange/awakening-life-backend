﻿// <auto-generated />
using System;
using AwakeningLifeBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AwakeningLifeBackend.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(RepositoryContext))]
    partial class RepositoryContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("AwakeningLifeBackend.Core.Domain.Entities.BaseEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("BaseEntityId");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("character varying(60)");

                    b.Property<string>("Country")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(60)
                        .HasColumnType("character varying(60)");

                    b.HasKey("Id");

                    b.ToTable("BaseEntities", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870"),
                            Address = "583 Wall Dr. Gwynn Oak, MD 21207",
                            Country = "USA",
                            Name = "IT_Solutions Ltd"
                        },
                        new
                        {
                            Id = new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3"),
                            Address = "312 Forest Avenue, BF 923",
                            Country = "USA",
                            Name = "Admin_Solutions Ltd"
                        });
                });

            modelBuilder.Entity("AwakeningLifeBackend.Core.Domain.Entities.DependantEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("DependantEntityId");

                    b.Property<int>("Age")
                        .HasColumnType("integer");

                    b.Property<Guid>("BaseEntityId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.HasKey("Id");

                    b.HasIndex("BaseEntityId");

                    b.ToTable("DependantEntities", (string)null);

                    b.HasData(
                        new
                        {
                            Id = new Guid("80abbca8-664d-4b20-b5de-024705497d4a"),
                            Age = 26,
                            BaseEntityId = new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870"),
                            Name = "Sam Raiden",
                            Position = "Software developer"
                        },
                        new
                        {
                            Id = new Guid("86dba8c0-d178-41e7-938c-ed49778fb52a"),
                            Age = 30,
                            BaseEntityId = new Guid("c9d4c053-49b6-410c-bc78-2d54a9991870"),
                            Name = "Jana McLeaf",
                            Position = "Software developer"
                        },
                        new
                        {
                            Id = new Guid("021ca3c1-0deb-4afd-ae94-2159a8479811"),
                            Age = 35,
                            BaseEntityId = new Guid("3d490a70-94ce-4d15-9494-5248280c2ce3"),
                            Name = "Kane Miller",
                            Position = "Administrator"
                        });
                });

            modelBuilder.Entity("AwakeningLifeBackend.Core.Domain.Entities.SubscriptionFeature", b =>
                {
                    b.Property<Guid>("SubscriptionFeatureId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int?>("FeatureOrder")
                        .HasColumnType("integer");

                    b.Property<string>("FeatureText")
                        .HasColumnType("text");

                    b.Property<bool>("IsIncluded")
                        .HasColumnType("boolean");

                    b.Property<string>("ProductId")
                        .HasColumnType("text");

                    b.HasKey("SubscriptionFeatureId");

                    b.ToTable("SubscriptionFeatures", (string)null);
                });

            modelBuilder.Entity("AwakeningLifeBackend.Core.Domain.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("FirstName")
                        .HasColumnType("text");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("LastName")
                        .HasColumnType("text");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.Property<DateTime>("RefreshTokenExpiryTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<string>("StripeCustomerId")
                        .HasColumnType("text");

                    b.Property<int>("SubscriptionType")
                        .HasColumnType("integer");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "c78cca8c-efa3-4524-ae21-98904dadf303",
                            AccessFailedCount = 0,
                            ConcurrencyStamp = "b8baa1c8-b7eb-4909-bbd5-a3e305a4381e",
                            Email = "admin@equanimity-solutions.com",
                            EmailConfirmed = true,
                            FirstName = "Equanimity",
                            IsDeleted = false,
                            LastName = "Admin",
                            LockoutEnabled = false,
                            NormalizedEmail = "ADMIN@EQUANIMITY-SOLUTIONS.COM",
                            NormalizedUserName = "EQUANIMITY-ADMIN",
                            PasswordHash = "AQAAAAIAAYagAAAAEBabxi2j4ViaUyJTawKOoncZJcJUvtnGSMQStB+D9Qqz8Yc/HfbNvCIW6FUvDM2tPw==",
                            PhoneNumberConfirmed = false,
                            RefreshTokenExpiryTime = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            SecurityStamp = "388fe995-3763-4c99-848b-8699c3131d0d",
                            SubscriptionType = 0,
                            TwoFactorEnabled = false,
                            UserName = "equanimity-admin"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "1fca31b0-e385-46b3-98f2-13f47864589b",
                            Name = "Administrator",
                            NormalizedName = "ADMINISTRATOR"
                        },
                        new
                        {
                            Id = "62227b59-0b0f-43a3-834e-5e2cd3a60ea7",
                            Name = "Modify Users",
                            NormalizedName = "MODIFY USERS"
                        },
                        new
                        {
                            Id = "77e345c0-86e5-4aa6-9744-54b83db0b53d",
                            Name = "View Users",
                            NormalizedName = "VIEW USERS"
                        },
                        new
                        {
                            Id = "6345c99d-8cce-45f1-a876-8a99fe91cd94",
                            Name = "View Roles",
                            NormalizedName = "VIEW ROLES"
                        },
                        new
                        {
                            Id = "5be06b26-d163-4027-84b9-31155c61393b",
                            Name = "View IP",
                            NormalizedName = "VIEW IP"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("RoleId")
                        .HasColumnType("text");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);

                    b.HasData(
                        new
                        {
                            UserId = "c78cca8c-efa3-4524-ae21-98904dadf303",
                            RoleId = "1fca31b0-e385-46b3-98f2-13f47864589b"
                        },
                        new
                        {
                            UserId = "c78cca8c-efa3-4524-ae21-98904dadf303",
                            RoleId = "62227b59-0b0f-43a3-834e-5e2cd3a60ea7"
                        },
                        new
                        {
                            UserId = "c78cca8c-efa3-4524-ae21-98904dadf303",
                            RoleId = "77e345c0-86e5-4aa6-9744-54b83db0b53d"
                        },
                        new
                        {
                            UserId = "c78cca8c-efa3-4524-ae21-98904dadf303",
                            RoleId = "6345c99d-8cce-45f1-a876-8a99fe91cd94"
                        },
                        new
                        {
                            UserId = "c78cca8c-efa3-4524-ae21-98904dadf303",
                            RoleId = "5be06b26-d163-4027-84b9-31155c61393b"
                        });
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("AwakeningLifeBackend.Core.Domain.Entities.DependantEntity", b =>
                {
                    b.HasOne("AwakeningLifeBackend.Core.Domain.Entities.BaseEntity", "BaseEntity")
                        .WithMany("DependantEntities")
                        .HasForeignKey("BaseEntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BaseEntity");
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
                    b.HasOne("AwakeningLifeBackend.Core.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("AwakeningLifeBackend.Core.Domain.Entities.User", null)
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

                    b.HasOne("AwakeningLifeBackend.Core.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("AwakeningLifeBackend.Core.Domain.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AwakeningLifeBackend.Core.Domain.Entities.BaseEntity", b =>
                {
                    b.Navigation("DependantEntities");
                });
#pragma warning restore 612, 618
        }
    }
}
