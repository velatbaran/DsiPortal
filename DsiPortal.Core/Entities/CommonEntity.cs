using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace DsiPortal.Core.Entities
{
    public class CommonEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Kaydeden"), StringLength(50)]
        public string Created { get; set; }

        [DisplayName("Kayıt Tarihi")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
