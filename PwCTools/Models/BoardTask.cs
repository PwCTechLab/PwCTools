using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public string AssignedToID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public Nullable<System.DateTime> DueDate { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual List<BoardTaskComment> Comments { get; set; }
    }
}