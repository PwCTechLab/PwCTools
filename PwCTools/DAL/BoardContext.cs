using PwCTools.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

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
        public DbSet<BoardTaskComment> BoardTasksComments { get; set; }
        public DbSet<BoardCommentAttachment> BoardCommentAttachments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}