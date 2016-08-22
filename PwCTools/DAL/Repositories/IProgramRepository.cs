using PwCTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PwCTools.DAL.Repositories
{
    public interface IProgramRepository : IDisposable
    {
        IEnumerable<Program> GetPrograms();
        Program GetProgramByID(int programId);
        void InsertProgram(Program program);
        void DeleteProgram(int programID);
        void UpdateProgram(Program program);
        void Save();
    }
}