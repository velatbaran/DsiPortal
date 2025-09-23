using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiPortal.Core.Entities
{
    public class Apps : CommonEntity
    {
        [DisplayName("Adı"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Name { get; set; }

        [DisplayName("Resim")]
        public byte[] Image { get; set; }

        [DisplayName("Dosya Tipi"), StringLength(10), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string? FileType { get; set; }

        [Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Link { get; set; }
    }
}
