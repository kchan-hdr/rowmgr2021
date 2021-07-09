using Relocation.DAL;
using System.Data.Entity;

namespace ROWM.Dal
{
    public class RelocationContext : DbContext
    {
        #region ctor
        public RelocationContext() : base("name=ROWM_Context") { }
        public RelocationContext(string c = "name=ROWM_Context") : base(c) { }
        #endregion

        public DbSet<ParcelRelocation> Relocations { get; set; }
        public DbSet<RelocationCase> RelocationCases { get; set; }
        public DbSet<RelocationEligibilityActivity> RelocationEligibilities { get; set; }
        public DbSet<RelocationDisplaceeActivity> RelocationActivities { get; set; }

        // types
        public DbSet<RelocationActivityType> RelocationActivity_Type { get; set; }
    }
}
