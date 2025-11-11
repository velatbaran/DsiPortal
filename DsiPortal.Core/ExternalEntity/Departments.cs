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
    [Table("Departments")]
    public class Departments
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Created { get; set; }
        public DateTime CreatedDate { get; set; }

        public IList<MyGuide> MyGuide { get; set; }

        public Departments()
        {
            MyGuide = new List<MyGuide>();
        }
    }
}
