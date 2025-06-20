﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using dfa_device_manager.API;

#nullable disable

namespace dfa_device_manager.API.Migrations
{
    [DbContext(typeof(DfaDeviceManagerContext))]
    [Migration("20250604094720_AddAccountAndRole")]
    partial class AddAccountAndRole
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.16")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("dfa_device_manager.API.Account", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("varchar(500)");

                    b.Property<int>("RoleId")
                        .HasColumnType("int");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(150)
                        .IsUnicode(false)
                        .HasColumnType("varchar(150)");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.HasIndex("RoleId");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Account", (string)null);
                });

            modelBuilder.Entity("dfa_device_manager.API.Device", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("AdditionalProperties")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(8000)
                        .IsUnicode(false)
                        .HasColumnType("varchar(8000)")
                        .HasDefaultValue("");

                    b.Property<int?>("DeviceTypeId")
                        .HasColumnType("int");

                    b.Property<bool>("IsEnabled")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(150)
                        .IsUnicode(false)
                        .HasColumnType("varchar(150)");

                    b.HasKey("Id");

                    b.HasIndex("DeviceTypeId");

                    b.ToTable("Device", (string)null);
                });

            modelBuilder.Entity("dfa_device_manager.API.DeviceEmployee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("DeviceId")
                        .HasColumnType("int");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("IssueDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(sysutcdatetime())");

                    b.Property<DateTime?>("ReturnDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("DeviceId");

                    b.HasIndex("EmployeeId");

                    b.ToTable("DeviceEmployee", (string)null);
                });

            modelBuilder.Entity("dfa_device_manager.API.DeviceType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Name" }, "UQ__DeviceTy__737584F64C648D0D")
                        .IsUnique();

                    b.ToTable("DeviceType", (string)null);
                });

            modelBuilder.Entity("dfa_device_manager.API.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("HireDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("(sysutcdatetime())");

                    b.Property<int>("PersonId")
                        .HasColumnType("int");

                    b.Property<int>("PositionId")
                        .HasColumnType("int");

                    b.Property<decimal>("Salary")
                        .HasColumnType("decimal(18, 2)");

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.HasIndex("PositionId");

                    b.ToTable("Employee", (string)null);
                });

            modelBuilder.Entity("dfa_device_manager.API.Person", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(150)
                        .IsUnicode(false)
                        .HasColumnType("varchar(150)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("MiddleName")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("PassportNumber")
                        .IsRequired()
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("varchar(30)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "PassportNumber" }, "UQ__Person__45809E718F531216")
                        .IsUnique();

                    b.HasIndex(new[] { "PhoneNumber" }, "UQ__Person__85FB4E38600D2AC1")
                        .IsUnique();

                    b.HasIndex(new[] { "Email" }, "UQ__Person__A9D105340F923EE2")
                        .IsUnique();

                    b.ToTable("Person", (string)null);
                });

            modelBuilder.Entity("dfa_device_manager.API.Position", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("MinExpYears")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "Name" }, "UQ__Position__737584F6B4CDEB70")
                        .IsUnique();

                    b.ToTable("Position", (string)null);
                });

            modelBuilder.Entity("dfa_device_manager.API.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Role", (string)null);

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Admin"
                        },
                        new
                        {
                            Id = 2,
                            Name = "User"
                        });
                });

            modelBuilder.Entity("dfa_device_manager.API.Account", b =>
                {
                    b.HasOne("dfa_device_manager.API.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("dfa_device_manager.API.Role", "Role")
                        .WithMany("Accounts")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Employee");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("dfa_device_manager.API.Device", b =>
                {
                    b.HasOne("dfa_device_manager.API.DeviceType", "DeviceType")
                        .WithMany("Devices")
                        .HasForeignKey("DeviceTypeId")
                        .HasConstraintName("FK_Device_DeviceType");

                    b.Navigation("DeviceType");
                });

            modelBuilder.Entity("dfa_device_manager.API.DeviceEmployee", b =>
                {
                    b.HasOne("dfa_device_manager.API.Device", "Device")
                        .WithMany("DeviceEmployees")
                        .HasForeignKey("DeviceId")
                        .IsRequired()
                        .HasConstraintName("FK_DeviceEmployee_Device");

                    b.HasOne("dfa_device_manager.API.Employee", "Employee")
                        .WithMany("DeviceEmployees")
                        .HasForeignKey("EmployeeId")
                        .IsRequired()
                        .HasConstraintName("FK_DeviceEmployee_Employee");

                    b.Navigation("Device");

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("dfa_device_manager.API.Employee", b =>
                {
                    b.HasOne("dfa_device_manager.API.Person", "Person")
                        .WithMany("Employees")
                        .HasForeignKey("PersonId")
                        .IsRequired()
                        .HasConstraintName("FK_Employee_Person");

                    b.HasOne("dfa_device_manager.API.Position", "Position")
                        .WithMany("Employees")
                        .HasForeignKey("PositionId")
                        .IsRequired()
                        .HasConstraintName("FK_Employee_Position");

                    b.Navigation("Person");

                    b.Navigation("Position");
                });

            modelBuilder.Entity("dfa_device_manager.API.Device", b =>
                {
                    b.Navigation("DeviceEmployees");
                });

            modelBuilder.Entity("dfa_device_manager.API.DeviceType", b =>
                {
                    b.Navigation("Devices");
                });

            modelBuilder.Entity("dfa_device_manager.API.Employee", b =>
                {
                    b.Navigation("DeviceEmployees");
                });

            modelBuilder.Entity("dfa_device_manager.API.Person", b =>
                {
                    b.Navigation("Employees");
                });

            modelBuilder.Entity("dfa_device_manager.API.Position", b =>
                {
                    b.Navigation("Employees");
                });

            modelBuilder.Entity("dfa_device_manager.API.Role", b =>
                {
                    b.Navigation("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}
