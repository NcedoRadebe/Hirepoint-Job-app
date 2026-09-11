using Microsoft.AspNetCore.Mvc.Rendering;

namespace HirePoint.ViewModels
{
    public class JobInterestViewModel
    {
        public List<int> SelectedInterestIds { get; set; } = new();

        public List<SelectListItem> Interests { get; set; } = new();
    }
}