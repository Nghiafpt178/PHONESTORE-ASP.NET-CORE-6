using Microsoft.AspNetCore.SignalR;

namespace MyRazorPage.HubServer
{
    public class SignalRServer: Hub
    {

        public void HasNewData()
        {
            Clients.All.SendAsync("ReloadProduct");
        }
    }
}
