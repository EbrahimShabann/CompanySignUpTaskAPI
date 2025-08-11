using CompanySignUpTask.Data_Layer.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanySignUpTask.Data_Layer
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
           
        }
        public DbSet<Company> Companies { get; set; }
    }
}
