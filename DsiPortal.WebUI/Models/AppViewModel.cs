using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DsiPortal.WebUI.Models
{
    public class AppViewModel
    {
        [Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Link { get; set; }

        [DisplayName("Adı"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Name { get; set; }

        [DisplayName("Dosya"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public IFormFile Image { get; set; }
    }
}
