﻿// <auto-generated />
using DST.Bot.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DST.Bot.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241227085547_RemoveObsoleteDialogStateIdField")]
    partial class RemoveObsoleteDialogStateIdField
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.11");

            modelBuilder.Entity("DST.Bot.Entities.FrontPageData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Course")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Group")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Initials")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Profile")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("SupervisorAcademicDegree")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("SupervisorAcademicTitle")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("SupervisorInitials")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("SupervisorJobTitle")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Theme")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<long>("UserId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Year")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("FrontPageData");
                });

            modelBuilder.Entity("DST.Bot.Entities.GenerateTopicData", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Country")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Language")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Scope")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("TimePeriod")
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<long>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("GenerateTopicData");
                });

            modelBuilder.Entity("DST.Bot.Entities.User", b =>
                {
                    b.Property<long>("ChatId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ArticleSearchTerm")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("DialogState")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<int>("PsychologicalTestPoints")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(0);

                    b.HasKey("ChatId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("DST.Bot.Entities.FrontPageData", b =>
                {
                    b.HasOne("DST.Bot.Entities.User", "User")
                        .WithOne("FrontPageData")
                        .HasForeignKey("DST.Bot.Entities.FrontPageData", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DST.Bot.Entities.GenerateTopicData", b =>
                {
                    b.HasOne("DST.Bot.Entities.User", "User")
                        .WithOne("GenerateTopicData")
                        .HasForeignKey("DST.Bot.Entities.GenerateTopicData", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("DST.Bot.Entities.User", b =>
                {
                    b.Navigation("FrontPageData")
                        .IsRequired();

                    b.Navigation("GenerateTopicData")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
