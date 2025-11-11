using DsiPortal.Core.ExternalEntity;
using DsiPortal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiPortal.Service.Concrete
{
    public class ExternalMyGuideService
    {
        private readonly ExternalDbContext _externalDb;

        public ExternalMyGuideService(ExternalDbContext externalDb)
        {
            _externalDb = externalDb;
        }
        //public class MyGuideDto
        //{
        //    public int Id { get; set; }
        //    public string NameSurname { get; set; }
        //    public string Title { get; set; }
        //    public string DepartmentName { get; set; }
        //    public bool IsDeleted { get; set; }
        //    public string InternalNo { get; set; }
        //    public string CepNo { get; set; }
        //}

        public async Task<List<MyGuideDto>> GetAllDtoAsync()
        {
            var q = from g in _externalDb.MyGuide
                    join d in _externalDb.Department on g.DepartmentId equals d.Id into gd
                    where g.IsDeleted == false
                    from d in gd.DefaultIfEmpty()
                    select new MyGuideDto
                    {
                        Id = g.Id,
                        NameSurname = g.NameSurname,
                        Title = g.Title,
                        DepartmentName = d != null ? d.Name : null,
                        InternalNo = g.InternalNo,
                        CepNo = g.CepNo
                    };

            return await q.AsNoTracking().ToListAsync();
        }

    }
}
