using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DsiPortal.WebUI.Models
{
    public class WorksConductedViewModel
    {
        [DisplayName("İşin Adı"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string WorkingName { get; set; }

        [DisplayName("Açıklama"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string? Description { get; set; }

        [DisplayName("Resimler"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public List<IFormFile> ImageFiles { get; set; } = new List<IFormFile>();
        //public IFormFile ImageFile { get; set; };
        public bool IsMainImage { get; set; }
    }
}
