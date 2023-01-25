using Microsoft.AspNetCore.Mvc;
using System.Runtime;
using Dadata;
using Dadata.Model;
using testRestApi.Model;

namespace testRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReverseGeocodingController : ControllerBase, ReverseGeocodingInterface
    {
        private readonly ILogger<ReverseGeocodingController> _logger;
        private readonly IConfiguration _iConfiguration;
        public ReverseGeocodingController(ILogger<ReverseGeocodingController> logger,
              IConfiguration iConfiguration
            )
        {
            _logger = logger;
            _iConfiguration = iConfiguration;
        }

        [HttpGet(Name = "GetDaData")]
        public async Task<List<DaDataResponse>> GetDaDataAsync(double latitude, double longitude)
        {
            List<DaDataResponse> results = new List<DaDataResponse>();
            var token = _iConfiguration["DaDataAPI:Token"];
            // Количество результатов(максимум — 20)
            if (!Int32.TryParse(_iConfiguration["DaDataAPI:Count"], out var count))
            {
                count = 20;
            }
            //	Радиус поиска в метрах (максимум – 1000)
            if (!Int32.TryParse(_iConfiguration["DaDataAPI:Radius"], out var radius))
            {
                radius = 1000;
            }
            var api = new SuggestClientAsync(token);
            try
            {
                _logger.LogInformation("Запрос геопозиции, широта: {0}, долгота: {1}, радиус: {2}, количество: {3}", latitude, longitude, radius, count);
                var response = await api.Geolocate(latitude, longitude, radius, count);
                if (response != null)
                {
                    foreach (var data in response.suggestions)
                    {
                        results.Add(PasreAddress(data.value));
                        _logger.LogInformation("Результат запроса геопозиции: {0}", data.value);
                    }
                }
                else
                {
                    _logger.LogError("Ошибка запроса геопозиции");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Ошибка запроса геопозиции", ex.Message);
            }
            return results;
        }

        private DaDataResponse PasreAddress(string address)
        {
            var result = new DaDataResponse { };
            var parseString = address.Split(',');
            result.Region = parseString[0];
            result.City= parseString[1];
            result.Street= parseString[2];
            result.House= parseString[3];
            return result;
        }
    }
}
