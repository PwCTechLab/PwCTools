﻿using PwCTools.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PwCTools.Models
{
    public class BoardRepository
    {
        public List<Column> GetColumns()
        {
            if (HttpContext.Current.Cache["columns"] == null)
            {
                //var columns = new List<Column>();
                //var tasks = new List<KanbanTask>();
                //for (int i = 1; i < 6; i++)
                //{
                //    tasks.Add(new KanbanTask { ColumnId = 1, Id = i, Name = "Task " + i, Description = "Task " + i + " Description" });
                //}
                //columns.Add(new Column { Description = "to do column", Id = 1, Name = "to do", Tasks = tasks });
                //columns.Add(new Column { Description = "in progress column", Id = 2, Name = "in progress", Tasks = new List<KanbanTask>() });
                //columns.Add(new Column { Description = "test column", Id = 3, Name = "test", Tasks = new List<KanbanTask>() });
                //columns.Add(new Column { Description = "done column", Id = 4, Name = "done", Tasks = new List<KanbanTask>() });

                BoardContext db = new BoardContext();
                var columns = db.Projects.Find(1).Columns.ToList();

                HttpContext.Current.Cache["columns"] = columns;
            }
            return (List<Column>)HttpContext.Current.Cache["columns"];
        }

        public Column GetColumn(int colId)
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