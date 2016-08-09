using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using PwCTools.Models;

namespace PwCTools.DAL
{
    public class BoardInitializer : System.Data.Entity.DropCreateDatabaseAlways<BoardContext>
    {
        protected override void Seed(BoardContext context)
        {
            var programs = new List<Program>
            {
                new Program { Name="FIMA", Description="Federal Insurance Mitigation Administration" }
            };

            programs.ForEach(s => context.Programs.Add(s));
            context.SaveChanges();

            var projects = new List<Project>
            {
                new Project { ProgramId=1, Name="UCORT", Description="UCORT Project" }
            };

            projects.ForEach(s => context.Projects.Add(s));
            context.SaveChanges();

            var columns = new List<Column>
            {
                new Column { ProjectId=1, Name="To Do", Description="To Do Column" },
                new Column { ProjectId=1, Name="In Progress", Description="In Progress Column" },
                new Column { ProjectId=1, Name="Test", Description="Test Column" },
                new Column { ProjectId=1, Name="Done", Description="Done Column" }
            };

            columns.ForEach(s => context.Columns.Add(s));
            context.SaveChanges();

            var sprints = new List<Sprint>
            {
                new Sprint { ProjectId=1, Name="Completed Sprint", Description="Completed UCORT Sprint", StartDate=DateTime.Now.AddDays(-14), EndDate=DateTime.Now, IsActive=false },
                new Sprint { ProjectId=1, Name="Active Sprint", Description="Active UCORT Sprint", StartDate=DateTime.Now, EndDate=DateTime.Now.AddDays(14), IsActive=true }
            };

            sprints.ForEach(s => context.Sprints.Add(s));
            context.SaveChanges();

            var tasks = new List<BoardTask>
            {
                new BoardTask { SprintId=1, ColumnId=1, Name="Task 1", Description="Task 1 Description" },
                new BoardTask { SprintId=2, ColumnId=1, Name="Task 2", Description="Task 2 Description" },
                new BoardTask { SprintId=2, ColumnId=1, Name="Task 3", Description="Task 3 Description" },
                new BoardTask { SprintId=2, ColumnId=1, Name="Task 4", Description="Task 4 Description" },
                new BoardTask { SprintId=2, ColumnId=1, Name="Task 5", Description="Task 5 Description" }
            };

            tasks.ForEach(s => context.BoardTasks.Add(s));
            context.SaveChanges();

            var comments = new List<BoardTaskComment>
            {
                new BoardTaskComment { BoardTaskId=2, Comment="Test Comment 1", CreatedBy="Chris Sallee", CreatedDateTime=DateTime.Now },
                new BoardTaskComment { BoardTaskId=2, Comment="Test Comment 2", CreatedBy="Michael Hall", CreatedDateTime=DateTime.Now },
                new BoardTaskComment { BoardTaskId=2, Comment="Test Comment 3", CreatedBy="Chris Sallee", CreatedDateTime=DateTime.Now }
            };

            comments.ForEach(s => context.BoardTasksComments.Add(s));
            context.SaveChanges();
        }
    }
}