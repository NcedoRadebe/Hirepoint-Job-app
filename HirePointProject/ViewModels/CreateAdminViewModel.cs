using System.ComponentModel.DataAnnotations;

namespace HirePoint.ViewModels
{
    public class CreateAdminViewModel
    {
        // =========================================================
        // ADMIN DETAILS
        // =========================================================

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(
            100,
            MinimumLength = 2,
            ErrorMessage = "Full name must be between 2 and 100 characters."
        )]
        public string FullName { get; set; } = string.Empty;


        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(
            ErrorMessage = "Please enter a valid email address."
        )]
        [StringLength(254)]
        public string Email { get; set; } = string.Empty;


        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(
            ErrorMessage = "Please enter a valid phone number."
        )]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;


        [Required(ErrorMessage = "Please select a gender.")]
        public string Gender { get; set; } = string.Empty;


        [Required(ErrorMessage = "Province is required.")]
        public int? ProvinceID { get; set; }


        // =========================================================
        // ACCOUNT TYPE
        // =========================================================

        // This form creates the company's Administrator.
        // The controller should force UserType = "Admin".

        [Display(Name = "Account Type")]
        public string UserType { get; set; } = "Admin";


        // =========================================================
        // PASSWORD
        // =========================================================

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [StringLength(
            100,
            MinimumLength = 8,
            ErrorMessage = "Password must be at least 8 characters."
        )]
        public string Password { get; set; } = string.Empty;


        [Required(ErrorMessage = "Please confirm the password.")]
        [DataType(DataType.Password)]
        [Compare(
            "Password",
            ErrorMessage = "Passwords do not match."
        )]
        public string ConfirmPassword { get; set; } = string.Empty;


        // =========================================================
        // COMPANY DETAILS
        // =========================================================

        [Required(ErrorMessage = "Company name is required.")]
        [StringLength(
            200,
            MinimumLength = 2,
            ErrorMessage = "Company name must be between 2 and 200 characters."
        )]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; } = string.Empty;


        [StringLength(
            5000,
            ErrorMessage = "Company description cannot exceed 5000 characters."
        )]
        [Display(Name = "Company Description")]
        public string? CompanyDescription { get; set; }


        [Url(ErrorMessage = "Please enter a valid website URL.")]
        [StringLength(500)]
        [Display(Name = "Website")]
        public string? Website { get; set; }
    }
}
