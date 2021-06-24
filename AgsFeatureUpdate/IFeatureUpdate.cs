using System;
using System.Threading.Tasks;

namespace geographia.ags
{
    public interface IFeatureUpdate
    {
        Task<bool> UpdateFeature(string parcelId, string track, int status);
        Task<bool> UpdateFeatureRoe(string parcelId, string track, int status);
        Task<bool> UpdateFeatureRoe_Ex(string parcelId, string track, int status, string condition);
        Task<bool> UpdateFeatureOutreach(string parcelId, string track, int status, string action, DateTimeOffset? due);
        Task<bool> UpdateRating(string parcelId, string track, int rating);
        Task<bool> UpdateFeatureDocuments(string parcelId, string track, string documentURL);
        Task<(string Token, DateTimeOffset Expiration)> Token();
    }
}
