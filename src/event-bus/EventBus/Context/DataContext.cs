using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp2.Models;
using Microsoft.EntityFrameworkCore;


namespace ConsoleApp2
{
    class DataContext: DbContext
    {
        public DbSet<FailedRequest> failedRequests { get; set; }
        public DbSet<SuccessfulRequests> successfulRequests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=host.docker.internal,1433;Database=TransferData;User ID=SA;Password=1Secure*Password1;");
        }
    }
}
