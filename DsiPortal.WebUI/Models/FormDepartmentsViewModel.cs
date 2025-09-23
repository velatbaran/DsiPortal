using DsiPortal.Core.Entities;

namespace DsiPortal.WebUI.Models
{
    public class FormDepartmentsViewModel
    {
        public string DepartmentName { get; set; }
        public int Count { get; set; }
        public List<Forms>? Forms { get; set; }
    }
}
