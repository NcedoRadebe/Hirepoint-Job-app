using System.ComponentModel.DataAnnotations;

namespace HirePoint.Models
{
    public class UserActivity
    {

        [Key]
        public int ActivityID { get; set; }


        public int RecruiterProfileID { get; set; }


        public string ActivityType { get; set; } = "";


        public DateTime ActivityDate { get; set; }
            = DateTime.Now;



        // Relationship

        public User? User { get; set; }

    }
}