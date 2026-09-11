using HirePoint.Models;
using Microsoft.EntityFrameworkCore;

namespace HirePoint.Data
{
    public class HirePointDbContext : DbContext
    {
        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public HirePointDbContext(
            DbContextOptions<HirePointDbContext> options)
            : base(options)
        {
        }


        // =========================================================
        // TABLES
        // =========================================================

        public DbSet<User> Users { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<JobApplication> Applications { get; set; }

        public DbSet<JobseekerProfile> JobseekerProfiles { get; set; }

        public DbSet<RecruiterProfile> RecruiterProfiles { get; set; }

        public DbSet<UserActivity> UserActivity { get; set; }

        public DbSet<Province> Provinces { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<JobInterest> JobInterests { get; set; }

        public DbSet<JobseekerInterest> JobseekerInterests { get; set; }

        public DbSet<Qualification> Qualifications { get; set; }

        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }


        // =========================================================
        // MODEL CONFIGURATION
        // =========================================================

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =====================================================
            // USER
            // =====================================================

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.UserID);

                entity.Property(x => x.UserID)
                    .HasColumnName("UserID");

                entity.Property(x => x.FullName)
                    .HasColumnName("FullName")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(x => x.Email)
                    .HasColumnName("Email")
                    .HasMaxLength(254)
                    .IsRequired();

                entity.Property(x => x.PasswordHash)
                    .HasColumnName("PasswordHash")
                    .IsRequired();

                // Phone number is OPTIONAL
                entity.Property(x => x.PhoneNumber)
                    .HasColumnName("PhoneNumber")
                    .HasMaxLength(20);

                entity.Property(x => x.ProvinceID)
                    .HasColumnName("ProvinceID");

                entity.Property(x => x.CompanyID)
                    .HasColumnName("CompanyID");

                entity.HasOne(x => x.Company)
                    .WithMany(x => x.Users)
                    .HasForeignKey(x => x.CompanyID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(x => x.UserType)
                    .HasColumnName("UserType")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(x => x.Status)
                    .HasColumnName("Status")
                    .HasMaxLength(30)
                    .IsRequired();

                entity.Property(x => x.IsApproved)
                    .HasColumnName("IsApproved");

                entity.Property(x => x.EmailConfirmed)
                    .HasColumnName("EmailConfirmed");

                entity.Property(x => x.DateCreated)
                    .HasColumnName("DateCreated");

                entity.Property(x => x.Gender)
                    .HasColumnName("Gender")
                    .HasMaxLength(20);

                entity.HasIndex(x => x.Email)
                    .IsUnique();

                entity.HasOne(x => x.Province)
                    .WithMany(x => x.Users)
                    .HasForeignKey(x => x.ProvinceID)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            // =====================================================
            // PASSWORD RESET TOKEN
            // =====================================================

            modelBuilder.Entity<PasswordResetToken>(entity =>
            {
                entity.HasKey(x => x.PasswordResetTokenID);

                entity.Property(x => x.TokenHash)
                    .HasMaxLength(128)
                    .IsRequired();

                entity.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(x => x.TokenHash)
                    .IsUnique();
            });


            // =====================================================
            // COMPANY
            // =====================================================

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(x => x.CompanyID);

                entity.Property(x => x.CompanyName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(x => x.CompanyDescription)
                    .HasMaxLength(5000);

                entity.Property(x => x.Website)
                    .HasMaxLength(500);

                entity.Property(x => x.Email)
                    .HasMaxLength(254);

                entity.Property(x => x.PhoneNumber)
                    .HasMaxLength(20);

                entity.Property(x => x.Address)
                    .HasMaxLength(500);

                entity.HasOne(x => x.Province)
                    .WithMany()
                    .HasForeignKey(x => x.ProvinceID)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            // =====================================================
            // JOB -> COMPANY
            // =====================================================

            modelBuilder.Entity<Job>()
                .HasOne(x => x.Company)
                .WithMany(x => x.Jobs)
                .HasForeignKey(x => x.CompanyID)
                .OnDelete(DeleteBehavior.Restrict);


            // =====================================================
            // NOTIFICATIONS
            // =====================================================

            modelBuilder.Entity<Notification>()
                .HasOne(x => x.User)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Cascade);


            // =====================================================
            // RECRUITER PROFILE
            // =====================================================

            modelBuilder.Entity<RecruiterProfile>()
                .HasKey(x => x.UserID);

            modelBuilder.Entity<RecruiterProfile>()
                .HasOne(x => x.User)
                .WithOne(x => x.RecruiterProfile)
                .HasForeignKey<RecruiterProfile>(x => x.UserID)
                .OnDelete(DeleteBehavior.Cascade);


            // =====================================================
            // JOBSEEKER PROFILE
            // =====================================================

            modelBuilder.Entity<JobseekerProfile>(entity =>
            {
                entity.HasKey(x => x.UserID);

                entity.HasOne(x => x.User)
                    .WithOne(x => x.JobseekerProfile)
                    .HasForeignKey<JobseekerProfile>(x => x.UserID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(x => x.Qualification)
                    .HasColumnName("Qualification")
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(x => x.Skills)
                    .HasColumnName("Skills")
                    .HasMaxLength(2000)
                    .IsRequired();

                entity.Property(x => x.Education)
                    .HasColumnName("Education")
                    .HasMaxLength(2000)
                    .IsRequired();

                entity.Property(x => x.Experience)
                    .HasColumnName("Experience")
                    .HasMaxLength(3000)
                    .IsRequired();

                entity.Property(x => x.CVFile)
                    .HasColumnName("CVFile")
                    .HasMaxLength(500);
            });


            // =====================================================
            // QUALIFICATION -> JOB
            // =====================================================

            modelBuilder.Entity<Job>()
                .HasOne(x => x.Qualification)
                .WithMany(x => x.Jobs)
                .HasForeignKey(x => x.QualificationID)
                .OnDelete(DeleteBehavior.Restrict);


            // =====================================================
            // RECRUITER -> JOBS
            // =====================================================

            modelBuilder.Entity<Job>()
                .HasOne(x => x.Recruiter)
                .WithMany(x => x.Jobs)
                .HasForeignKey(x => x.RecruiterID)
                .OnDelete(DeleteBehavior.Restrict);


            // =====================================================
            // APPLICATION -> JOB
            // =====================================================

            modelBuilder.Entity<JobApplication>()
                .HasOne(x => x.Job)
                .WithMany()
                .HasForeignKey(x => x.JobID)
                .OnDelete(DeleteBehavior.Cascade);


            // =====================================================
            // APPLICATION -> JOBSEEKER
            // =====================================================

            modelBuilder.Entity<JobApplication>()
                .HasOne(x => x.Jobseeker)
                .WithMany()
                .HasForeignKey(x => x.JobseekerID)
                .OnDelete(DeleteBehavior.Restrict);


            // =====================================================
            // JOB INTEREST
            // =====================================================

            modelBuilder.Entity<JobInterest>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                    .HasMaxLength(100)
                    .IsRequired();

                // Prevent duplicate interest names
                entity.HasIndex(x => x.Name)
                    .IsUnique();
            });


