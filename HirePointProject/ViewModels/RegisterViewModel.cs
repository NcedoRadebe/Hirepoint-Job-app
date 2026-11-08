using HirePoint.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace HirePoint.ViewModels
{
    public class RegisterViewModel : IValidatableObject
    {
        // =========================================================
        // PERSONAL INFORMATION
        // =========================================================

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(
            100,
            MinimumLength = 2,
            ErrorMessage = "Full name must be between 2 and 100 characters."
        )]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;


        // =========================================================
        // EMAIL
        // =========================================================

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(
            254,
            ErrorMessage = "Email address is too long."
        )]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;


        // =========================================================
        // PASSWORD
        // JOBSEEKER ONLY
        // =========================================================

        [DataType(DataType.Password)]
        [StringLength(
            100,
            MinimumLength = 8,
            ErrorMessage = "Password must be at least 8 characters long."
        )]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$",
            ErrorMessage =
                "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character."
        )]
        [Display(Name = "Password")]
        public string? Password { get; set; }


        // =========================================================
        // CONFIRM PASSWORD
        // JOBSEEKER ONLY
        // =========================================================

        [DataType(DataType.Password)]
        [Compare(
            "Password",
            ErrorMessage = "Passwords do not match."
        )]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword { get; set; }


        // =========================================================
        // PHONE
        // OPTIONAL
        // =========================================================

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [StringLength(
            20,
            MinimumLength = 7,
            ErrorMessage = "Please enter a valid phone number."
        )]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }


        // =========================================================
        // PROVINCE
        // OPTIONAL
        // =========================================================

        [Display(Name = "Province")]
        public int? ProvinceID { get; set; }

        public List<SelectListItem> Provinces { get; set; } = new();


        // =========================================================
        // QUALIFICATION
        // JOBSEEKER ONLY
        // =========================================================

        [StringLength(
            150,
            MinimumLength = 2,
            ErrorMessage =
                "Qualification must be between 2 and 150 characters."
        )]
        [Display(Name = "Qualification")]
        public string? Qualification { get; set; }


        // =========================================================
        // QUALIFICATION ID
        // OPTIONAL
        // =========================================================

        public int? QualificationID { get; set; }


        // =========================================================
        // ACCOUNT TYPE
        // =========================================================

        [Required(
            ErrorMessage = "Please select an account type."
        )]
        [RegularExpression(
            "^(Jobseeker|Recruiter)$",
            ErrorMessage = "Please select a valid account type."
        )]
        [Display(Name = "Account Type")]
        public string UserType { get; set; } = string.Empty;


        // =========================================================
        // GENDER
        // OPTIONAL
        // =========================================================

        [RegularExpression(
            "^(Male|Female)$",
            ErrorMessage = "Please select a valid gender."
        )]
        [Display(Name = "Gender")]
        public string? Gender { get; set; }


        // =========================================================
        // RECRUITER INFORMATION
        // =========================================================

        [StringLength(
            150,
            ErrorMessage = "Company name cannot exceed 150 characters."
        )]
        [Display(Name = "Company Name")]
        public string? CompanyName { get; set; }


        [StringLength(
            2000,
            ErrorMessage =
                "Company description cannot exceed 2000 characters."
        )]
        [Display(Name = "Company Description")]
        public string? CompanyDescription { get; set; }


        [Url(
            ErrorMessage = "Please enter a valid website URL."
        )]
        [StringLength(
            300,
            ErrorMessage = "Website cannot exceed 300 characters."
        )]
        [Display(Name = "Company Website")]
        public string? Website { get; set; }


        // =========================================================
        // REPRESENTATIVE INFORMATION
        // RECRUITER ONLY
        // =========================================================

        [StringLength(
            150,
            ErrorMessage =
                "Representative name cannot exceed 150 characters."
        )]
        [Display(Name = "Representative Name")]
        public string? RepresentativeName { get; set; }


        [EmailAddress(
            ErrorMessage =
                "Please enter a valid representative email address."
        )]
        [StringLength(
            254,
            ErrorMessage =
                "Representative email address is too long."
        )]
        [Display(Name = "Representative Email")]
        public string? RepresentativeEmail { get; set; }


        // =========================================================
        // CONDITIONAL VALIDATION
        // =========================================================

        public IEnumerable<ValidationResult> Validate(
            ValidationContext validationContext)
        {
            // -----------------------------------------------------
            // RECRUITER
            // -----------------------------------------------------

            if (UserType.Equals(
                    "Recruiter",
                    StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(
                        RepresentativeName))
                {
                    yield return new ValidationResult(
                        "Representative name is required for recruiter accounts.",
                        new[]
                        {
                            nameof(RepresentativeName)
                        });
                }

                if (string.IsNullOrWhiteSpace(
                        RepresentativeEmail))
                {
                    yield return new ValidationResult(
                        "Representative email is required for recruiter accounts.",
                        new[]
                        {
                            nameof(RepresentativeEmail)
                        });
                }
            }


            // -----------------------------------------------------
            // JOBSEEKER
            // -----------------------------------------------------

            if (UserType.Equals(
                    "Jobseeker",
                    StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(
                        Password))
                {
                    yield return new ValidationResult(
                        "Password is required for jobseeker accounts.",
                        new[]
                        {
                            nameof(Password)
                        });
                }

                if (string.IsNullOrWhiteSpace(
                        ConfirmPassword))
                {
                    yield return new ValidationResult(
                        "Please confirm your password.",
                        new[]
                        {
                            nameof(ConfirmPassword)
                        });
                }
            }
        }
    }
}