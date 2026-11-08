using System.ComponentModel.DataAnnotations;

namespace HirePoint.ViewModels
{
    public class ApplicationMessageViewModel
    {

            public int ApplicationID { get; set; }

            public string ApplicantName { get; set; } = string.Empty;

            public string ApplicantEmail { get; set; } = string.Empty;

            public string JobTitle { get; set; } = string.Empty;

            public string ApplicationStatus { get; set; } = "Pending";

            public string Qualification { get; set; } = string.Empty;

            public string Skills { get; set; } = string.Empty;

            public string Education { get; set; } = string.Empty;

            public string Experience { get; set; } = string.Empty;

            [Required(ErrorMessage = "Please provide a reason or additional details for this status.")]
            [StringLength(
                1000,
                ErrorMessage = "The reason cannot exceed 1000 characters.")]
            public string Reason { get; set; } = string.Empty;

            [StringLength(
                1000,
                ErrorMessage = "The message cannot exceed 1000 characters.")]
            public string? GeneratedMessage { get; set; }

            public bool MessageGenerated { get; set; }
        }
    }


