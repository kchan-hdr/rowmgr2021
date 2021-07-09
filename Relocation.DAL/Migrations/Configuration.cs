namespace Relocation.DAL.Migrations
{
    using ROWM.Dal;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ROWM.Dal.RelocationContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ROWM.Dal.RelocationContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method
            //  to avoid creating duplicate seed data.

            var myTypes = new List<RelocationActivityType>
            {
                new RelocationActivityType{ ActivityTypeCode = "interview", Description = "Date Initial Interview", DisplayOrder = 0 },
                new RelocationActivityType{ ActivityTypeCode = "supplement", Description = "Supplement", DisplayOrder = 1, TrackSentConsultant=true, TrackSentClient=true, TrackClientApproval=true },
                new RelocationActivityType{ ActivityTypeCode = "self-move", Description = "Negotiated Self-Move Approval", DisplayOrder = 2, TrackSentConsultant=true, TrackSentClient=true, TrackClientApproval=true },
                new RelocationActivityType{ ActivityTypeCode = "fixed-move", Description = "Fixed Move (in lieu) Approval", DisplayOrder = 3, TrackSentConsultant=true, TrackSentClient=true, TrackClientApproval=true },
                new RelocationActivityType{ ActivityTypeCode = "storage", Description = "Temporary Storage Approval", DisplayOrder = 4,TrackSentConsultant=true, TrackSentClient=true, TrackClientApproval=true },
                new RelocationActivityType{ ActivityTypeCode = "90day", Description = "90-Day Notice", DisplayOrder = 5, TrackSent = true, TrackDelivered = true },
                new RelocationActivityType{ ActivityTypeCode = "30day", Description = "30-Day Notice", DisplayOrder = 6, TrackSent = true, TrackDelivered = true },
                new RelocationActivityType{ ActivityTypeCode = "possession", Description = "Date of Possession", DisplayOrder = 7 },
                new RelocationActivityType{ ActivityTypeCode = "vacate-extension", Description = "Vacate Extension", DisplayOrder = 8, TrackClientApproval=true },
                new RelocationActivityType{ ActivityTypeCode = "required-move", Description = "Date Required to Move", DisplayOrder = 9 },
                new RelocationActivityType{ ActivityTypeCode = "vacated", Description = "Actual Date Vacated", DisplayOrder = 10 },
                new RelocationActivityType{ ActivityTypeCode = "vacate-memo", Description = "Vacate Memo", DisplayOrder = 11, TrackSentClient=true },
                new RelocationActivityType{ ActivityTypeCode = "demo-required", Description = "Demo Required", DisplayOrder = 11, IncludeYesNo=true },
                new RelocationActivityType{ ActivityTypeCode = "benefits", Description = "Date Benefits Expire", DisplayOrder = 12 },
                new RelocationActivityType{ ActivityTypeCode = "certificate", Description = "Displacee Certificate of Completion", DisplayOrder = 13 }
            };

            foreach( var t in myTypes)
                context.RelocationActivity_Type.AddOrUpdate(t);
        }
    }
}
