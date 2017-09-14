using geographia.ags;
using Microsoft.AspNetCore.Mvc;
using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM.Controllers
{
    [Produces("application/json")]
    [Route("manager/api")]
    public class ManagerController : Controller
    {
        #region ctor
        readonly OwnerRepository _repo;
        readonly SunflowerParcel _fs;
        readonly ParcelStatusHelper _parser;
        public ManagerController(OwnerRepository r, ParcelStatusHelper p, IFeatureUpdate f)
        {
            _repo = r;
            _parser = p;
            _fs = (SunflowerParcel)f;
        }
        #endregion

        /// <summary>
        /// Get status from feature service. 
        /// Shouldn't have to do this too often. Since we're the master.
        /// </summary>
        /// <returns></returns>
        [HttpGet("agsSync"), Obsolete()]
        public async Task<IEnumerable<(string parcel, string status)>> Sync()
        {
            var parcels = await _fs.GetAllParcels();

            foreach (var parcel in parcels)
            {
                if (int.TryParse(parcel.ParcelStatus, out var s))
                {
                    var p = await _repo.GetParcel(parcel.ParcelId);
                    var sc = _parser.ParseDomainValue(s);
                    if (p.ParcelStatusCode != sc)
                    {
                        p.ParcelStatusCode = sc;
                        await _repo.UpdateParcel(p);
                    }
                }
            }

            return parcels.Select(px => (px.ParcelId, px.ParcelStatus));
        }


        #region helper. feature domain values do not match...
        /*
         * 
         * Parcel Status:
                0 – No Activities
                1 – Owner Contacted
                2 – ROE Obtained
                3 -  Offer Made
                4 – Easement Signed
                5 – Compensation Received

        static Parcel.RowStatus Parse(string agsStatus)
        {
            switch (agsStatus)
            {
                case "Contacted": return Parcel.RowStatus.Owner_Contacted;
                case "Obtained": return Parcel.RowStatus.ROE_Obtained;
                case "Denied": return Parcel.RowStatus.ROE_Obtained;
                default:
                    return Parcel.RowStatus.No_Activities;
            }
        }
        */
        #endregion
    }
}
