using com.hdr.rowmgr.Relocation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    /// <summary>
    /// Relocation case implementation
    /// </summary>
    public partial class RelocationCase : IRelocationCase
    {
        public string AcqFilenamePrefix => $"{ParcelKey}.{this.RelocationNumber:D3}{TypeString()}";

        public IEnumerable<IRelocationEligibilityActivity> EligibilityHistory => this.History;
        public IEnumerable<IRelocationDisplaceeActivity> DisplaceeActivities => this.Activities;

        #region helper
        [NotMapped]
        public string ParcelKey { get; set; }

        string TypeString()
        {
            string d = "";
            string r = "";

            switch ( this.DisplaceeType)
            {
                case DisplaceeType.Landlord: d = "L"; break;
                case DisplaceeType.Owner: d = "O"; break;
                case DisplaceeType.BusinessTenant: d = "TB"; break;
                case DisplaceeType.ResidentialTenant: d = "TR"; break;
                case DisplaceeType.PersonalProperty: d = "P"; break;
                case DisplaceeType.OAS: d = "OAS"; break;
            }

            //switch ( this.RelocationType)
            //{
            //    case RelocationType.Bueinsess: r = "B"; break;
            //    case RelocationType.Residential: r = "R"; break;
            //    case RelocationType.PersonalProperty: r = "P"; break;
            //    case RelocationType.OAS: r = "OAS"; break;
            //}

            return $"{d}{r}";
        }
        #endregion
    }
}
