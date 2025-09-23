using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiPortal.Core.Entities
{
    public class WorksConducteds : CommonEntity
    {
        [DisplayName("İşin Adı"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string WorkingName { get; set; }

        [DisplayName("Açıklama"), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string? Description { get; set; }

        [DisplayName("Resimler")]
        public byte[] Images { get; set; }

        [DisplayName("Dosya Tipi"), StringLength(10), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string? FileType { get; set; }

        [DisplayName("Dosya Adı"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string? FileName { get; set; }

        public bool IsMainImage { get; set; } // Ana resim mi belirteci

    }
}
