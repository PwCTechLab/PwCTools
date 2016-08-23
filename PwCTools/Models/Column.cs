using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PwCTools.Models
{
    public partial class Column
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Column")]
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual Project Project { get; set; }

        public virtual List<BoardTask> Tasks { get; set; }
    }
}