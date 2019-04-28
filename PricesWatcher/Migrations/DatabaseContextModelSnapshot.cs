﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PricesWatcher.Database;

namespace PricesWatcher.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PricesWatcher.Database.Domain.Hotel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("HotelCode");

                    b.Property<float>("HotelStandard");

                    b.HasKey("Id");

                    b.ToTable("Hotels");
                });

            modelBuilder.Entity("PricesWatcher.Database.Domain.Offer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Board");

                    b.Property<string>("Code");

                    b.Property<DateTime>("DepartureDateTime");

                    b.Property<int?>("HotelForeignKey");

                    b.Property<DateTime>("ReturnDate");

                    b.Property<string>("RoomCode");

                    b.Property<string>("RoomName");

                    b.HasKey("Id");

                    b.HasIndex("HotelForeignKey");

                    b.ToTable("Offers");
                });

            modelBuilder.Entity("PricesWatcher.Database.Domain.Price", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<float>("DiscountPrice");

                    b.Property<int?>("OfferForeignKey");

                    b.Property<float>("StandardPrice");

                    b.Property<DateTime>("Timestamp");

                    b.HasKey("Id");

                    b.HasIndex("OfferForeignKey");

                    b.ToTable("Prices");
                });

            modelBuilder.Entity("PricesWatcher.Database.Domain.TripAdvisorHotelRating", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("HotelForeignKey");

                    b.Property<DateTime>("Timestamp");

                    b.Property<float>("TripAdvisorRating");

                    b.Property<int>("TripAdvisorReviewsNo");

                    b.HasKey("Id");

                    b.HasIndex("HotelForeignKey");

                    b.ToTable("TripAdvisorHotelRatings");
                });

            modelBuilder.Entity("PricesWatcher.Database.Domain.Offer", b =>
                {
                    b.HasOne("PricesWatcher.Database.Domain.Hotel", "Hotel")
                        .WithMany()
                        .HasForeignKey("HotelForeignKey");
                });

            modelBuilder.Entity("PricesWatcher.Database.Domain.Price", b =>
                {
                    b.HasOne("PricesWatcher.Database.Domain.Offer", "Offer")
                        .WithMany()
                        .HasForeignKey("OfferForeignKey");
                });

            modelBuilder.Entity("PricesWatcher.Database.Domain.TripAdvisorHotelRating", b =>
                {
                    b.HasOne("PricesWatcher.Database.Domain.Hotel", "Hotel")
                        .WithMany()
                        .HasForeignKey("HotelForeignKey");
                });
#pragma warning restore 612, 618
        }
    }
}
