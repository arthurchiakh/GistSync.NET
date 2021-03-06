// <auto-generated />
using System;
using GistSync.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GistSync.Core.Migrations
{
    [DbContext(typeof(GistSyncDbContext))]
    [Migration("20210919032809_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.0-rc.1.21452.10");

            modelBuilder.Entity("GistSync.Core.Models.SyncTask", b =>
                {
                    b.Property<string>("GistId")
                        .HasColumnType("TEXT");

                    b.Property<string>("FileChecksum")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("GistFileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("GitHubPersonalAccessToken")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Directory")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("SyncMode")
                        .HasColumnType("INTEGER");

                    b.HasKey("GistId");

                    b.ToTable("SyncTasks", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
