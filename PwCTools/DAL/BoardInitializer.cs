using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using PwCTools.Models;

namespace PwCTools.DAL
{
    public class BoardInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<BoardContext>
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

            var tasks = new List<BoardTask>
            {
                new BoardTask { ColumnId=1, Name="Task 1", Description="Task 1 Description" },
                new BoardTask { ColumnId=1, Name="Task 2", Description="Task 2 Description" },
                new BoardTask { ColumnId=1, Name="Task 3", Description="Task 3 Description" },
                new BoardTask { ColumnId=1, Name="Task 4", Description="Task 4 Description" },
                new BoardTask { ColumnId=1, Name="Task 5", Description="Task 5 Description" }
            };

            tasks.ForEach(s => context.BoardTasks.Add(s));
            context.SaveChanges();
        }
    }
}