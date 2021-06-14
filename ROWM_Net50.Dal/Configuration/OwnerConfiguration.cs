using Microsoft.EntityFrameworkCore;
using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public static class OwnerConfiguration
    {
        public static void Configure(ModelBuilder model)
        {
            model.Entity<Owner>()
                .HasMany(o => o.Ownership)
                .WithOne(os => os.Owner)
                .HasForeignKey(nameof(Ownership.OwnerId));

            model.Entity<Owner>()
                .HasMany(o => o.RelatedOwners)
                .WithOne()
                .HasForeignKey("Owner_OwnerId");

            model.Entity<Owner>()
                .HasMany(e => e.ContactInfo)
                .WithOne(e => e.Owner)
                .IsRequired()
                .HasForeignKey(e => e.ContactOwnerId);

            model.Entity<Owner>()
                .HasMany(e => e.ContactLog)
                .WithOne(e => e.Owner)
                .HasForeignKey(e => e.Owner_OwnerId);
        }
    }
}
