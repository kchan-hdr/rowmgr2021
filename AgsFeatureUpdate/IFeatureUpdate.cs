using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geographia.ags
{
    public interface IFeatureUpdate
    {
        Task<bool> UpdateFeature(string parcelId, string status);
    }
}
