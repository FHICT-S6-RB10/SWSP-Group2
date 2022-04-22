using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using event_bus.Models;
using Microsoft.EntityFrameworkCore;


namespace event_bus.Context
{
    class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) :
            base(options)
        {
        }
        public DbSet<FailedRequest> failedRequests { get; set; }
        public DbSet<SuccessfulRequests> successfulRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }


    }
}
