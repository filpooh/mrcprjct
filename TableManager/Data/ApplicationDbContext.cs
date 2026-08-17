using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TableManager.Models;

namespace TableManager.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        //public DbSet<TableProp> TableProps { get; set; }
        public DbSet<FileCsv> FileCsvs { get; set; }
        public DbSet<CsvRow> CsvRows { get; set; }
        //public DbSet<Header> Headers { get; set; }
        //public DbSet<TableRow> TableRows { get; set; }
        //public DbSet<Cell> Cells { get; set; }
        public DbSet<MlCsv> MlCsv { get; set; }
        public DbSet<MlCsvRow> MlCsvRows { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Statistics> Statistics { get; set; }
        /*
         dotnet ef migrations add NomeMigration 
        update-database

        per svuotare i dati elimino tutto e ricreo
        dotnet ef database drop
        dotnet ef database update

         */
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // FK verso ApplicationUser → NO CASCADE
            builder.Entity<FileCsv>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<MlCsv>()
                .HasOne(m => m.User)
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<MlCsv>()
                .HasOne(m => m.Setting)
                .WithMany()
                .HasForeignKey(m => m.SettingId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cascade interno FileCsv → CsvRows
            builder.Entity<CsvRow>()
                .HasOne(r => r.File)
                .WithMany(f => f.Rows)
                .HasForeignKey(r => r.FileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cascade interno MlCsv → MlCsvRows
            builder.Entity<MlCsvRow>()
                .HasOne(r => r.MlCsv)
                .WithMany(m => m.Rows)
                .HasForeignKey(r => r.MlCsvId)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}