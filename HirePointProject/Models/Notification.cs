using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HirePoint.Models
{
    public class Notification
    {
        // =========================
        // PRIMARY KEY
        // =========================

        [Key]
        public int NotificationID { get; set; }


        // =========================
        // USER
        // =========================

        [Required]
        public int UserID { get; set; }


        // =========================
        // NOTIFICATION CONTENT
        // =========================

        [Required(ErrorMessage = "Notification title is required.")]
        [StringLength(
            150,
            MinimumLength = 2,
            ErrorMessage = "Notification title must be between 2 and 150 characters."
        )]
        public string Title { get; set; } = string.Empty;


        [Required(ErrorMessage = "Notification message is required.")]
        [StringLength(
            2000,
            MinimumLength = 2,
            ErrorMessage = "Notification message must be between 2 and 2000 characters."
        )]
        public string Message { get; set; } = string.Empty;


        // =========================
        // STATUS
        // =========================

        [Required]
        public bool IsRead { get; set; } = false;


        // =========================
        // DATE
        // =========================

        [Required]
        public DateTime DateCreated { get; set; } = DateTime.Now;


        // =========================
        // RELATIONSHIP
        // =========================

        [ForeignKey(nameof(UserID))]
        public User User { get; set; } = null!;
    }
}