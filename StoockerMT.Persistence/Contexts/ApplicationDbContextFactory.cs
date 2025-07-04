using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoockerMT.Persistence.Contexts
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<MasterDbContext>
    {
        public MasterDbContext CreateDbContext(string[] args)
        {
            
            var optionsBuilder = new DbContextOptionsBuilder<MasterDbContext>();
            optionsBuilder.UseSqlServer(Configuration.ConnectionStringMasterDb); 
            return new MasterDbContext(optionsBuilder.Options);
        }
    }
   

    public class TenantDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext> 
    {
        public TenantDbContext CreateDbContext(string[] args)
        {
           
            var optionsBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionsBuilder.UseSqlServer(Configuration.ConnectionStringTenantDb);

            return new TenantDbContext(optionsBuilder.Options);
        }

       
    }
}
