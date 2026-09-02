using MediatR;
using PortfoyTakipAPI.DTOs;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PortfoyTakipAPI.CQRS.Queries
{
    public class CalculateHalkaArzSimulasyonQuery : IRequest<SimulasyonResponseDTO>
    {
        public SimulasyonRequestDTO Parameters { get; set; }
    }

    public class CalculateHalkaArzSimulasyonQueryHandler : IRequestHandler<CalculateHalkaArzSimulasyonQuery, SimulasyonResponseDTO>
    {
        public Task<SimulasyonResponseDTO> Handle(CalculateHalkaArzSimulasyonQuery request, CancellationToken cancellationToken)
        {
            var p = request.Parameters;

            // 1. Ana para maliyeti hesaplanıyor
            decimal anaParaMaliyeti = p.AlinanLotAdedi * p.HalkaArzFiyati;

            // 2. Hedef kâr marjına göre ulaşılacak fiyat (Örn: %100 artış)
            decimal artisCarpani = 1 + (p.HedefKarMarjiYuzdesi / 100m);
            decimal hedefFiyat = p.HalkaArzFiyati * artisCarpani;

            // 3. Ana parayı çıkarmak için satılması gereken lot sayısı
            int satilacakLot = (int)Math.Ceiling(anaParaMaliyeti / hedefFiyat);

            // 4. Kâr olarak içeride kalan lot sayısı 
            int kalanLot = p.AlinanLotAdedi - satilacakLot;

            var response = new SimulasyonResponseDTO
            {
                ToplamYatirilanTutar = anaParaMaliyeti,
                HedefFiyat = hedefFiyat,
                SatilacakLotAdedi = satilacakLot,
                IcerideKalanLotAdedi = kalanLot,
                IcerideKalanLotlarinDegeri = kalanLot * hedefFiyat,
                StratejiAciklamasi = $"Fiyat {hedefFiyat:F2} TL'ye ulaştığında {satilacakLot} lot satarak ana paranızı kurtarabilir, geriye kalan net {kalanLot} lotu maliyetsiz bir şekilde tutmaya devam edebilirsiniz."
            };

            return Task.FromResult(response);
        }
    }
}
