using DsiPortal.Core.Entities;

namespace DsiPortal.WebUI.Models
{
    public class WorksConductedDetailViewModel
    {
        public string WorkingName { get; set; }
        public string Description { get; set; }
        public int Count { get; set; }
        public List<WorksConducteds>? WorksConducteds { get; set; }
    }
}
