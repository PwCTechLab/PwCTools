﻿using PwCTools.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PwCTools.Models
{
    public class BoardRepository : DAL.Repositories.IBoardRepository, IDisposable
    {

        private int _projectId = (int)HttpContext.Current.Cache["ActiveProject"]; //ToDo need code to select and change this
        private BoardContext _db;

        public BoardRepository()
        {
            this._db = new BoardContext();
        }

        private int GetActiveSprint(int projectId)
        {
            if (HttpContext.Current.Cache["ActiveSprint"] == null)
            {
                //Get ActiveSprint ID
                var activeSprintId = _db.Sprints
                    .Where(s => s.ProjectId == projectId && s.IsActive == true).FirstOrDefault().Id;

                HttpContext.Current.Cache["ActiveSprint"] = activeSprintId; 
            }
            return (int)HttpContext.Current.Cache["ActiveSprint"];
        }

        public List<Column> GetColumns()
        {
            if (HttpContext.Current.Cache["columns"] == null)
            {
                int activeSprintId = GetActiveSprint(_projectId);

                var columns = _db.Columns
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

            _db.BoardTasks.Add(task);
            _db.SaveChanges();

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

            _db.Entry(task).State = System.Data.Entity.EntityState.Deleted;
            _db.SaveChanges();
        }

        private void UpdateTask(BoardTask task)
        {
            _db.Entry(task).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();
        }

        private void UpdateColumns(List<Column> columns)
        {
            HttpContext.Current.Cache["columns"] = columns;
        }

        public List<BoardTaskComment> GetComments(int taskId)
        {
            var task = this.GetTask(taskId);
            if (task != null)
            {
                return task.Comments;
            }

            return null;
        }

        public int AddComment(int taskId, string commentText, string createdBy, int? commentId)
        {
            if (commentId != null)
            {
                EditComment(taskId, commentText, createdBy, (int)commentId);
                return (int)commentId;
            }

            var comment = new BoardTaskComment();
            comment.BoardTaskId = taskId;
            comment.Comment = commentText;
            comment.CreatedBy = createdBy;
            comment.CreatedDateTime = DateTime.Now;

            //Save updated comment
            _db.BoardTasksComments.Add(comment);
            _db.SaveChanges();

            return comment.Id;
        }

        public void EditComment(int taskId, string commentText, string createdBy, int commentId)
        {
            var comment = (from c in GetTask(taskId).Comments
                           where c.Id == commentId
                          select c).FirstOrDefault();

            comment.Comment = commentText;
            comment.CreatedBy = createdBy;
            comment.CreatedDateTime = DateTime.Now;

            //Save updated comment
            _db.Entry(comment).State = System.Data.Entity.EntityState.Modified;
            _db.SaveChanges();

        }

        public int AddCommentAttachment(int taskId, string fileName, string path, string fileType, int? commentId)
        {
            //Create a comment for the attachment if one doesn't exist
            if (commentId == null)
                commentId = AddComment(taskId, null, "Chris Sallee", null); //ToDo add identity

            var attachment = new BoardCommentAttachment();
            attachment.BoardTaskCommentId = (int)commentId;
            attachment.FileName = fileName;
            attachment.FilePath = path;
            attachment.FileContentType = fileType;

            //Save attachment
            _db.BoardCommentAttachments.Add(attachment);
            _db.SaveChanges();

            return attachment.BoardTaskCommentId;
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}