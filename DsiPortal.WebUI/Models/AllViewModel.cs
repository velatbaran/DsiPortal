using DsiPortal.Core.Entities;

namespace DsiPortal.WebUI.Models
{
    public class AllViewModel
    {
        public RegionalManagers? RegionalManager { get; set; }
        public GuestFeeChart? GuestFeeChart { get; set; }
        public ListMenuOfDayViewModel? ListMenuOfDayViewModel { get; set; }
        public FoodList? FoodList { get; set; }
        public FoodPriceList? FoodPriceList { get; set; }
        public List<WorksConducteds>? WorksConducteds { get; set; }
        public List<WorksConducteds>? OldWorksConducteds { get; set; }
        public List<Apps>? Apps { get; set; }
        public List<Cameras>? Cameras { get; set; }
        public List<BenefitLinks>? BenefitLinks { get; set; }
        public List<Announcements>? Announcements { get; set; }
        public List<FormDepartmentsViewModel>? FormDepartmentsViewModels { get; set; }
    }
}
