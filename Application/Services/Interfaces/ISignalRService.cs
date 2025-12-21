using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Interfaces
{
    public interface ISignalRService
    {
        Task SendNotificationAsync(string message,string type = "success");
        Task RefreshEntityListAsync(string entityName); 
        Task RefreshDashboardAsync();
    }
}
