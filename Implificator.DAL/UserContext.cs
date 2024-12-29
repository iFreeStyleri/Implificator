using Implificator.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Implificator.DAL
{
    public class UserContext : DbContext
    {
        private readonly IConfiguration _config;
        public DbSet<User> Users => Set<User>();
        public DbSet<VIP> VIPs => Set<VIP>();
        public DbSet<QRMessage> QRMessages => Set<QRMessage>();
        public UserContext(IConfiguration config)
        {
            _config = config;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var str = _config.GetConnectionString("PsqlConnection");
            optionsBuilder.UseNpgsql(str);

        }
    }
}
