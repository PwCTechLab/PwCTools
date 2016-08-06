using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PwCTools.Models
{
    public class BoardTask
    {
        public int Id { get; set; }
        public int? ColumnId { get; set; }
        public int? SprintId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        //public virtual Column Column { get; set; }
        //public virtual Sprint Sprint { get; set; }
    }
}