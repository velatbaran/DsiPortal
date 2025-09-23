using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiPortal.Service.IService
{
    public interface IMenuofDay
    {
        (string?,string?,string?,string?)   IListMenuofDay();
    }
}
