using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public class ROWM_Context : DbContext
    {
        public DbSet<Owner> Owners { get; set; }
        public DbSet<ContactInfo> Contacts { get; set; }
        public DbSet<Parcel> Parcels { get; set; }
        public DbSet<ContactLog> ContactLogs { get; set; }

        public DbSet<Agent> Agents { get; set; }

        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentPackage> DocumentPackages { get; set; }
        public DbSet<DocumentTracking> DocumentActivities { get; set; }

        // vocabulary
        public DbSet<Channel_Master> Channels { get; set; }
        public DbSet<Purpose_Master> Purposes { get; set; }
        public DbSet<Representation> Representations { get; set; }

        public DbSet<ParcelStatus_Master> ParcelStatus { get; set; }
        public DbSet<RoeStatus_Master> RoeStatus { get; set; }

        // for seed
        public DbSet<Ownership_import> OwnershipWorking { get; set; }

        public ROWM_Context() : base(DbConnection.GetConnectionString()) { }
    }
}
