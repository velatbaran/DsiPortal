using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DsiPortal.WebUI.Models
{
    public class GuestFeeChartViewModel
    {
        [DisplayName("Adı"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Name { get; set; }

        [DisplayName("Dosya"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public IFormFile File { get; set; }
    }
}
