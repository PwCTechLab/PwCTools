using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PwCTools.Models;
using System.Data.Entity;

namespace PwCTools.DAL.Repositories
{
    public class ProgramRepository : IProgramRepository, IDisposable
    {
        private BoardContext context;

        public ProgramRepository(BoardContext context)
        {
            this.context = context;
        }

        public IEnumerable<Program> GetPrograms()
        {
            return context.Programs.ToList();
        }

        public Program GetProgramByID(int id)
        {
            return context.Programs.Find(id);
        }

        public void InsertProgram(Program program)
        {
            context.Programs.Add(program);
        }

        public void DeleteProgram(int programID)
        {
            Program program = context.Programs.Find(programID);
            context.Programs.Remove(program);
        }

        public void UpdateProgram(Program program)
        {
            context.Entry(program).State = EntityState.Modified;
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