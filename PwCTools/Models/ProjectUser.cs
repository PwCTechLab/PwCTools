using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PwCTools.Models
{
    public partial class ProjectUser
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        [ForeignKey("Project")]
        public int ProjectId { get; set; }

        
        public virtual ApplicationUser User { get; set; }
        public virtual Project Project { get; set; }
    }
}