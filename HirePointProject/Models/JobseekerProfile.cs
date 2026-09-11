using System.ComponentModel.DataAnnotations;

namespace HirePoint.Models
{
    public class JobseekerProfile
    {
        // =========================
        // PRIMARY KEY
        // =========================

        [Key]
        public int UserID { get; set; }


        // =========================
        // QUALIFICATION
        // =========================

        [Required(ErrorMessage = "Please enter your qualification.")]
        [StringLength(150, ErrorMessage = "Qualification cannot exceed 150 characters.")]
        public string Qualification { get; set; } = string.Empty;


        // =========================
        // SKILLS
        // =========================

        [Required(ErrorMessage = "Please enter your skills.")]
        [StringLength(2000, ErrorMessage = "Skills cannot exceed 2000 characters.")]
        public string Skills { get; set; } = string.Empty;


        // =========================
        // EDUCATION
        // =========================

        [Required(ErrorMessage = "Please enter your education.")]
        [StringLength(2000, ErrorMessage = "Education cannot exceed 2000 characters.")]
        public string Education { get; set; } = string.Empty;


        // =========================
        // EXPERIENCE
        // =========================

        [Required(ErrorMessage = "Please enter your work experience.")]
        [StringLength(3000, ErrorMessage = "Experience cannot exceed 3000 characters.")]
        public string Experience { get; set; } = string.Empty;


        // =========================
        // CV
        // =========================

        [StringLength(500, ErrorMessage = "CV file path cannot exceed 500 characters.")]
        public string? CVFile { get; set; }


        // =========================
        // USER RELATIONSHIP
        // =========================

        [Required]
        public User User { get; set; } = null!;
    }
}