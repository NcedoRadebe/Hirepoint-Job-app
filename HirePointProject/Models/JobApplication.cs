using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HirePoint.Models
{
    public class JobApplication
    {
        [Key]
        public int ApplicationID { get; set; }


        // =====================================================
        // RELATIONSHIP IDs
        // =====================================================

        [Required]
        public int JobID { get; set; }


        [Required]
        public int JobseekerID { get; set; }


        // =====================================================
        // APPLICATION STATUS
        // =====================================================

        [Required]
        [StringLength(30)]
        public string ApplicationStatus { get; set; } = "Pending";


        // =====================================================
        // DECISION
        // =====================================================

        [StringLength(
            1000,
            ErrorMessage = "Decision message cannot exceed 1000 characters."
        )]
        public string? DecisionMessage { get; set; }


        public DateTime? DecisionDate { get; set; }


        // =====================================================
        // INTERVIEW
        // =====================================================

        public DateTime? InterviewDate { get; set; }


        [StringLength(
            20,
            ErrorMessage = "Interview time cannot exceed 20 characters."
        )]
        public string? InterviewTime { get; set; }


        [StringLength(
            50,
            ErrorMessage = "Interview type cannot exceed 50 characters."
        )]
        public string? InterviewType { get; set; }


        [StringLength(
            250,
            ErrorMessage = "Interview location cannot exceed 250 characters."
        )]
        public string? InterviewLocation { get; set; }


        [StringLength(
            2000,
            ErrorMessage = "Interview notes cannot exceed 2000 characters."
        )]
        public string? InterviewNotes { get; set; }


        // =====================================================
        // APPLICATION DATE
        // =====================================================

        [Required]
        public DateTime ApplicationDate { get; set; } = DateTime.Now;


        // =====================================================
        // RELATIONSHIPS
        // =====================================================

        [ForeignKey(nameof(JobID))]
        public Job? Job { get; set; }


        [ForeignKey(nameof(JobseekerID))]
        public User? Jobseeker { get; set; }
    }
}