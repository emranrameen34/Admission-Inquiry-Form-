using Microsoft.EntityFrameworkCore;
using AdmissionInquiryRecord.Models;

namespace AdmissionInquiryRecord.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Counsellor> Counsellors { get; set; }        
        public DbSet<CounsellingRecord> CounsellingRecords { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Tehsil> Tehsils { get; set; }
        public DbSet<AcademicProgram> Programs { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<LearnSource> LearnSources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: configure relationships
            modelBuilder.Entity<CounsellingRecord>()
                .HasOne(c => c.Person)
                .WithMany()
                .HasForeignKey(c => c.PersonId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CounsellingRecord>()
                .HasOne(c => c.Counsellor)
                .WithMany()
                .HasForeignKey(c => c.CounsellorId)
                .OnDelete(DeleteBehavior.Restrict);
        }



    }
}