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

        public virtual List<BoardCommentAttachment> Attachments { get; set; }

        public string CreatedTimeSpan
        {
            get
            {
                TimeSpan dateDiff = (DateTime.Now - CreatedDateTime);
                if (dateDiff.Days > 0)
                        return dateDiff.Days.ToString() + " days ago";
                else if (dateDiff.Hours > 0)
                    return dateDiff.Hours.ToString() + " hours ago";
                else
                    return dateDiff.Minutes.ToString() + " minutes ago";
            }    
        }
    }
}