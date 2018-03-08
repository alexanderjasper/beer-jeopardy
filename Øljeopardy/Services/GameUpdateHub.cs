using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Oljeopardy.Services
{
    public class GameUpdateHub : Hub
    {
        public Task JoinGroup(string groupName)
        {
            return Groups.AddAsync(Context.ConnectionId, groupName);
        }

        public Task LeaveGroup(string groupName)
        {
            return Groups.RemoveAsync(Context.ConnectionId, groupName);
        }
    }
}