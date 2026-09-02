using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Client;
using PortfoyTakipAPI.Hubs;
using PortfoyTakipAPI.Models;
using PortfoyTakipAPI.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PortfoyTakipAPI.CQRS.Commands
{
    public class CreateVarlikCommand : IRequest<Varlik>
    {
        public string Sembol { get; set; }
        public string VarlikTuru { get; set; }
        public decimal Miktar { get; set; }
        public decimal AlisFiyati { get; set; }
        public DateTime AlimTarihi  { get; set; }
        public string KullaniciId { get; set; }
    }

    public class CreateVarlikCommandHandler : IRequestHandler<CreateVarlikCommand, Varlik>
    {
        private readonly IVarlikRepository _repository;
        private readonly IHubContext<PortfoyHub> _hubContext; // YENİ: SignalR Kulesi

        // Controller yerine Mediator, Mediator yerine de Handler kuleyi çağırıyor
        public CreateVarlikCommandHandler(IVarlikRepository repository, IHubContext<PortfoyHub> hubContext)
        {
            _repository = repository;
            _hubContext = hubContext;
        }

        public async Task<Varlik> Handle(CreateVarlikCommand request, CancellationToken cancellationToken)
        {
            var yeniVarlik = new Varlik
            {
                Sembol = request.Sembol,
                VarlikTuru = request.VarlikTuru,
                Miktar = request.Miktar,
                AlisFiyati = request.AlisFiyati,
                Bakiye = request.Miktar * request.AlisFiyati,
                AlimTarihi = request.AlimTarihi
            };

            // 1. Veritabanına kalıcı olarak kaydet
            _repository.Add(yeniVarlik);
            _repository.Save();

            // 2. SİHİRLİ DOKUNUŞ: Kayıt başarılı olduğu an anlık bildirim fırlat!
            await _hubContext.Clients.All.SendAsync("BildirimAl",
                "Yeni Varlık Eklendi 🚀",
                $"{request.Miktar} adet {request.Sembol} portföye başarıyla eklendi.");

            return await Task.FromResult(yeniVarlik);
        }
    }
}