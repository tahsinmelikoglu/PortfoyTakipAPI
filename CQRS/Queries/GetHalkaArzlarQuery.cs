using MediatR;
using Microsoft.EntityFrameworkCore;
using PortfoyTakipAPI.Models;
using PortfoyTakipAPI.DTOs; // DTO'larımızı içeri alıyoruz
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PortfoyTakipAPI.CQRS.Queries
{
    // 1. SORGU PAKETİ: Artık veritabanı Entity'si (HalkaArz) yerine doğrudan DTO listesi dönüyoruz
    public class GetHalkaArzlarQuery : IRequest<List<HalkaArzListesiDTO>>
    {
        public string? StatuFiltresi { get; set; }
    }

    // 2. İŞLEYİCİ: Veritabanına gidip listeyi çeken ve ortalamaları hesaplayan asıl uzman
    public class GetHalkaArzlarQueryHandler : IRequestHandler<GetHalkaArzlarQuery, List<HalkaArzListesiDTO>>
    {
        private readonly AppDbContext _context;

        public GetHalkaArzlarQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<HalkaArzListesiDTO>> Handle(GetHalkaArzlarQuery request, CancellationToken cancellationToken)
        {
            // Ortalamaları doğru hesaplayabilmek için tüm arzları bir kez hafızaya alıyoruz
            var tumArzlar = await _context.HalkaArzlar.AsNoTracking().ToListAsync(cancellationToken);

            // 1. GENEL PİYASA ORTALAMASI: Sektör fark etmeksizin son 10 gerçekleşmiş arz
            var genelPiyasaOrtalamasi = tumArzlar
                .Where(x => x.GerceklesenKatilimciSayisi != null && x.GerceklesenKatilimciSayisi > 0)
                .OrderByDescending(x => x.TalepToplamaBitis)
                .Take(10)
                .Average(x => (double?)x.GerceklesenKatilimciSayisi) ?? 0;

            // 2. FİLTRELEME: Eğer parametre geldiyse (Örn: "Yaklaşan") sadece onları alıyoruz
            var filtrelenmisArzlar = tumArzlar.AsEnumerable();
            if (!string.IsNullOrEmpty(request.StatuFiltresi))
            {
                filtrelenmisArzlar = filtrelenmisArzlar.Where(h => h.Statu == request.StatuFiltresi);
            }

            // 3. DTO DÖNÜŞÜMÜ VE SEKTÖR ORTALAMASI HESABI
            var sonuc = filtrelenmisArzlar
                .OrderByDescending(h => h.TalepToplamaBaslangic)
                .Select(arz => new HalkaArzListesiDTO
                {
                    Id = arz.Id,
                    SirketAdi = arz.SirketAdi,
                    BorsaKodu = arz.BorsaKodu,
                    LotFiyati = arz.LotFiyati,
                    Statu = arz.Statu,
                    TalepToplamaBaslangic = arz.TalepToplamaBaslangic.ToString("dd.MM.yyyy"),
                    TalepToplamaBitis = arz.TalepToplamaBitis.ToString("dd.MM.yyyy"),
                    Sektor = arz.Sektor,
                    KonsorsiyumLideri = arz.KonsorsiyumLideri,
                    KatilimEndeksineUygunMu = arz.KatilimEndeksineUygunMu,
                    GerceklesenKatilimciSayisi = arz.GerceklesenKatilimciSayisi,

                    // Sadece bu şirketin sektöründeki son 5 gerçekleşmiş arzı hesapla
                    SektorOrtalamasi = tumArzlar
                        .Where(x => x.Sektor == arz.Sektor && x.GerceklesenKatilimciSayisi != null && x.GerceklesenKatilimciSayisi > 0)
                        .OrderByDescending(x => x.TalepToplamaBitis)
                        .Take(5)
                        .Average(x => (double?)x.GerceklesenKatilimciSayisi) ?? 0,

                    GenelPiyasaOrtalamasi = genelPiyasaOrtalamasi
                }).ToList();

            return sonuc;
        }
    }
}