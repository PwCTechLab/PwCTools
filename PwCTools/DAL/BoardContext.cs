using PwCTools.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Web;

namespace PwCTools.DAL
{
    public class BoardContext : DbContext
    {
        private static BoardContext instance;

        private BoardContext() : base("DefaultConnection")
        {
        }

        public static BoardContext Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BoardContext();
                }
                return instance;
            }
        }

        public DbSet<Program> Programs { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Column> Columns { get; set; }
        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<BoardTask> BoardTasks { get; set; }
        public DbSet<BoardTaskComment> BoardTasksComments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}