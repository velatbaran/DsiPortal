using DsiPortal.Core.Entities;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DsiPortal.WebUI.Models
{
    public class FoodListViewModel
    {
        [DisplayName("Adı"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Name { get; set; }

        [DisplayName("Dosya"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public IFormFile File { get; set; }
    }
}
