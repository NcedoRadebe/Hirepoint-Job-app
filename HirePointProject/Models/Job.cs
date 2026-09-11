using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HirePoint.Models
{
    public class Job
    {
        // =====================================================
        // PRIMARY KEY
        // =====================================================

        [Key]
        public int JobID { get; set; }


        // =====================================================
        // RECRUITER
        // =====================================================

        [Required]
        public int RecruiterID { get; set; }

        [ForeignKey(nameof(RecruiterID))]
        public User Recruiter { get; set; } = null!;


        // =====================================================
        // COMPANY
        // =====================================================

        public int? CompanyID { get; set; }

        [ForeignKey(nameof(CompanyID))]
        public Company? Company { get; set; }


        // =====================================================
        // QUALIFICATION
        // =====================================================

        [Required(ErrorMessage = "Qualification is required.")]
        public int? QualificationID { get; set; }
        [ForeignKey(nameof(QualificationID))]
        public Qualification Qualification { get; set; } = null!;
        // =====================================================
        // SKILLS
        // =====================================================

        [Required(ErrorMessage = "Job skills are required.")]
        [StringLength(
            2000,
            MinimumLength = 2,
            ErrorMessage = "Job skills must be between 2 and 2000 characters."
        )]
        public string Skills { get; set; } = string.Empty;


        // =====================================================
        // JOB INFORMATION
        // =====================================================

        [Required(ErrorMessage = "Job title is required.")]
        [StringLength(
            150,
            MinimumLength = 3,
            ErrorMessage = "Job title must be between 3 and 150 characters."
        )]
        public string JobTitle { get; set; } = string.Empty;


        [Required(ErrorMessage = "Job description is required.")]
        [StringLength(
            5000,
            MinimumLength = 20,
            ErrorMessage = "Job description must be between 20 and 5000 characters."
        )]
        public string JobDescription { get; set; } = string.Empty;


        [Required(ErrorMessage = "Job requirements are required.")]
        [StringLength(
            3000,
            MinimumLength = 5,
            ErrorMessage = "Requirements must be between 5 and 3000 characters."
        )]
        public string Requirements { get; set; } = string.Empty;


        // =====================================================
        // SALARY
        // =====================================================

        [Required(ErrorMessage = "Salary range is required.")]
        [StringLength(
            100,
            ErrorMessage = "Salary range cannot exceed 100 characters."
        )]
        public string Salary { get; set; } = string.Empty;


        // =====================================================
        // LOCATION
        // =====================================================

        [Required(ErrorMessage = "Province is required.")]
        [StringLength(100)]
        public string Province { get; set; } = string.Empty;


        [Required(ErrorMessage = "City is required.")]
        [StringLength(
            100,
            MinimumLength = 2,
            ErrorMessage = "City must be between 2 and 100 characters."
        )]
        public string City { get; set; } = string.Empty;


        // =====================================================
        // EMPLOYMENT
        // =====================================================

        [Required(ErrorMessage = "Employment type is required.")]
        [StringLength(
            50,
            ErrorMessage = "Employment type cannot exceed 50 characters."
        )]
        public string EmploymentType { get; set; } = string.Empty;


        [Required(ErrorMessage = "Experience level is required.")]
        [StringLength(
            50,
            ErrorMessage = "Experience level cannot exceed 50 characters."
        )]
        public string ExperienceLevel { get; set; } = string.Empty;


        // =====================================================
        // DATES
        // =====================================================

        [Required(ErrorMessage = "Closing date is required.")]
        public DateTime ClosingDate { get; set; }


        [Required]
        public DateTime DatePosted { get; set; } = DateTime.Now;


        // =====================================================
        // STATUS
        // =====================================================

        [Required(ErrorMessage = "Job status is required.")]
        [StringLength(
            30,
            ErrorMessage = "Job status cannot exceed 30 characters."
        )]
        public string JobStatus { get; set; } = "Active";
    }
}