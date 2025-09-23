using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DsiPortal.WebUI.Models
{
    public class RegionalManagerViewModel
    {
        [DisplayName("Adı"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Name { get; set; }

        [DisplayName("Unvanı"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Title { get; set; }

        [DisplayName("Dosya"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public IFormFile Image { get; set; }
    }
}
