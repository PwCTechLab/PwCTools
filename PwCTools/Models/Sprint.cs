using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PwCTools.Models
{
    public class Sprint
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Project")]
        public int ProjectId { get; set; }
        [Display(Name = "Sprint")]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        public virtual Project Project { get; set; }

        public virtual List<BoardTask> Tasks { get; set; }
    }
}