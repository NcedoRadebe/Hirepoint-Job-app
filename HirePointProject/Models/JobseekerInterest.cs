namespace HirePoint.Models
{
    public class JobseekerInterest
    {
        public int Id { get; set; }

        public int JobseekerId { get; set; }

        public int JobInterestId { get; set; }

        public User Jobseeker { get; set; } = null!;

        public JobInterest JobInterest { get; set; } = null!;
    }
}
