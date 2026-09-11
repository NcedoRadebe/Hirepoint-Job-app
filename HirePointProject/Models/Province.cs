using System.ComponentModel.DataAnnotations;

namespace HirePoint.Models
{
    public class Province
    {
        [Key]
        public int ProvinceID { get; set; }


        [Required(ErrorMessage = "Province name is required.")]

        public string ProvinceName { get; set; } = string.Empty;


        // Users belonging to this province
        public ICollection<User> Users { get; set; }
            = new List<User>();
    }
}