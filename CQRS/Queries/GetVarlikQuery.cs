using MediatR;
using PortfoyTakipAPI.DTOs;
using PortfoyTakipAPI.Models;
using PortfoyTakipAPI.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace PortfoyTakipAPI.CQRS.Queries
{
    // 1. SORGU (PAKET): Listeleme yaparken kullanılacak sayfalama/filtre parametrelerini taşıyan paket
    public class GetVarliklarQuery : IRequest<PagedResult<Varlik>>
    {
        public VarlikRequestParameters Parameters { get; set; }
    }

    // 2. İŞLEYİCİ (HANDLER): Paketi alıp veritabanından listeyi çeken asıl uzman
    public class GetVarliklarQueryHandler : IRequestHandler<GetVarliklarQuery, PagedResult<Varlik>>
    {
        private readonly IVarlikRepository _repository;

        public GetVarliklarQueryHandler(IVarlikRepository repository)
        {
            _repository = repository;
        }

        public async Task<PagedResult<Varlik>> Handle(GetVarliklarQuery request, CancellationToken cancellationToken)
        {
            // Artık Service katmanını aradan çıkardık, Handler doğrudan Repository'den veriyi çekiyor!
            return await _repository.GetPagedVarliklarAsync(request.Parameters);
        }
    }
}