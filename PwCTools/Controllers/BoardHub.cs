using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;

namespace PwCTools.Controllers
{
    [HubName("KanbanBoard")]
    public class BoardHub : Hub
    {
        public void NotifyBoardUpdated(string projectId)
        {
            //Clients.All.BoardUpdated();
            Clients.OthersInGroup(projectId).BoardUpdated();
        }

        public Task JoinGroup(string projectId)
        {
            return Groups.Add(Context.ConnectionId, projectId);
        }

        public Task LeaveGroup(string projectId)
        {
            return Groups.Remove(Context.ConnectionId, projectId);
        }

        public override Task OnConnected()
        {
            string projectId = Context.QueryString["projectId"];
            JoinGroup(projectId);
            return base.OnConnected();
        }

        //rejoin groups if client disconnects and then reconnects
        public override Task OnReconnected()
        {
            string projectId = Context.QueryString["projectId"];
            JoinGroup(projectId);
            return base.OnReconnected();
        }
    }
}