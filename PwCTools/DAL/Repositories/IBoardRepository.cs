using PwCTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PwCTools.DAL.Repositories
{
    public interface IBoardRepository : IDisposable
    {
        List<Column> GetColumns();
        Column GetColumn(int? colId);
        BoardTask GetTask(int taskId);
        void MoveTask(int taskId, int targetColId);
        void EditTask(int taskId, string taskName, string TaskDescription);
        void AddTask(string taskName, string TaskDescription);
        void ArchiveTask(int taskId);
        void DeleteTask(int taskId);
        List<BoardTaskComment> GetComments(int taskId);
        int AddComment(int taskId, string commentText, string createdById, int? commentId);
        void EditComment(int taskId, string commentText, string createdById, int commentId);
        int AddCommentAttachment(int taskId, string fileName, string path, string fileType, int? commentId);
    }
}