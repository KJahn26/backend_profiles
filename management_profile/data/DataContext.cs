using Microsoft.EntityFrameworkCore;
            
     public class DataContext : DbContext
     {
         public DataContext(DbContextOptions<DataContext> options) : base(options) {}

         public DbSet<Profile> profiles { get; set; } // Tabla perfiles
     }

