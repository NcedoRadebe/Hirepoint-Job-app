using System.ComponentModel.DataAnnotations;

namespace HirePoint.ViewModels
{
    public class ResetPasswordViewModel
    {
        // Secure password-reset token from the email link
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [DataType(DataType.Password)]
        [StringLength(
            8,
            MinimumLength = 8,
            ErrorMessage = "Password must be exactly 8 characters."
        )]
        [RegularExpression(
            @"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d.*\d)(?=.*[^A-Za-z0-9]).{8}$",
            ErrorMessage = "Password must be exactly 8 characters and contain at least 1 uppercase letter, 1 lowercase letter, 2 numbers, and 1 special character."
        )]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm your new password.")]
        [DataType(DataType.Password)]
        [Compare(
            "NewPassword",
            ErrorMessage = "Passwords do not match."
        )]
        [Display(Name = "Confirm New Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
