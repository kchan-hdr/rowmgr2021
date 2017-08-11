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

        public ROWM_Context() : base(DbConnection.GetConnectionString()) { }
    }
}
