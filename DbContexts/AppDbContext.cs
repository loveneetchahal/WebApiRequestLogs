using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using WebApiRequestLogs.Models;

namespace WebApiRequestLogs.DbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { 
        
        }
        public DbSet<RequestLogs> RequestLogs { get; set; }
    }
}
