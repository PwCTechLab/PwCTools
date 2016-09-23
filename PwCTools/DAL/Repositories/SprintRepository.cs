using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PwCTools.Models;
using System.Data.Entity;

namespace PwCTools.DAL.Repositories
{
    public class SprintRepository : ISprintRepository, IDisposable
    {
        private ApplicationDbContext context;

        public SprintRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Sprint> GetSprints()
        {
            return context.Sprints.ToList();
        }

        public Sprint GetSprintByID(int id)
        {
            return context.Sprints.Find(id);
        }

        public void InsertSprint(Sprint sprint)
        {
            context.Sprints.Add(sprint);
        }

        public void DeleteSprint(int sprintID)
        {
            Sprint sprint = context.Sprints.Find(sprintID);
            context.Sprints.Remove(sprint);
        }

        public void UpdateSprint(Sprint sprint)
        {
            context.Entry(sprint).State = EntityState.Modified;
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