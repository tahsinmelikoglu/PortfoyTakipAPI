using Microsoft.Extensions.Caching.Distributed; // YENİ: Redis arayüzü
using PortfoyTakipAPI.DTOs;
using PortfoyTakipAPI.Models;
using PortfoyTakipAPI.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json; // YENİ: JSON çeviri işlemleri için
using System.Threading.Tasks;

namespace PortfoyTakipAPI.Services
{
    public class VarlikService : IVarlikService
    {
        private readonly IVarlikRepository _repository;
        private readonly IDistributedCache _cache; // YENİ: Önbellek sorumlusu

        // Dependency Injection ile hem Kiler sorumlusunu hem de Önbellek sorumlusunu Aşçıya veriyoruz
        public VarlikService(IVarlikRepository repository, IDistributedCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        public IEnumerable<VarlikDTO> GetAll()
        {
            var varliklar = _repository.GetAll();

            return varliklar.Select(v => new VarlikDTO
            {
                Id = v.Id,
                Sembol = v.Sembol,
                Miktar = v.Miktar,
            }).ToList();
        }

        // --- REDIS CACHE EKLENMİŞ METOT ---
        public async Task<PagedResult<VarlikDTO>> GetPagedVarliklarAsync(VarlikRequestParameters parameters)
        {
            // 1. Müşterinin isteğine özel benzersiz bir Önbellek Anahtarı (Cache Key) üretiyoruz
            string cacheKey = $"Varliklar_Page_{parameters.PageNumber}_Size_{parameters.PageSize}_Search_{parameters.SearchTerm}";

            // 2. Önce Redis'e soruyoruz: "Tezgahta bu veri hazır var mı?"
            string cachedData = await _cache.GetStringAsync(cacheKey);

            if (!string.IsNullOrEmpty(cachedData))
            {
                // 3A. Veri Redis'te VARSA: Hiç veritabanına (kilere) inmeden veriyi dönüştürüp anında teslim et
                return JsonSerializer.Deserialize<PagedResult<VarlikDTO>>(cachedData);
            }

            // 3B. Veri Redis'te YOKSA: Veritabanına in, veriyi çek ve DTO'ya dönüştür
            var pagedData = await _repository.GetPagedVarliklarAsync(parameters);

            var dtoList = pagedData.Items.Select(v => new VarlikDTO
            {
                Id = v.Id,
                Sembol = v.Sembol,
                Miktar = v.Miktar,
            }).ToList();

            var result = new PagedResult<VarlikDTO>(dtoList, pagedData.TotalCount, pagedData.PageNumber, pagedData.PageSize);

            // 4. Çekilen bu yeni veriyi, bir dahaki sefere çok hızlı verebilmek için Redis'e kaydet
            // Sadece 5 dakika boyunca hafızada tutmasını söylüyoruz (Bayat veri görmemek için)
            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            string serializedData = JsonSerializer.Serialize(result);
            await _cache.SetStringAsync(cacheKey, serializedData, cacheOptions);

            return result;
        }
        // ----------------------------------

        public VarlikDTO GetById(int id)
        {
            var v = _repository.GetById(id);
            if (v == null) return null;

            return new VarlikDTO
            {
                Id = v.Id,
                Sembol = v.Sembol,
                Miktar = v.Miktar,
            };
        }

        public void Add(VarlikCreateDTO varlikDto)
        {
            var yeniVarlik = new Varlik
            {
                Sembol = varlikDto.Sembol,
                VarlikTuru = varlikDto.VarlikTuru,
                Miktar = varlikDto.Miktar,
            };

            _repository.Add(yeniVarlik);
            _repository.Save();
        }

        public void Update(VarlikUpdateDTO varlikDto)
        {
            var mevcutVarlik = _repository.GetById(varlikDto.Id);

            if (mevcutVarlik != null)
            {
                mevcutVarlik.Sembol = varlikDto.Sembol;
                mevcutVarlik.VarlikTuru = varlikDto.VarlikTuru;
                mevcutVarlik.Miktar = varlikDto.Miktar;

                _repository.Update(mevcutVarlik);
                _repository.Save();
            }
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
            _repository.Save();
        }
    }
}