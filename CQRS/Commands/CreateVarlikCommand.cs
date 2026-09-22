using MediatR;
using Microsoft.AspNetCore.SignalR;
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
        public DateTime AlimTarihi { get; set; }
        public string KullaniciId { get; set; }
    }

    public class CreateVarlikCommandHandler : IRequestHandler<CreateVarlikCommand, Varlik>
    {
        private readonly IVarlikRepository _repository;
        private readonly IHubContext<PortfoyHub> _hubContext;

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
                AlimTarihi = request.AlimTarihi == default ? DateTime.Now : request.AlimTarihi,
                KullaniciId = string.IsNullOrWhiteSpace(request.KullaniciId) ? "zorunlu_test" : request.KullaniciId
            };

            _repository.Add(yeniVarlik);
            _repository.Save();

            await _hubContext.Clients.All.SendAsync("BildirimAl",
                "Portföy Güncellendi 🚀",
                $"{request.Miktar} adet {request.Sembol} portföye başarıyla eklendi.");

            return await Task.FromResult(yeniVarlik);
        }
    }
}