﻿// <auto-generated />
using System;
using Kiss.Bff.Beheer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Kiss.Bff.NieuwsEnWerkinstructies.Migrations
{
    [DbContext(typeof(BeheerDbContext))]
    [Migration("20230809124340_Remove-KlantContactemomentTable")]
    partial class RemoveKlantContactemomentTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("BerichtSkill", b =>
                {
                    b.Property<int>("BerichtenId")
                        .HasColumnType("integer");

                    b.Property<int>("SkillsId")
                        .HasColumnType("integer");

                    b.HasKey("BerichtenId", "SkillsId");

                    b.HasIndex("SkillsId");

                    b.ToTable("BerichtSkill");
                });

            modelBuilder.Entity("Kiss.Bff.Beheer.Gespreksresultaten.Data.Entities.Gespreksresultaat", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("DateCreated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("DateUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Definitie")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Definitie")
                        .IsUnique();

                    b.ToTable("Gespreksresultaten");
                });

            modelBuilder.Entity("Kiss.Bff.Beheer.Links.Data.Entities.Link", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Categorie")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("DateCreated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("DateUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Titel")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Links");
                });

            modelBuilder.Entity("Kiss.Bff.Beheer.Verwerking.VerwerkingsLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("ApiEndpoint")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("InsertedAt")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("timestamp with time zone")
                        .HasDefaultValueSql("NOW()");

                    b.Property<string>("Method")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("VerwerkingsLogs");
                });

            modelBuilder.Entity("Kiss.Bff.NieuwsEnWerkinstructies.Data.Entities.Bericht", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("DateCreated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("DateUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Inhoud")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsBelangrijk")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset>("PublicatieDatum")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("PublicatieEinddatum")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Titel")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.HasKey("Id");

                    b.HasIndex("Type");

                    b.ToTable("Berichten");
                });

            modelBuilder.Entity("Kiss.Bff.NieuwsEnWerkinstructies.Data.Entities.BerichtGelezen", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("text");

                    b.Property<int>("BerichtId")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("GelezenOp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("UserId", "BerichtId");

                    b.HasIndex("BerichtId");

                    b.ToTable("Gelezen");
                });

            modelBuilder.Entity("Kiss.Bff.NieuwsEnWerkinstructies.Data.Entities.Skill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTimeOffset>("DateCreated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("DateUpdated")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Naam")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Skills");
                });

            modelBuilder.Entity("Kiss.Bff.ZaakGerichtWerken.Contactmomenten.ContactmomentDetails", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("Einddatum")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("EmailadresKcm")
                        .HasColumnType("text");

                    b.Property<string>("Gespreksresultaat")
                        .HasColumnType("text");

                    b.Property<string>("SpecifiekeVraag")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("Startdatum")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Vraag")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("Vraag");

                    b.ToTable("ContactMomentDetails");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.DataProtection.EntityFrameworkCore.DataProtectionKey", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FriendlyName")
                        .HasColumnType("text");

                    b.Property<string>("Xml")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("DataProtectionKeys");
                });

            modelBuilder.Entity("BerichtSkill", b =>
                {
                    b.HasOne("Kiss.Bff.NieuwsEnWerkinstructies.Data.Entities.Bericht", null)
                        .WithMany()
                        .HasForeignKey("BerichtenId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Kiss.Bff.NieuwsEnWerkinstructies.Data.Entities.Skill", null)
                        .WithMany()
                        .HasForeignKey("SkillsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Kiss.Bff.NieuwsEnWerkinstructies.Data.Entities.BerichtGelezen", b =>
                {
                    b.HasOne("Kiss.Bff.NieuwsEnWerkinstructies.Data.Entities.Bericht", null)
                        .WithMany("Gelezen")
                        .HasForeignKey("BerichtId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Kiss.Bff.NieuwsEnWerkinstructies.Data.Entities.Bericht", b =>
                {
                    b.Navigation("Gelezen");
                });
#pragma warning restore 612, 618
        }
    }
}
