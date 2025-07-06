using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ContactAppNLayer.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactAppNLayer.DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
