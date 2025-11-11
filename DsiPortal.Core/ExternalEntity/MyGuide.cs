using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace DsiPortal.Core.ExternalEntity
{
    [Table("MyGuide")] // tablo adı birebir veritabanındakiyle aynı
    public class MyGuide
    {
        public int Id { get; set; }
        public string NameSurname { get; set; }
        public string Title { get; set; }
        // 🔹 Bu sütun veritabanında gerçekten mevcut olmalı
        [Column("DepartmentId")]
        public int? DepartmentId { get; set; }

        // 🔹 Navigation property – EF’ye açıkça foreign key'i belirtelim
        [ForeignKey("DepartmentId")]
        public Departments? Department { get; set; }
        public string InternalNo { get; set; }
        public string CepNo { get; set; }
        public bool IsDeleted { get; set; }
        public string Created { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
