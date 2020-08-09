using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoSgin.DB.Models;
using Microsoft.EntityFrameworkCore;

namespace AutoSgin.DB
{
    public class SginDbContext : DbContext
    {
        public DbSet<WebSite> WebSite { get; set; }
        public DbSet<UserInfo> UserInfo { get; set; }

        public SginDbContext(DbContextOptions<SginDbContext> options)
            : base(options)
        {

        }
    }
}
