using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PwCTools.Models
{
    public partial class Project
    {
        public int Id { get; set; }
        public int ProgramId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual List<Column> Columns { get; set; }
        public virtual List<Sprint> Sprints { get; set; }
    }
}