using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using testRestApi.Model;

namespace testRestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeoCodingController : ControllerBase, GeoCodingInterface
    {
        private readonly ILogger<GeoCodingController> _logger;
        private readonly IConfiguration _iConfiguration;
        public GeoCodingController(IConfiguration iConfiguration,
            ILogger<GeoCodingController> logger)
        {
            _iConfiguration = iConfiguration;
            _logger = logger;
        }

        [HttpGet(Name = "GetGeoData")]
        public async Task<GeoDataResponse?> GetGeoDataAsync(string сountry, string city, string street)
        {
            if(string.IsNullOrEmpty(сountry))
            {
                throw new ArgumentNullException("Поле {0} не должно быть пустым", сountry);
            }
            if (string.IsNullOrEmpty(city))
            {
                throw new ArgumentNullException("Поле {0} не должно быть пустым", city);
            }
            if (string.IsNullOrEmpty(street))
            {
                throw new ArgumentNullException("Поле {0} не должно быть пустым", street);
            }

            var link = string.Format(
            _iConfiguration["OpenstreetmapAPI:Url"] + "country={0}&city={1}&street={2}" + "&format=json&limit=2",
            сountry,
            city,
            street);

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/2.0)");
                    _logger.LogInformation("Запрос геокодирования, страна: {0}, город: {1}, улица: {2}", сountry, city, street);
                    HttpResponseMessage response = await client.GetAsync(link);
                    HttpContent content = response.Content;
                    string result = await content.ReadAsStringAsync();
                    var parsedResponse = JsonConvert.DeserializeObject<List<RootObject>>(result);
                    if(response.StatusCode != HttpStatusCode.OK) 
                    {
                        _logger.LogError("Не удалось выполнить запрос, код сервера : {0}", response.StatusCode);
                        throw new Exception("Не удалось выполнить запрос, код сервера :" + response.StatusCode);
                    }
                    if (parsedResponse != null && parsedResponse.Count != 0)
                    {
                        _logger.LogInformation("Координаты найдены, широта: {0}, долгота: {1}", parsedResponse[0].lat, parsedResponse[0].lon);
                        return new GeoDataResponse { Latitude = parsedResponse[0].lat, Longitude = parsedResponse[0].lon, Name = parsedResponse[0].display_name };
                    }
                    else
                    {
                        _logger.LogInformation("Координаты не найдены, страна: {0}, город: {1}, улица: {2}", сountry, city, street);
                        throw new Exception("Координаты не найдены, страна:" + сountry + ", город: " + city + ", улица:" + street);
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex,
                        "Не удалось выполнить запрос : {0}", ex.StackTrace);
                    return null;
                }
            }

        }

    }
}
