using PwCTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PwCTools.DAL.Repositories
{
    public interface ISprintRepository : IDisposable
    {
        IEnumerable<Sprint> GetSprints();
        Sprint GetSprintByID(int sprintId);
        void InsertSprint(Sprint sprint);
        void DeleteSprint(int sprintID);
        void UpdateSprint(Sprint sprint);
        void Save();
    }
}