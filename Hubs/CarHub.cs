using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AutoShop.Hubs
{
    public class CarHub : Hub
    {
        // Метод, който извикваме от бекенда при добавяне на нова кола
        public async Task NotifyNewCar(string carName)
        {
            await Clients.All.SendAsync("ReceiveCarNotification", carName);
        }
    }
}
