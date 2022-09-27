using Microsoft.AspNetCore.SignalR;

namespace API.Hubs
{
    public class ChatHub : Hub
    {
        public void BroadcastMessage(string senderName, string receiverGroup, string msg)
        {
            Clients.Group(receiverGroup).SendAsync("ReceiveMessage", senderName, msg);
        }
    }
}
