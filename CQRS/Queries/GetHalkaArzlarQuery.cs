using MediatR;
using Microsoft.EntityFrameworkCore;
using PortfoyTakipAPI.Models;
using PortfoyTakipAPI.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json; // JsonSerializer için eklendi
using System.Threading;
using System.Threading.Tasks;

namespace PortfoyTakipAPI.CQRS.Queries
{
    public class GetHalkaArzlarQuery : IRequest<List<HalkaArzListesiDTO>>
    {
        public string? StatuFiltresi { get; set; }
    }

    public class GetHalkaArzlarQueryHandler : IRequestHandler<GetHalkaArzlarQuery, List<HalkaArzListesiDTO>>
    {
        private readonly AppDbContext _context;

        public GetHalkaArzlarQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<HalkaArzListesiDTO>> Handle(GetHalkaArzlarQuery request, CancellationToken cancellationToken)
        {
            var tumArzlar = await _context.HalkaArzlar.AsNoTracking().ToListAsync(cancellationToken);

            var genelPiyasaOrtalamasi = tumArzlar
                .Where(x => x.GerceklesenKatilimciSayisi != null && x.GerceklesenKatilimciSayisi > 0)
                .OrderByDescending(x => x.TalepToplamaBitis)
                .Take(10)
                .Average(x => (double?)x.GerceklesenKatilimciSayisi) ?? 0;

            var filtrelenmisArzlar = tumArzlar.AsEnumerable();
            if (!string.IsNullOrEmpty(request.StatuFiltresi))
            {
                filtrelenmisArzlar = filtrelenmisArzlar.Where(h => h.Statu == request.StatuFiltresi);
            }

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
                    ToplamDagilacakLot = arz.ToplamDagilacakLot, // BU SATIRI EKLE
                    Sektor = arz.Sektor,
                    KonsorsiyumLideri = arz.KonsorsiyumLideri,
                    KatilimEndeksineUygunMu = arz.KatilimEndeksineUygunMu,
                    GerceklesenKatilimciSayisi = arz.GerceklesenKatilimciSayisi,

                    SektorOrtalamasi = tumArzlar
                        .Where(x => x.Sektor == arz.Sektor && x.GerceklesenKatilimciSayisi != null && x.GerceklesenKatilimciSayisi > 0)
                        .OrderByDescending(x => x.TalepToplamaBitis)
                        .Take(5)
                        .Average(x => (double?)x.GerceklesenKatilimciSayisi) ?? 0,

                    GenelPiyasaOrtalamasi = genelPiyasaOrtalamasi,

                    // --- YENİ EKLENEN VİZYONER ALANLARIN DÖNÜŞÜMÜ ---
                    SirketOzeti = arz.SirketOzeti,
                    DagitimYontemi = arz.DagitimYontemi,
                    ArzBuyukluguTL = arz.ArzBuyukluguTL,
                    HalkaAciklikOrani = arz.HalkaAciklikOrani,
                    IskontoOrani = arz.IskontoOrani,
                    FinansalCiroArtisi = arz.FinansalCiroArtisi,
                    FinansalKarMarji = arz.FinansalKarMarji,
                    FinansalBorcluluk = arz.FinansalBorcluluk,

                    // JSON formatında tutulan verileri Arayüz için Nesneye çeviriyoruz
                    FonKullanimYerleri = !string.IsNullOrEmpty(arz.FonKullanimYerleriJson)
                        ? JsonSerializer.Deserialize<object>(arz.FonKullanimYerleriJson)
                        : null,

                    Taahhutler = !string.IsNullOrEmpty(arz.TaahhutlerJson)
                        ? JsonSerializer.Deserialize<List<string>>(arz.TaahhutlerJson)
                        : new List<string>()

                }).ToList();

            return sonuc;
        }
    }
}