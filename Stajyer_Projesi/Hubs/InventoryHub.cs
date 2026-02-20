using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Stajyer_Projesi.Hubs
{
    [Authorize]
    public class InventoryHub : Hub<IInventoryHub>
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}
