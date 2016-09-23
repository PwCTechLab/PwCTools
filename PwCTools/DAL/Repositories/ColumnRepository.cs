using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PwCTools.Models;
using System.Data.Entity;

namespace PwCTools.DAL.Repositories
{
    public class ColumnRepository : IColumnRepository, IDisposable
    {
        private ApplicationDbContext context;

        public ColumnRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Column> GetColumns()
        {
            return context.Columns.ToList();
        }

        public Column GetColumnByID(int id)
        {
            return context.Columns.Find(id);
        }

        public void InsertColumn(Column column)
        {
            context.Columns.Add(column);
        }

        public void DeleteColumn(int columnID)
        {
            Column column = context.Columns.Find(columnID);
            context.Columns.Remove(column);
        }

        public void UpdateColumn(Column column)
        {
            context.Entry(column).State = EntityState.Modified;
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