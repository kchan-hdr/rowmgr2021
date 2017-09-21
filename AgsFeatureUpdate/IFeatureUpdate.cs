using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geographia.ags
{
    public interface IFeatureUpdate
    {
        Task<bool> UpdateFeature(string parcelId, int status);
        Task<bool> UpdateFeatureRoe(string parcelId, int status);
        Task<bool> UpdateFeatureDocuments(string parcelId, string documentURL);
    }
}
