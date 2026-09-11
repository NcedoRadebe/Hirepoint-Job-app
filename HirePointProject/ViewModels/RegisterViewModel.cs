using HirePoint.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using static Org.BouncyCastle.Math.EC.ECCurve;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HirePoint.ViewModels
{
    public class RegisterViewModel
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
        [StringLength(254, ErrorMessage = "Email address is too long.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;


        // =========================================================
        // PASSWORD
        // =========================================================

        [Required(ErrorMessage = "Password is required.")]
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
        public string Password { get; set; } = string.Empty;


        // =========================================================
        // CONFIRM PASSWORD
        // =========================================================

        [Required(ErrorMessage = "Please confirm your password.")]
        [DataType(DataType.Password)]
        [Compare(
            "Password",
            ErrorMessage = "Passwords do not match."
        )]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;


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
        // OPTIONAL
        // =========================================================

        [StringLength(
            150,
            MinimumLength = 2,
            ErrorMessage = "Qualification must be between 2 and 150 characters."
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

        [Required(ErrorMessage = "Please select an account type.")]
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
        // OPTIONAL
        // =========================================================

        [StringLength(
            150,
            ErrorMessage = "Company name cannot exceed 150 characters."
        )]
        [Display(Name = "Company Name")]
        public string? CompanyName { get; set; }


        [StringLength(
            2000,
            ErrorMessage = "Company description cannot exceed 2000 characters."
        )]
        [Display(Name = "Company Description")]
        public string? CompanyDescription { get; set; }


        [Url(ErrorMessage = "Please enter a valid website URL.")]
        [StringLength(
            300,
            ErrorMessage = "Website cannot exceed 300 characters."
        )]
        [Display(Name = "Company Website")]
        public string? Website { get; set; }
    }
}
