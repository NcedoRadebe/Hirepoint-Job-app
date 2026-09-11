using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HirePoint.Models
{
    public class RecruiterProfile
    {
        // =========================
        // PRIMARY KEY / FOREIGN KEY
        // =========================

        [Key]
        [ForeignKey(nameof(User))]
        public int UserID { get; set; }


        // =========================
        // COMPANY INFORMATION
        // =========================

        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(
            150,
            MinimumLength = 2,
            ErrorMessage = "Company name must be between 2 and 150 characters."
        )]
        public string CompanyName { get; set; } = string.Empty;


        [StringLength(
            2000,
            ErrorMessage = "Company description cannot exceed 2000 characters."
        )]
        public string? CompanyDescription { get; set; }


        [Url(ErrorMessage = "Please enter a valid website URL.")]
        [StringLength(
            300,
            ErrorMessage = "Website cannot exceed 300 characters."
        )]
        public string? Website { get; set; }


        // =========================
        // RELATIONSHIP
        // =========================

        public User User { get; set; } = null!;
    }
}