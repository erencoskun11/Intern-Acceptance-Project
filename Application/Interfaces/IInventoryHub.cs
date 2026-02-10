using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IInventoryHub
    {

        Task ReceiveNotification(string message, string type = "success");

        Task ReceiveEntityUpdate(string entityName);

        Task ReceiveDashboardUpdate();
    }
}
