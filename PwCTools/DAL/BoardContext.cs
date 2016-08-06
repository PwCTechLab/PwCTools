using PwCTools.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace PwCTools.DAL
{
    public class BoardContext : DbContext
    {
        public BoardContext() : base("DefaultConnection")
        {
        }

        public DbSet<Program> Programs { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Column> Columns { get; set; }
        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<BoardTask> BoardTasks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}