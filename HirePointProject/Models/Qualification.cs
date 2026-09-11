using System.ComponentModel.DataAnnotations;

namespace HirePoint.Models
{
    public class Qualification
    {
        // =====================================================
        // PRIMARY KEY
        // =====================================================

        [Key]
        public int QualificationID { get; set; }


        // =====================================================
        // QUALIFICATION NAME
        // =====================================================

        [Required(ErrorMessage = "Qualification name is required.")]
        [StringLength(
            150,
            MinimumLength = 2,
            ErrorMessage = "Qualification must be between 2 and 150 characters."
        )]
        public string Name { get; set; } = string.Empty;


        // =====================================================
        // RELATIONSHIPS
        // =====================================================

        public ICollection<JobseekerProfile> JobseekerProfiles { get; set; }
            = new List<JobseekerProfile>();

        public ICollection<Job> Jobs { get; set; }
            = new List<Job>();
    }
}