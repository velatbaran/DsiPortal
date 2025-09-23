using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DsiPortal.WebUI.Models
{
    public class CamerasViewModel
    {
        [DisplayName("Adı"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Link { get; set; }

        [Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Password { get; set; }
    }
}
