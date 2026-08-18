using PortfoyTakipAPI.Models; // Varlik modelinin bulunduğu klasör

namespace PortfoyTakipAPI.Repositories
{
    public interface IVarlikRepository
    {
        IEnumerable<Varlik> GetAll();         // Hepsini Getir (Read)
        Varlik GetById(int id);               // Tekil Getir (Read)
        void Add(Varlik varlik);              // Ekle (Create)
        void Update(Varlik varlik);           // Güncelle (Update)
        void Delete(int id);                  // Sil (Delete)
        void Save();                          // Veritabanına Kaydet
    }
}