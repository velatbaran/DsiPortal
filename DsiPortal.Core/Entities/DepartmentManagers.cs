using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiPortal.Core.Entities
{
    public class DepartmentManagers :CommonEntity
    {
        [DisplayName("Adı"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Name { get; set; }

        [DisplayName("Unvan"), StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Title { get; set; }

        [DisplayName("Eposta"), StringLength(50), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Eposta { get; set; }

        [DisplayName("Telefon"),StringLength(20), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Phone { get; set; }

        [DisplayName("Resim")]
        public byte[]? Image { get; set; }

        [DisplayName("Dosya Tipi"), StringLength(10)]
        public string? FileType { get; set; }
    }
}
