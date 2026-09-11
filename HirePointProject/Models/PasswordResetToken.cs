using System.ComponentModel.DataAnnotations;

namespace HirePoint.Models
{
    public class PasswordResetToken
    {
        [Key]
        public int PasswordResetTokenID { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        [StringLength(128)]
        public string TokenHash { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiresAt { get; set; }

        public DateTime? UsedAt { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public User User { get; set; } = null!;
    }
}