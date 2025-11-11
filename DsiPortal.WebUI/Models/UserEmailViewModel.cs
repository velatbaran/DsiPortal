using System.ComponentModel.DataAnnotations;

namespace DsiPortal.WebUI.Models
{
    public class UserEmailViewModel
    {
        [StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Eposta { get; set; }
    }
}
