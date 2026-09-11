using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HirePoint.Models
{
    public class User
    {
        // =========================
        // PRIMARY KEY
        // =========================

        [Key]
        public int UserID { get; set; }


        // =========================
        // PERSONAL INFORMATION
        // =========================

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(
            100,
            MinimumLength = 2,
            ErrorMessage = "Full name must be between 2 and 100 characters."
        )]
        public string FullName { get; set; } = string.Empty;


        // =========================
        // EMAIL
        // =========================

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(254)]
        public string Email { get; set; } = string.Empty;


        // =========================
        // PASSWORD
        // =========================

        // Stores ONLY the password hash.
        // Never store the actual password here.

        [Required]
        public string PasswordHash { get; set; } = string.Empty;


        // =========================
        // PHONE
        // =========================

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;


        // =========================
        // PROVINCE
        // =========================

        public int? ProvinceID { get; set; }

        [ForeignKey(nameof(ProvinceID))]
        public Province? Province { get; set; }


        // =========================
        // COMPANY
        // =========================

        // Recruiters and Admins are associated
        // with their own company.
        //
        // Nullable for now so existing users can
        // continue to exist while company ownership
        // is assigned.

        public int? CompanyID { get; set; }

        [ForeignKey(nameof(CompanyID))]
        public Company? Company { get; set; }


        // =========================
        // USER TYPE
        // =========================

        [Required(ErrorMessage = "User type is required.")]
        [StringLength(30)]
        public string UserType { get; set; } = string.Empty;


        // =========================
        // ACCOUNT STATUS
        // =========================

        [Required]
        [StringLength(30)]
        public string Status { get; set; } = "Active";


        // =========================
        // APPROVAL
        // =========================

        public bool IsApproved { get; set; } = true;


        // =========================
        // EMAIL VERIFICATION
        // =========================

        public bool EmailConfirmed { get; set; } = false;


        // =========================
        // ACCOUNT CREATION
        // =========================

        [Required]
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;


        // =========================
        // GENDER
        // =========================

        [StringLength(20)]
        public string Gender { get; set; } = string.Empty;


        // =========================
        // REGISTRATION ONLY
        // =========================

        [NotMapped]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;


        [NotMapped]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;


        // =========================
        // OTHER RELATIONSHIPS
        // =========================

        public RecruiterProfile? RecruiterProfile { get; set; }

        public JobseekerProfile? JobseekerProfile { get; set; }

        public ICollection<Job> Jobs { get; set; }
            = new List<Job>();

        public ICollection<JobApplication> Applications { get; set; }
            = new List<JobApplication>();

        public ICollection<Notification> Notifications { get; set; }
            = new List<Notification>();
    }
}