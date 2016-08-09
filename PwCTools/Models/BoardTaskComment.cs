using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PwCTools.Models
{
    public class BoardTaskComment
    {
        public int Id { get; set; }
        public int BoardTaskId { get; set; }
        public string Comment { get; set; }
        public string CreatedBy { get; set; } //ToDo Change to int and link to user table
        public DateTime CreatedDateTime { get; set; }

        public string CreatedTimeSpan
        {
            get
            {
                if (CreatedDateTime.TimeOfDay.Days > 0)
                    return CreatedDateTime.TimeOfDay.Days.ToString() + " days ago";
                else
                    return CreatedDateTime.TimeOfDay.Hours.ToString() + " hours ago";
            }    
        }
    }
}