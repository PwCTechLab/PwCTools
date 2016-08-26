using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
        public string CreatedById { get; set; }
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

        public string CreatedByName
        {
            get
            {
                using (var appContext = new ApplicationDbContext())
                {
                    var store = new UserStore<ApplicationUser>(appContext);
                    var userManager = new UserManager<ApplicationUser>(store);
                    return userManager.FindById(CreatedById).FullName; 
                }
            }
        }
    }
}