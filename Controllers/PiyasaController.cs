using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PortfoyTakipAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PiyasaController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        // Dependency Injection ile HttpClient alıyoruz
        public PiyasaController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            // Yahoo Finance bazen User-Agent olmadan gelen istekleri bot sanıp reddeder.
            if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            {
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
            }
        }

        [HttpGet("ozet")]
        public async Task<IActionResult> GetPiyasaOzeti()
        {
            try
            {
                var bist100 = await GetYahooFinanceData("XU100.IS");
                var bist30 = await GetYahooFinanceData("XU030.IS");
                var xbank = await GetYahooFinanceData("XBANK.IS");
                var usdTry = await GetYahooFinanceData("TRY=X");

                return Ok(new
                {
                    Bist100 = bist100,
                    Bist30 = bist30,
                    Xbank = xbank,
                    UsdTry = usdTry
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Piyasa verileri çekilemedi: " + ex.Message);
            }
        }

        private async Task<object> GetYahooFinanceData(string symbol)
        {
            var url = $"https://query1.finance.yahoo.com/v8/finance/chart/{symbol}?interval=1d&range=1d";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonString = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(jsonString);

            var meta = document.RootElement
                .GetProperty("chart")
                .GetProperty("result")[0]
                .GetProperty("meta");

            decimal guncelFiyat = meta.GetProperty("regularMarketPrice").GetDecimal();
            decimal oncekiKapanis = meta.GetProperty("chartPreviousClose").GetDecimal();
            decimal degisimYuzdesi = ((guncelFiyat - oncekiKapanis) / oncekiKapanis) * 100;

            return new
            {
                Deger = guncelFiyat,
                Yuzde = degisimYuzdesi
            };
        }
    }
}