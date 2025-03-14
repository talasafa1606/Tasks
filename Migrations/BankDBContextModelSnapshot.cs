﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Task1Bank.Data;

#nullable disable

namespace Task1Bank.Migrations
{
    [DbContext(typeof(BankDBContext))]
    partial class BankDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Task1Bank.Entities.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("AccountNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Balance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastUpdatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PreferredLanguage")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AccountNumber")
                        .IsUnique();

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Task1Bank.Entities.AccountTransaction", b =>
                {
                    b.Property<long>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("TransactionId"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("DescriptionDe")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DescriptionFr")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Details")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("FromAccountId")
                        .HasColumnType("integer");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("ToAccountId")
                        .HasColumnType("integer");

                    b.Property<string>("TransactionType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("TransactionId");

                    b.HasIndex("FromAccountId");

                    b.HasIndex("ToAccountId");

                    b.ToTable("AccountTransactions");
                });

            modelBuilder.Entity("Task1Bank.Entities.Log", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<Guid>("RequestId")
                        .HasColumnType("uuid");

                    b.Property<string>("RequestObject")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("RouteURL")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("RequestId");

                    b.HasIndex("RouteURL");

                    b.HasIndex("Timestamp");

                    b.ToTable("Logs");
                });

            modelBuilder.Entity("Task1Bank.Entities.TransactionEvent", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Details")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<long>("TransactionId")
                        .HasColumnType("bigint");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("TransactionEvents");
                });

            modelBuilder.Entity("Task1Bank.Entities.TransactionLog", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("AccountId")
                        .HasColumnType("bigint");

                    b.Property<decimal>("Amount")
                        .HasColumnType("numeric");

                    b.Property<string>("Details")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("Timestamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("TransactionType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("TransactionLogs");
                });

            modelBuilder.Entity("Task1Bank.Entities.AccountTransaction", b =>
                {
                    b.HasOne("Task1Bank.Entities.Account", "FromAccount")
                        .WithMany("Transactions")
                        .HasForeignKey("FromAccountId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Task1Bank.Entities.Account", "ToAccount")
                        .WithMany()
                        .HasForeignKey("ToAccountId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("FromAccount");

                    b.Navigation("ToAccount");
                });

            modelBuilder.Entity("Task1Bank.Entities.Account", b =>
                {
                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
