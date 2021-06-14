namespace ROWM.Dal
{
    using Microsoft.EntityFrameworkCore;

    public partial class ROWM_Context : DbContext
    {
        public ROWM_Context(DbContextOptions<ROWM_Context> options) : base(options)
        {
        }

        public ROWM_Context(string c = "name=ROWM_Context")
        {
        }

        public virtual DbSet<Agent> Agent { get; set; }
        public virtual DbSet<Contact_Channel> Contact_Channel { get; set; }
        public virtual DbSet<Contact_Purpose> Contact_Purpose { get; set; }
        public virtual DbSet<ContactInfo> ContactInfo { get; set; }
        public virtual DbSet<ContactLog> ContactLog { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<Document_Type> Document_Type { get; set; }
        public virtual DbSet<DocumentActivity> DocumentActivity { get; set; }
        public virtual DbSet<DocumentPackage> DocumentPackage { get; set; }
        public virtual DbSet<Landowner_Score> Landowner_Score { get; set; }
        public virtual DbSet<Owner> Owner { get; set; }
        public virtual DbSet<Ownership> Ownership { get; set; }
        public virtual DbSet<Parcel> Parcel { get; set; }
        public virtual DbSet<Parcel_Status> Parcel_Status { get; set; }
        public virtual DbSet<Repesentation_Type> Repesentation_Type { get; set; }
        public virtual DbSet<Roe_Status> Roe_Status { get; set; }
        public virtual DbSet<RoeCondition> RoeCondition { get; set; }
        public virtual DbSet<StatusActivity> Activities { get; set; }
        public virtual DbSet<Followup> Followup { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<MapConfiguration> MapConfiguration { get; set; }
        public virtual DbSet<StatusCategory> StatusCategory { get; set; } 

        public virtual DbSet<DocumentTiltlePl> DocumentTitlePicklist { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>()
                .HasMany(e => e.ContactLog)
                .WithOne(e => e.Agent)
                .IsRequired()
                .HasForeignKey(e => e.ContactAgentId);

            modelBuilder.Entity<Agent>()
                .HasMany(e => e.Document)
                .WithMany(e => e.Agent)
                .UsingEntity(m => m.ToTable("DocumentAgents", "ROWM"));

            modelBuilder.Entity<ContactInfo>()
                .HasMany(e => e.ContactLog)
                .WithMany(e => e.ContactInfo)
                .UsingEntity(m => m.ToTable("ContactInfoContactLogs", "ROWM"));

            modelBuilder.Entity<ContactInfo>()
                .HasMany(e => e.Parcels)
                .WithMany(e => e.ParcelContacts)
                .UsingEntity(m => m.ToTable("ParcelContactInfo", "ROWM"));

            modelBuilder.Entity<ContactLog>()
                .HasMany(e => e.Parcel)
                .WithMany(e => e.ContactLog)
                .UsingEntity(m => m.ToTable("ParcelContactLogs", "ROWM"));

            modelBuilder.Entity<Document>()
                .HasMany(e => e.DocumentActivity)
                .WithOne(e => e.Document)
                .IsRequired()
                .HasForeignKey(e => e.ChildDocumentId);

            modelBuilder.Entity<Document>()
                .HasMany(e => e.DocumentActivity1)
                .WithOne(e => e.Document1)
                .IsRequired()
                .HasForeignKey(e => e.ParentDocumentId);

            modelBuilder.Entity<Document>()
                .HasMany(e => e.Owner)
                .WithMany(e => e.Document)
                .UsingEntity(m => m.ToTable("OwnerDocuments", "ROWM"));

            modelBuilder.Entity<Document>()
                .HasMany(e => e.Parcel)
                .WithMany(e => e.Document)
                .UsingEntity(m => m.ToTable("ParcelDocuments", "ROWM"));

            modelBuilder.Entity<DocumentPackage>()
                .HasMany(e => e.Document)
                .WithOne(e => e.DocumentPackage)
                .HasForeignKey(e => e.DocumentPackage_PackageId);

            OwnerConfiguration.Configure(modelBuilder);


            modelBuilder.Entity<Parcel>()
                .Property(e => e.County_FIPS)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<Parcel_Status>()
                .HasMany(e => e.Parcel)
                .WithOne(e => e.Parcel_Status)
                .HasForeignKey(e => e.ParcelStatusCode);

            modelBuilder.Entity<Roe_Status>()
                .HasMany(e => e.Parcel)
                .WithOne(e => e.Roe_Status)
                .HasForeignKey(e => e.RoeStatusCode);

            modelBuilder.Entity<Followup>()
                .HasOne(f => f.ParentContactLog)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Followup>()
                .HasOne(f => f.ChildContactLog)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
