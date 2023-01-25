using testRestApi.Model;

namespace testRestApi.Controllers
{
    public interface ReverseGeocodingInterface
    {
        Task<List<DaDataResponse>> GetDaDataAsync(double latitude, double longitude);
    }
}
