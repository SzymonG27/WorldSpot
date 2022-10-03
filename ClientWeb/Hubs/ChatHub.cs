using Microsoft.AspNetCore.SignalR;

namespace ClientWeb.Hubs
{
    public class ChatHub : Hub
    {
        public string GetConnectionId() => Context.ConnectionId;
    }
}
