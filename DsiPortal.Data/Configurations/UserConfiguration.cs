using DsiPortal.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiPortal.Data.Configurations
{
    public class UserConfiguration:IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> builder)
        {
            builder.HasData(new Users
            {
                Id = 1,
                Name = "Velat",
                Surname = "BARAN",
                Username ="velatbaran",
                Password = "2121",
                IsAdmin = true,
                IsActive = true,
                UserGuid = Guid.NewGuid(),
                Created ="velatbaran",
                CreatedDate = DateTime.Now,
            });
        }
    }
}
