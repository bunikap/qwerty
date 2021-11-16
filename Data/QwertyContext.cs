using Microsoft.EntityFrameworkCore;
using qwerty.Models;


namespace qwerty.Data
{
    public class QwertyContext : DbContext
    {
        public QwertyContext(DbContextOptions<QwertyContext> options)
            : base(options)
        {
        }

        public DbSet<Owner> Owner { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<UserPer> UserPer{get;set;}
        public DbSet<Permission> Permission{get;set;}
        public DbSet<Department> Department {get;set;}
    

        // public DbSet<HomeModel> HomeModel {get;set;}
    
    }
}
