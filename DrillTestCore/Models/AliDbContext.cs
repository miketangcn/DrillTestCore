using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DrillTestCore.Models
{
    public class AliDbContext:DbContext

    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(@"Server=rm-bp15xf4wi0dk028js2o.mysql.rds.aliyuncs.com;Database=Drill;
            user id=miketangcn;password=txg3115gwx", x => x.ServerVersion("8.0.20-mysql"));
        }
        public DbSet<Holerec> Holerecs { get; set; }
        public DbSet<Workrec> Workrecs { get; set; }

    }
}