            // =====================================================
            // JOBSEEKER INTEREST
            // =====================================================

            modelBuilder.Entity<JobseekerInterest>(entity =>
            {
                entity.HasKey(x => x.Id);

                // JobseekerInterest -> User
                entity.HasOne(x => x.Jobseeker)
                    .WithMany()
                    .HasForeignKey(x => x.JobseekerId)
                    .HasPrincipalKey(x => x.UserID)
                    .OnDelete(DeleteBehavior.Cascade);

                // JobseekerInterest -> JobInterest
                entity.HasOne(x => x.JobInterest)
                    .WithMany(x => x.JobseekerInterests)
                    .HasForeignKey(x => x.JobInterestId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Prevent duplicate interests for the same jobseeker
                entity.HasIndex(x => new
                {
                    x.JobseekerId,
                    x.JobInterestId
                })
                .IsUnique();
            });


            // =====================================================
            // SEED JOB INTERESTS
            // =====================================================

            modelBuilder.Entity<JobInterest>().HasData(

                new JobInterest
                {
                    Id = 1,
                    Name = "Software Developer"
                },

                new JobInterest
                {
                    Id = 2,
                    Name = "Web Developer"
                },

                new JobInterest
                {
                    Id = 3,
                    Name = "Mobile App Developer"
                },

                new JobInterest
                {
                    Id = 4,
                    Name = "Data Analyst"
                },

                new JobInterest
                {
                    Id = 5,
                    Name = "Database Administrator"
                },

                new JobInterest
                {
                    Id = 6,
                    Name = "Network Administrator"
                },

                new JobInterest
                {
                    Id = 7,
                    Name = "Cybersecurity"
                },

                new JobInterest
                {
                    Id = 8,
                    Name = "IT Support"
                },

                new JobInterest
                {
                    Id = 9,
                    Name = "Graphic Designer"
                },

                new JobInterest
                {
                    Id = 10,
                    Name = "UI/UX Designer"
                },

                new JobInterest
                {
                    Id = 11,
                    Name = "Accountant"
                },

                new JobInterest
                {
                    Id = 12,
                    Name = "Human Resources"
                },

                new JobInterest
                {
                    Id = 13,
                    Name = "Marketing"
                },

                new JobInterest
                {
                    Id = 14,
                    Name = "Sales"
                },

                new JobInterest
                {
                    Id = 15,
                    Name = "Project Manager"
                }
            );
        }
    }
}
