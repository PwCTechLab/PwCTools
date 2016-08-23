using PwCTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PwCTools.DAL.Repositories
{
    public interface IProjectRepository : IDisposable
    {
        IEnumerable<Project> GetProjects();
        Project GetProjectByID(int projectId);
        void InsertProject(Project project);
        void DeleteProject(int projectID);
        void UpdateProject(Project project);
        void Save();
    }
}