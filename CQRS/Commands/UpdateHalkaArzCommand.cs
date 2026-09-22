using MediatR;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PortfoyTakipAPI.Models;

namespace PortfoyTakipAPI.CQRS.Commands
{
    // Command Nesnesi: Mevcut alanlara ek olarak güncellenecek kaydın Id'sini içeriyor
    public class UpdateHalkaArzCommand : IRequest<bool>
    {
        public int Id { get; set; }
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

    // İşleyici Nesnesi: Veritabanında kaydı bulur ve yeni değerlerle ezer
    public class UpdateHalkaArzCommandHandler : IRequestHandler<UpdateHalkaArzCommand, bool>
    {
        private readonly AppDbContext _context;

        public UpdateHalkaArzCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateHalkaArzCommand request, CancellationToken cancellationToken)
        {
            // 1. Veritabanından güncellenecek kaydı bul (İlişkiler dahil!)
            var arz = await _context.HalkaArzlar
                .Include(h => h.Konsorsiyumlar) // Eski bankaları da getir ki üzerine yazabilelim
                .FirstOrDefaultAsync(h => h.Id == request.Id, cancellationToken);

            if (arz == null)
            {
                return false;
            }

            // 2. Yeni gelen verileri eski kaydın üzerine yaz
            arz.SirketAdi = request.SirketAdi;
            arz.BorsaKodu = request.BorsaKodu;
            arz.LotFiyati = request.LotFiyati;
            arz.Statu = request.Statu;
            arz.TalepToplamaBaslangic = request.TalepToplamaBaslangic;
            arz.TalepToplamaBitis = request.TalepToplamaBitis;
            arz.ToplamDagilacakLot = request.ToplamDagilacakLot;
            arz.Sektor = request.Sektor;
            arz.KatilimEndeksineUygunMu = request.KatilimEndeksineUygunMu;
            arz.GerceklesenKatilimciSayisi = request.GerceklesenKatilimciSayisi;
            arz.SirketOzeti = request.SirketOzeti;
            arz.DagitimYontemi = request.DagitimYontemi;
            arz.ArzBuyukluguTL = request.ArzBuyukluguTL;
            arz.HalkaAciklikOrani = request.HalkaAciklikOrani;
            arz.IskontoOrani = request.IskontoOrani;
            arz.FinansalCiroArtisi = request.FinansalCiroArtisi;
            arz.FinansalKarMarji = request.FinansalKarMarji;
            arz.FinansalBorcluluk = request.FinansalBorcluluk;

            // JSON alanlarını yeniden serileştir
            arz.FonKullanimYerleriJson = request.FonKullanimYerleri != null
                ? JsonSerializer.Serialize(request.FonKullanimYerleri)
                : null;

            arz.TaahhutlerJson = request.Taahhutler != null
                ? JsonSerializer.Serialize(request.Taahhutler)
                : null;

            // 3. YENİ EKLENDİ: Banka / Konsorsiyum Güncellemesi
            arz.Konsorsiyumlar.Clear(); // Önce eskileri tamamen temizle

            if (request.KonsorsiyumIds != null && request.KonsorsiyumIds.Any())
            {
                var yeniBankalar = await _context.Konsorsiyumlar
                    .Where(k => request.KonsorsiyumIds.Contains(k.Id))
                    .ToListAsync(cancellationToken);

                foreach (var banka in yeniBankalar)
                {
                    arz.Konsorsiyumlar.Add(banka); // Yenileri ata
                }
            }

            // 4. Değişiklikleri kaydet
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}