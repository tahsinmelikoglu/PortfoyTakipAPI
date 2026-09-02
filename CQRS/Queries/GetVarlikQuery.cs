using MediatR;
using PortfoyTakipAPI.DTOs;
using PortfoyTakipAPI.Models;
using PortfoyTakipAPI.Repositories;
using PortfoyTakipAPI.Services;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PortfoyTakipAPI.CQRS.Queries
{
    public class GetVarliklarQuery : IRequest<PagedResult<Varlik>>
    {
        public VarlikRequestParameters Parameters { get; set; }
        public string KullaniciId { get; set; }
    }

    public class GetVarliklarQueryHandler : IRequestHandler<GetVarliklarQuery, PagedResult<Varlik>>
    {
        private readonly IVarlikRepository _repository;
        private readonly IYapayZekaService _fiyatServisi;

        public GetVarliklarQueryHandler(IVarlikRepository repository, IYapayZekaService fiyatServisi)
        {
            _repository = repository;
            _fiyatServisi = fiyatServisi;
        }

        public async Task<PagedResult<Varlik>> Handle(GetVarliklarQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _repository.GetPagedVarliklarAsync(request.Parameters);

            if (!string.IsNullOrEmpty(request.KullaniciId) && pagedResult?.Items != null)
            {
                var filtrelenmisListe = pagedResult.Items
                    .Where(v => v.KullaniciId == request.KullaniciId)
                    .ToList();

                foreach (var varlik in filtrelenmisListe)
                {
                    // Artık metot "public" olduğu için sorunsuzca çağırabiliyoruz
                    var canliFiyat = await _fiyatServisi.CanliFiyatGetirAsync(varlik.Sembol);

                    if (canliFiyat > 0)
                    {
                        varlik.GuncelFiyat = canliFiyat;
                    }
                }

                return new PagedResult<Varlik>(
                    Items: filtrelenmisListe,
                    TotalCount: filtrelenmisListe.Count,
                    PageNumber: pagedResult.PageNumber,
                    PageSize: pagedResult.PageSize
                );
            }

            return pagedResult;
        }
    }
}