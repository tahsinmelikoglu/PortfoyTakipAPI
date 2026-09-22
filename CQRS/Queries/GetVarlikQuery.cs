using MediatR;
using PortfoyTakipAPI.DTOs;
using PortfoyTakipAPI.Models;
using PortfoyTakipAPI.Repositories;
using PortfoyTakipAPI.Services; // BİST Servisini kullanabilmek için bunu ekledik
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
        private readonly IBistService _bistService; // YENİ: BİST Servisini tanımladık

        // YENİ: BİST Servisini Constructor (Yapıcı Metot) içerisine dahil ettik
        public GetVarliklarQueryHandler(IVarlikRepository repository, IBistService bistService)
        {
            _repository = repository;
            _bistService = bistService;
        }

        public async Task<PagedResult<Varlik>> Handle(GetVarliklarQuery request, CancellationToken cancellationToken)
        {
            var pagedResult = await _repository.GetPagedVarliklarAsync(request.Parameters);

            if (!string.IsNullOrEmpty(request.KullaniciId) && pagedResult?.Items != null)
            {
                var filtrelenmisListe = pagedResult.Items
                    .Where(v => v.KullaniciId == request.KullaniciId)
                    .ToList();

                // YENİ: Portföydeki her bir hisse için Yahoo Finance'e gidip güncel fiyatı soruyoruz
                foreach (var varlik in filtrelenmisListe)
                {
                    // Eğer varlık türü Hisse ise canlı fiyat çek (Altın/Döviz vs. eklersen buraya IF koyabilirsin)
                    var canliFiyat = await _bistService.GetHisseFiyatiAsync(varlik.Sembol);

                    // Eğer API'den fiyat başarıyla geldiyse, varlığın güncel fiyatını ez
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