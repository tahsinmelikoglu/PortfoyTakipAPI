using MediatR;
using PortfoyTakipAPI.Models;
using System.Threading;
using System.Threading.Tasks;

namespace PortfoyTakipAPI.CQRS.Commands
{
    // Command Nesnesi: Sadece silinecek kaydın ID'sini istiyoruz
    public class DeleteHalkaArzCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    // Handler Nesnesi: Veritabanında bul ve sil
    public class DeleteHalkaArzCommandHandler : IRequestHandler<DeleteHalkaArzCommand, bool>
    {
        private readonly AppDbContext _context;

        public DeleteHalkaArzCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteHalkaArzCommand request, CancellationToken cancellationToken)
        {
            var halkaArz = await _context.HalkaArzlar.FindAsync(new object[] { request.Id }, cancellationToken);

            if (halkaArz == null)
            {
                return false; // Kayıt bulunamadıysa false dön
            }

            _context.HalkaArzlar.Remove(halkaArz);
            await _context.SaveChangesAsync(cancellationToken);

            return true; // Başarıyla silindi
        }
    }
}