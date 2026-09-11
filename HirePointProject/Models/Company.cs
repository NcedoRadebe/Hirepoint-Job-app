using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HirePoint.Models
{
    public class Company
    {
        // =====================================================
        // PRIMARY KEY
        // =====================================================

        [Key]
        public int CompanyID { get; set; }


        // =====================================================
        // COMPANY INFORMATION
        // =====================================================

        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(200, MinimumLength = 2)]
        public string CompanyName { get; set; } = string.Empty;


        [StringLength(5000)]
        public string? CompanyDescription { get; set; }


        [Url(ErrorMessage = "Please enter a valid website URL.")]
        [StringLength(500)]
        public string? Website { get; set; }


        [EmailAddress]
        [StringLength(254)]
        public string? Email { get; set; }


        [Phone]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }


        [StringLength(500)]
        public string? Address { get; set; }


        // =====================================================
        // PROVINCE
        // =====================================================

        public int? ProvinceID { get; set; }

        [ForeignKey(nameof(ProvinceID))]
        public Province? Province { get; set; }


        // =====================================================
        // COMPANY STATUS
        // =====================================================

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;


        // =====================================================
        // RELATIONSHIPS
        // =====================================================

        // One company can have many recruiters.
        public ICollection<User> Users { get; set; }
            = new List<User>();


        // One company can have many jobs.
        public ICollection<Job> Jobs { get; set; }
            = new List<Job>();
    }
}