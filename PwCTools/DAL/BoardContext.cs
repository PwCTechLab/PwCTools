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
        //read-only dictionary to track multitons
        private static IDictionary<int, BoardContext> _Tracker = new Dictionary<int, BoardContext> { };

        private BoardContext() : base("DefaultConnection")
        {
        }

        public static BoardContext GetInstance(int key)
        {
            //value to return
            BoardContext item = null;

            //lock collection to prevent changes during operation
            lock (_Tracker)
            {
                //if value not found, create and add
                if (!_Tracker.TryGetValue(key, out item))
                {
                    item = new BoardContext();

                    //add item
                    _Tracker.Add(key, item);
                }
            }
            return item;
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