using testRestApi.Model;

namespace testRestApi.Controllers
{
    public interface GeoCodingInterface
    {
        Task<GeoDataResponse> GetGeoDataAsync(string сountry, string city, string street);
    }
}
