using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiPortal.Core.Entities
{
    public class UserEmails : CommonEntity
    {
        [StringLength(100), Required(ErrorMessage = "{0} alanı boş geçilemez")]
        public string Eposta { get; set; }
    }
}
