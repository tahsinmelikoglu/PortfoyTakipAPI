using MediatR;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PortfoyTakipAPI.Models;
using System;

namespace PortfoyTakipAPI.CQRS.Commands
{
    // Swagger'da listeleri temiz girmek için alt DTO
    public class FonKullanimDto
    {
        public string Alan { get; set; }
        public int Oran { get; set; }
        public string Renk { get; set; }
    }

    // Command Nesnesi (Request)
    public class CreateHalkaArzCommand : IRequest<int>
    {
        public string SirketAdi { get; set; }
        public string BorsaKodu { get; set; }
        public decimal LotFiyati { get; set; }
        public string Statu { get; set; }
        public DateTime TalepToplamaBaslangic { get; set; }
        public DateTime TalepToplamaBitis { get; set; }
        public int ToplamDagilacakLot { get; set; }

        public string? Sektor { get; set; }

        // YENİ EKLENDİ: Banka/Aracı Kurum ID'lerini alacak liste
        public List<int>? KonsorsiyumIds { get; set; }

        public bool KatilimEndeksineUygunMu { get; set; }
        public int? GerceklesenKatilimciSayisi { get; set; }
        public string? SirketOzeti { get; set; }
        public string? DagitimYontemi { get; set; }
        public decimal? ArzBuyukluguTL { get; set; }
        public decimal? HalkaAciklikOrani { get; set; }
        public decimal? IskontoOrani { get; set; }

        public string? FinansalCiroArtisi { get; set; }
        public string? FinansalKarMarji { get; set; }
        public string? FinansalBorcluluk { get; set; }

        public List<FonKullanimDto>? FonKullanimYerleri { get; set; }
        public List<string>? Taahhutler { get; set; }
    }

    // Handler Nesnesi (Veritabanı İşlemi)
    public class CreateHalkaArzCommandHandler : IRequestHandler<CreateHalkaArzCommand, int>
    {
        private readonly AppDbContext _context;

        public CreateHalkaArzCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateHalkaArzCommand request, CancellationToken cancellationToken)
        {
            var halkaArz = new HalkaArz
            {
                SirketAdi = request.SirketAdi,
                BorsaKodu = request.BorsaKodu,
                LotFiyati = request.LotFiyati,
                Statu = request.Statu,
                TalepToplamaBaslangic = request.TalepToplamaBaslangic,
                TalepToplamaBitis = request.TalepToplamaBitis,
                ToplamDagilacakLot = request.ToplamDagilacakLot,
                Sektor = request.Sektor,
                KatilimEndeksineUygunMu = request.KatilimEndeksineUygunMu,
                GerceklesenKatilimciSayisi = request.GerceklesenKatilimciSayisi,
                SirketOzeti = request.SirketOzeti,
                DagitimYontemi = request.DagitimYontemi,
                ArzBuyukluguTL = request.ArzBuyukluguTL,
                HalkaAciklikOrani = request.HalkaAciklikOrani,
                IskontoOrani = request.IskontoOrani,
                FinansalCiroArtisi = request.FinansalCiroArtisi,
                FinansalKarMarji = request.FinansalKarMarji,
                FinansalBorcluluk = request.FinansalBorcluluk,

                FonKullanimYerleriJson = request.FonKullanimYerleri != null
                    ? JsonSerializer.Serialize(request.FonKullanimYerleri)
                    : null,

                TaahhutlerJson = request.Taahhutler != null
                    ? JsonSerializer.Serialize(request.Taahhutler)
                    : null,

                Konsorsiyumlar = new List<Konsorsiyum>()
            };

            // YENİ EKLENDİ: Gelen ID'lere göre Bankaları DB'den bul ve arz'a bağla
            if (request.KonsorsiyumIds != null && request.KonsorsiyumIds.Any())
            {
                var bankalar = await _context.Konsorsiyumlar
                    .Where(k => request.KonsorsiyumIds.Contains(k.Id))
                    .ToListAsync(cancellationToken);

                halkaArz.Konsorsiyumlar = bankalar;
            }

            _context.HalkaArzlar.Add(halkaArz);
            await _context.SaveChangesAsync(cancellationToken);

            return halkaArz.Id;
        }
    }
}