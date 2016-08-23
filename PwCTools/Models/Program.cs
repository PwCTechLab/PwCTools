using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PwCTools.Models
{
    public partial class Program
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Program")]
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public virtual List<Project> Projects { get; set; }
    }
}