using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PwCTools.Models
{
    public partial class Project
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Program")]
        public int ProgramId { get; set; }
        [Display(Name = "Project")]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public virtual Program Program { get; set; }

        public virtual List<Column> Columns { get; set; }
        public virtual List<Sprint> Sprints { get; set; }
        public virtual List<ProjectUser> ProjectUsers { get; set; }
    }
}