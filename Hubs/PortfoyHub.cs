using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace PortfoyTakipAPI.Hubs
{
    // Hub sınıfından miras alıyoruz. Bu sınıf artık bizim yayın kulemiz.
    public class PortfoyHub : Hub
    {
        // Gerekirse frontend (tarayıcı) doğrudan bu metodu tetikleyebilir.
        // Şimdilik sadece kuleyi diktik, asıl yayını birazdan CQRS'in içinden yapacağız!
        public async Task AnlikBildirimGonder(string baslik, string mesaj)
        {
            // "BildirimAl" isimli kanalı dinleyen tüm kullanıcılara bu veriyi canlı fırlatır.
            await Clients.All.SendAsync("BildirimAl", baslik, mesaj);
        }
    }
}