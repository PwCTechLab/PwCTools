using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PwCTools.Models;
using System.Data.Entity;

namespace PwCTools.DAL.Repositories
{
    public class ProjectRepository : IProjectRepository, IDisposable
    {
        private ApplicationDbContext context;

        public ProjectRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Project> GetProjects()
        {
            return context.Projects.ToList();
        }

        public Project GetProjectByID(int id)
        {
            return context.Projects.Find(id);
        }

        public void InsertProject(Project project)
        {
            context.Projects.Add(project);
        }

        public void DeleteProject(int projectID)
        {
            Project project = context.Projects.Find(projectID);
            context.Projects.Remove(project);
        }

        public void UpdateProject(Project project)
        {
            context.Entry(project).State = EntityState.Modified;
        }

        public List<Project> GetUserProjects(string id)
        {
            var projects = (from p in context.Projects
                            from pu in p.ProjectUsers
                            where pu.UserId == id
                            select p).ToList();

            return projects;

        }

        public int? GetDefaultProject(string id)
        {
            if (HttpContext.Current.Cache["ActiveProject"] == null)
            {
                var projects = context.ProjectUsers
                        .Where(p => p.UserId == id)
                        .Where(d => d.Default == true).FirstOrDefault();

                if (projects != null)
                {
                    HttpContext.Current.Cache["ActiveProject"] = projects.Id;
                    return projects.Id;
                }
            }

            return (int?)HttpContext.Current.Cache["ActiveProject"];
        }

        public void Save()
        {
            context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
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