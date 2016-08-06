using PwCTools.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PwCTools.Models
{
    public class BoardRepository
    {

        private int _projectId = (int)HttpContext.Current.Cache["ActiveProject"]; //ToDo need code to select and change this

        private int GetActiveSprint(int projectId)
        {
            if (HttpContext.Current.Cache["ActiveSprint"] == null)
            {
                using (BoardContext db = new BoardContext())
                {
                    //Get ActiveSprint ID
                    var activeSprintId = db.Sprints
                        .Where(s => s.ProjectId == projectId && s.IsActive == true).FirstOrDefault().Id;

                    HttpContext.Current.Cache["ActiveSprint"] = activeSprintId; 
                }
            }
            return (int)HttpContext.Current.Cache["ActiveSprint"];
        }

        public List<Column> GetColumns()
        {
            if (HttpContext.Current.Cache["columns"] == null)
            {
                BoardContext db = new BoardContext();

                int activeSprintId = GetActiveSprint(_projectId);

                var columns = db.Columns
                    .Where(c => c.ProjectId == _projectId)
                    .Select(g => new
                    {
                        column = g,
                        task = g.Tasks.Where(t => t.SprintId == activeSprintId)
                    }).AsEnumerable() //Project to system object (.AsEnumerable) then can create entity object
                    .Select(c => new Column
                    {
                        Id = c.column.Id,
                        ProjectId = c.column.ProjectId,
                        Name = c.column.Name,
                        Description = c.column.Description,
                        Tasks = c.task.ToList()
                    })
                    .ToList();

                HttpContext.Current.Cache["columns"] = columns;
            }
            return (List<Column>)HttpContext.Current.Cache["columns"];
        }

        public Column GetColumn(int? colId)
        {
            return (from c in this.GetColumns()
                    where c.Id == colId
                    select c).FirstOrDefault();
        }

        public BoardTask GetTask(int taskId)
        {
            var columns = this.GetColumns();
            foreach (var c in columns)
            {
                foreach (var task in c.Tasks)
                {
                    if (task.Id == taskId)
                        return task;
                }
            }

            return null;
        }

        public void MoveTask(int taskId, int targetColId)
        {
            var columns = this.GetColumns();
            var targetColumn = this.GetColumn(targetColId);

            // Add task to the target column
            var task = this.GetTask(taskId);
            var sourceColId = task.ColumnId;
            task.ColumnId = targetColId;
            targetColumn.Tasks.Add(task);

            // Remove task from source column
            var sourceCol = this.GetColumn(sourceColId);
            sourceCol.Tasks.RemoveAll(t => t.Id == taskId);

            // Update column collection
            columns.RemoveAll(c => c.Id == sourceColId || c.Id == targetColId);
            columns.Add(targetColumn);
            columns.Add(sourceCol);

            this.UpdateColumns(columns.OrderBy(c => c.Id).ToList());

            //Update DB
            UpdateTask(task);
        }

        public void EditTask(int taskId, string taskName, string TaskDescription)
        {
            var task = this.GetTask(taskId);
            task.Name = taskName;
            task.Description = TaskDescription;

            UpdateTask(task);
        }

        public void AddTask(string taskName, string TaskDescription)
        {
            var task = new BoardTask();
            task.Name = taskName;
            task.Description = TaskDescription;
            task.SprintId = GetActiveSprint(_projectId);

            //Put in first column
            var columns = this.GetColumns();
            var column = columns.OrderBy(c => c.Id).First();
            task.ColumnId = column.Id;

            using (BoardContext db = new BoardContext())
            {
                db.BoardTasks.Add(task);
                db.SaveChanges();
            }

            column.Tasks.Add(task);
        }

        public void ArchiveTask(int taskId)
        {
            var task = this.GetTask(taskId);

            // Remove task from source column
            var sourceCol = this.GetColumn(task.ColumnId);
            sourceCol.Tasks.RemoveAll(t => t.Id == taskId);

            task.ColumnId = null;

            UpdateTask(task);
        }

        public void DeleteTask(int taskId)
        {
            var task = this.GetTask(taskId);

            // Remove task from source column
            var sourceCol = this.GetColumn(task.ColumnId);
            sourceCol.Tasks.RemoveAll(t => t.Id == taskId);

            //Delete from the db
            using (BoardContext db = new BoardContext())
            {
                db.Entry(task).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
        }

        private void UpdateTask(BoardTask task)
        {
            using (BoardContext db = new BoardContext())
            {
                db.Entry(task).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
        }

        private void UpdateColumns(List<Column> columns)
        {
            HttpContext.Current.Cache["columns"] = columns;
        }
    }
}