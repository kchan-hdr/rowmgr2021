using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public partial class ROWM_Context
    {
        public ROWM_Context(string c) : base(Make(c)) { }

        private static string Make(string c)
        {
            var b = new EntityConnectionStringBuilder
            {
                Provider = "System.Data.SqlClient",                 
                ProviderConnectionString = c,
                Metadata= @"res://*/Model.csdl|res://*/Model.ssdl|res://*/Model.msl"
            };

            return b.ConnectionString;
        }
    }
}
