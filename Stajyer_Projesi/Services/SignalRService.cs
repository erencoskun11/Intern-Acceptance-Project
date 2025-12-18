using Application.Interfaces;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Stajyer_Projesi.Hubs;

namespace Stajyer_Projesi.Services
{
    public class SignalRService : ISignalRService
    {
        private readonly IHubContext<InventoryHub, IInventoryHub> _hubContext;

        public SignalRService(IHubContext<InventoryHub,IInventoryHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task RefreshDashboardAsync()
        {
            await _hubContext.Clients.All.ReceiveDashboardUpdate();
        }

        public async Task RefreshEntityListAsync(string entityName)
        {
            await _hubContext.Clients.All.ReceiveEntityUpdate(entityName);
        }

        public async Task SendNotificationAsync(string message, string type = "success")
        {
            await _hubContext.Clients.All.ReceiveNotification(message,type);
        }
    }
}
