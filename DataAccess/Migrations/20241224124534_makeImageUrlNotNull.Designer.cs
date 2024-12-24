﻿// <auto-generated />
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace UI.Migrations
{
    [DbContext(typeof(AppDBContext))]
    [Migration("20241224124534_makeImageUrlNotNull")]
    partial class makeImageUrlNotNull
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Models.Category", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("ID");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            ID = 1,
                            DisplayOrder = 1,
                            Name = "Maths"
                        },
                        new
                        {
                            ID = 2,
                            DisplayOrder = 2,
                            Name = "Science"
                        },
                        new
                        {
                            ID = 3,
                            DisplayOrder = 3,
                            Name = "Language"
                        });
                });

            modelBuilder.Entity("Models.Product", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ID"));

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CategoryID")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ISBN")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ListPrice")
                        .HasColumnType("float");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<double>("Price100")
                        .HasColumnType("float");

                    b.Property<double>("Price50")
                        .HasColumnType("float");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ID");

                    b.HasIndex("CategoryID");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            ID = 1,
                            Author = "Carlson Ben",
                            CategoryID = 1,
                            Description = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum.",
                            ISBN = "SWD999001",
                            ImageUrl = "",
                            ListPrice = 99.0,
                            Price = 90.0,
                            Price100 = 80.0,
                            Price50 = 85.0,
                            Title = "Fortune of Time"
                        },
                        new
                        {
                            ID = 2,
                            Author = "Benjamin Franklin",
                            CategoryID = 3,
                            Description = "etetur sadipscing elitrut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum.",
                            ISBN = "SWD999002",
                            ImageUrl = "",
                            ListPrice = 119.0,
                            Price = 109.0,
                            Price100 = 95.0,
                            Price50 = 100.0,
                            Title = "How to avoid war"
                        },
                        new
                        {
                            ID = 3,
                            Author = "Iminabo Tombo",
                            CategoryID = 2,
                            Description = "Tempor invidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum.",
                            ISBN = "SWD999003",
                            ImageUrl = "",
                            ListPrice = 89.0,
                            Price = 80.0,
                            Price100 = 70.0,
                            Price50 = 75.0,
                            Title = "Religious Freedom"
                        },
                        new
                        {
                            ID = 4,
                            Author = "Kroma Belema",
                            CategoryID = 1,
                            Description = "Dolor sit ametaliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum.",
                            ISBN = "SWD999004",
                            ImageUrl = "",
                            ListPrice = 55.0,
                            Price = 50.0,
                            Price100 = 40.0,
                            Price50 = 45.0,
                            Title = "Marital Success"
                        },
                        new
                        {
                            ID = 5,
                            Author = "Biokpo Alabo",
                            CategoryID = 2,
                            Description = "At vero eos et accusam et justo duo dolores et ea rebum.",
                            ISBN = "SWD999005",
                            ImageUrl = "",
                            ListPrice = 99.0,
                            Price = 70.0,
                            Price100 = 60.0,
                            Price50 = 65.0,
                            Title = "Financial Success"
                        },
                        new
                        {
                            ID = 6,
                            Author = "Endurance Goodwill",
                            CategoryID = 3,
                            Description = "Lnvidunt ut labore et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum.",
                            ISBN = "SWD999006",
                            ImageUrl = "",
                            ListPrice = 99.0,
                            Price = 90.0,
                            Price100 = 80.0,
                            Price50 = 85.0,
                            Title = "Starts of a Firm"
                        });
                });

            modelBuilder.Entity("Models.Product", b =>
                {
                    b.HasOne("Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });
#pragma warning restore 612, 618
        }
    }
}
