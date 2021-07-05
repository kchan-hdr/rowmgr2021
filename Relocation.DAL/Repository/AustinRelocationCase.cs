using com.hdr.rowmgr.Relocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.DAL
{
    /// <summary>
    /// Relocation case implementation
    /// </summary>
    public partial class RelocationCase : IRelocationCase
    {
        public string AcqFilenamePrefix => $"{ParcelKey()}.{this.RelocationNumber:D3}{TypeString()}";

        public IEnumerable<IRelocationEligibilityActivity> EligibilityHistory => this.History;
        public IEnumerable<IRelocationDisplaceeActivity> DisplaceeActivities => this.Activities;

        #region helper
        string ParcelKey() => "xxx";

        string TypeString()
        {
            string d = "";
            string r = "";

            switch ( this.DisplaceeType)
            {
                case DisplaceeType.Landlord: d = "L"; break;
                case DisplaceeType.Owner: d = "O"; break;
                case DisplaceeType.Tenant: d = "T"; break;
            }

            switch( this.RelocationType)
            {
                case RelocationType.Bueinsess: r = "B"; break;
                case RelocationType.Residential: r = "R"; break;
                case RelocationType.PersonalProperty: r = "P"; break;
                case RelocationType.OAS: r = "O"; break;
            }

            return $"{d}{r}";
        }
        #endregion
    }
}
