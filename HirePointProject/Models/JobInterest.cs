namespace HirePoint.Models
{
    public class JobInterest
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public ICollection<JobseekerInterest> JobseekerInterests { get; set; }
            = new List<JobseekerInterest>();
    }
}