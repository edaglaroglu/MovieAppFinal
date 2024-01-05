using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Contexts
{
    public class DbFactory : IDesignTimeDbContextFactory<Db>
    {
        public Db CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<Db>();

            optionsBuilder.UseSqlServer("server=(localdb)\\mssqllocaldb;database=MovieDB;trusted_connection=true;");

            return new Db(optionsBuilder.Options);
        }
    }
}
