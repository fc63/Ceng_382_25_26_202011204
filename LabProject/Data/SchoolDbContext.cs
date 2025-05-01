using Microsoft.EntityFrameworkCore;
using ClassManApp.Models;

namespace ClassManApp.Data
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
            : base(options)
        {
        }

        public DbSet<ClassInformationModel> Classes { get; set; }
    }
}