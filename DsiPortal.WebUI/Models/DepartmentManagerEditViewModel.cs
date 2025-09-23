using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DsiPortal.WebUI.Models
{
    public class DepartmentManagerEditViewModel
    {
        public int Id { get; set; }
        [DisplayName("Adı"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Name { get; set; }

        [DisplayName("Unvan"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Title { get; set; }

        [DisplayName("Eposta"), StringLength(50), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Eposta { get; set; }

        [DisplayName("Telefon"), StringLength(20), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Phone { get; set; }

        [DisplayName("Dosya")]
        public IFormFile? Image { get; set; }
    }
}
