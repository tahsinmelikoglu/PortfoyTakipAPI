using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace PortfoyTakipAPI.Hubs
{
    public class PortfoyHub : Hub
    {
        // Gerekirse frontend (tarayıcı) doğrudan bu metodu tetikleyebilir.
        public async Task AnlikBildirimGonder(string baslik, string mesaj)
        {
           
            await Clients.All.SendAsync("BildirimAl", baslik, mesaj);
        }
    }
}