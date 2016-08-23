using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PwCTools.Models
{
    public class BoardTask
    {
        public int Id { get; set; }
        public int? ColumnId { get; set; }
        public int? SprintId { get; set; }
        [Display(Name = "Task")]
        public string Name { get; set; }
        public string Description { get; set; }

        //public virtual Column Column { get; set; }
        //public virtual Sprint Sprint { get; set; }

        public virtual List<BoardTaskComment> Comments { get; set; }
    }
}