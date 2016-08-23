using PwCTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PwCTools.DAL.Repositories
{
    public interface IColumnRepository : IDisposable
    {
        IEnumerable<Column> GetColumns();
        Column GetColumnByID(int columnId);
        void InsertColumn(Column column);
        void DeleteColumn(int columnID);
        void UpdateColumn(Column column);
        void Save();
    }
}