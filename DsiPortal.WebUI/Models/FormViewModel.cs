using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DsiPortal.WebUI.Models
{
    public class FormViewModel
    {
        [DisplayName("Birim"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Department { get; set; }

        [DisplayName("Adı"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Name { get; set; }

        [DisplayName("Dosya"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public IFormFile File { get; set; }
    }
}
