using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PwCTools.Models
{
    public class BoardTask
    {
        public int Id { get; set; }
        public int ColumnId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        
    }
}