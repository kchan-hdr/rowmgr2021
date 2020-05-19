using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ROWM
{
    public partial class rowmContext : DbContext
    {
        public rowmContext()
        {
        }

        public rowmContext(DbContextOptions<rowmContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Agent> Agent { get; set; }
        public virtual DbSet<Appraisal> Appraisal { get; set; }
        public virtual DbSet<ContactChannel> ContactChannel { get; set; }
        public virtual DbSet<ContactFollowup> ContactFollowup { get; set; }
        public virtual DbSet<ContactInfo> ContactInfo { get; set; }
        public virtual DbSet<ContactInfoContactLogs> ContactInfoContactLogs { get; set; }
        public virtual DbSet<ContactLog> ContactLog { get; set; }
        public virtual DbSet<ContactPurpose> ContactPurpose { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<DocumentActivity> DocumentActivity { get; set; }
        public virtual DbSet<DocumentAgents> DocumentAgents { get; set; }
        public virtual DbSet<DocumentPackage> DocumentPackage { get; set; }
        public virtual DbSet<DocumentType> DocumentType { get; set; }
        public virtual DbSet<LandownerScore> LandownerScore { get; set; }
        public virtual DbSet<Map> Map { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<Owner> Owner { get; set; }
        public virtual DbSet<OwnerDocuments> OwnerDocuments { get; set; }
        public virtual DbSet<Ownership> Ownership { get; set; }
        public virtual DbSet<Parcel> Parcel { get; set; }
        public virtual DbSet<ParcelContactInfo> ParcelContactInfo { get; set; }
        public virtual DbSet<ParcelContactLogs> ParcelContactLogs { get; set; }
        public virtual DbSet<ParcelDocuments> ParcelDocuments { get; set; }
        public virtual DbSet<ParcelStatus> ParcelStatus { get; set; }
        public virtual DbSet<PreferredContactMode> PreferredContactMode { get; set; }
        public virtual DbSet<RepresentationType> RepresentationType { get; set; }
        public virtual DbSet<RoeConditions> RoeConditions { get; set; }
        public virtual DbSet<RoeStatus> RoeStatus { get; set; }
        public virtual DbSet<StatusActivity> StatusActivity { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=wharton-dev-rowm.database.windows.net;Initial Catalog=rowm;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Agent>(entity =>
            {
                entity.ToTable("Agent", "ROWM");

                entity.Property(e => e.AgentId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.AgentName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            });

            modelBuilder.Entity<Appraisal>(entity =>
            {
                entity.ToTable("Appraisal", "ROWM");

                entity.Property(e => e.AppraisalId)
                    .HasColumnName("Appraisal_Id")
                    .HasDefaultValueSql("(newid())");

                entity.Property(e => e.AppraisalConditions).HasColumnName("Appraisal_Conditions");

                entity.Property(e => e.AppraisalDate)
                    .HasColumnName("Appraisal_Date")
                    .HasColumnType("date");

                entity.Property(e => e.AppraisalFirm)
                    .HasColumnName("Appraisal_Firm")
                    .HasMaxLength(200);

                entity.Property(e => e.AppraisedAcrage).HasColumnName("Appraised_Acrage");

                entity.Property(e => e.AppraisedValue)
                    .HasColumnName("Appraised_Value")
                    .HasColumnType("money");

                entity.Property(e => e.AppraisedValueType)
                    .HasColumnName("Appraised_Value_Type")
                    .HasMaxLength(200);

                entity.Property(e => e.Appraiser)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.ReportDate)
                    .HasColumnName("Report_Date")
                    .HasColumnType("date");

                entity.Property(e => e.ReportId).HasColumnName("Report_Id");

                entity.Property(e => e.ReviewerApprovalDate)
                    .HasColumnName("Reviewer_Approval_Date")
                    .HasColumnType("date");

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.Appraisal)
                    .HasForeignKey(d => d.AgentId)
                    .HasConstraintName("FK_Appraisal_Agent");

                entity.HasOne(d => d.Parcel)
                    .WithMany(p => p.Appraisal)
                    .HasForeignKey(d => d.ParcelId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Appraisal_Parcel");

                entity.HasOne(d => d.Report)
                    .WithMany(p => p.Appraisal)
                    .HasForeignKey(d => d.ReportId)
                    .HasConstraintName("FK_Appraisal_DocumentPackage");
            });

            modelBuilder.Entity<ContactChannel>(entity =>
            {
                entity.HasKey(e => e.ContactTypeCode);

                entity.ToTable("Contact_Channel", "ROWM");

                entity.Property(e => e.ContactTypeCode)
                    .HasMaxLength(20)
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<ContactFollowup>(entity =>
            {
                entity.ToTable("Contact_Followup", "ROWM");

                entity.HasIndex(e => e.ChildContactLogId)
                    .HasName("IX_ChildContactLogId");

                entity.HasIndex(e => e.ParentContactLogId)
                    .HasName("IX_ParentContactLogId");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.HasOne(d => d.ChildContactLog)
                    .WithMany(p => p.ContactFollowupChildContactLog)
                    .HasForeignKey(d => d.ChildContactLogId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ROWM.Contact_Followup_ROWM.ContactLog_ChildContactLogId");

                entity.HasOne(d => d.ParentContactLog)
                    .WithMany(p => p.ContactFollowupParentContactLog)
                    .HasForeignKey(d => d.ParentContactLogId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ROWM.Contact_Followup_ROWM.ContactLog_ParentContactLogId");
            });

            modelBuilder.Entity<ContactInfo>(entity =>
            {
                entity.HasKey(e => e.ContactId);

                entity.ToTable("ContactInfo", "ROWM");

                entity.HasIndex(e => e.ContactOwnerId)
                    .HasName("IX_ContactOwnerId");

                entity.HasIndex(e => e.OrganizationId)
                    .HasName("IX_OrganizationId");

                entity.Property(e => e.ContactId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.City).HasMaxLength(100);

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.PreferredContactMode).HasMaxLength(10);

                entity.Property(e => e.Representation)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.State).HasMaxLength(20);

                entity.Property(e => e.StreetAddress).HasMaxLength(400);

                entity.Property(e => e.Zip)
                    .HasColumnName("ZIP")
                    .HasMaxLength(10);

                entity.HasOne(d => d.ContactOwner)
                    .WithMany(p => p.ContactInfo)
                    .HasForeignKey(d => d.ContactOwnerId)
                    .HasConstraintName("FK_ROWM.ContactInfo_ROWM.Owner_ContactOwnerId");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.ContactInfo)
                    .HasForeignKey(d => d.OrganizationId)
                    .HasConstraintName("FK_ROWM.ContactInfo_ROWM.Organization_OrganizationId");

                entity.HasOne(d => d.PreferredContactModeNavigation)
                    .WithMany(p => p.ContactInfo)
                    .HasForeignKey(d => d.PreferredContactMode)
                    .HasConstraintName("FK_MODE");
            });

            modelBuilder.Entity<ContactInfoContactLogs>(entity =>
            {
                entity.HasKey(e => new { e.ContactInfoContactId, e.ContactLogContactLogId });

                entity.ToTable("ContactInfoContactLogs", "ROWM");

                entity.HasIndex(e => e.ContactInfoContactId)
                    .HasName("IX_ContactInfo_ContactId");

                entity.HasIndex(e => e.ContactLogContactLogId)
                    .HasName("IX_ContactLog_ContactLogId");

                entity.Property(e => e.ContactInfoContactId).HasColumnName("ContactInfo_ContactId");

                entity.Property(e => e.ContactLogContactLogId).HasColumnName("ContactLog_ContactLogId");

                entity.HasOne(d => d.ContactInfoContact)
                    .WithMany(p => p.ContactInfoContactLogs)
                    .HasForeignKey(d => d.ContactInfoContactId)
                    .HasConstraintName("FK_ROWM.ContactInfoContactLogs_ROWM.ContactInfo_ContactInfo_ContactId");

                entity.HasOne(d => d.ContactLogContactLog)
                    .WithMany(p => p.ContactInfoContactLogs)
                    .HasForeignKey(d => d.ContactLogContactLogId)
                    .HasConstraintName("FK_ROWM.ContactInfoContactLogs_ROWM.ContactLog_ContactLog_ContactLogId");
            });

            modelBuilder.Entity<ContactLog>(entity =>
            {
                entity.ToTable("ContactLog", "ROWM");

                entity.HasIndex(e => e.ContactAgentId)
                    .HasName("IX_ContactAgentId");

                entity.HasIndex(e => e.OwnerOwnerId)
                    .HasName("IX_Owner_OwnerId");

                entity.Property(e => e.ContactLogId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.ContactChannel).HasMaxLength(20);

                entity.Property(e => e.LandownerScore).HasColumnName("Landowner_Score");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.OwnerOwnerId).HasColumnName("Owner_OwnerId");

                entity.Property(e => e.ProjectPhase).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(200);

                entity.HasOne(d => d.ContactAgent)
                    .WithMany(p => p.ContactLog)
                    .HasForeignKey(d => d.ContactAgentId)
                    .HasConstraintName("FK_ROWM.ContactLog_ROWM.Agent_ContactAgentId");

                entity.HasOne(d => d.OwnerOwner)
                    .WithMany(p => p.ContactLog)
                    .HasForeignKey(d => d.OwnerOwnerId)
                    .HasConstraintName("FK_ROWM.ContactLog_ROWM.Owner_Owner_OwnerId");
            });

            modelBuilder.Entity<ContactPurpose>(entity =>
            {
                entity.HasKey(e => e.PurposeCode);

                entity.ToTable("Contact_Purpose", "ROWM");

                entity.HasIndex(e => e.MilestoneCode)
                    .HasName("IX_MilestoneCode");

                entity.Property(e => e.PurposeCode)
                    .HasMaxLength(50)
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.MilestoneCode).HasMaxLength(40);

                entity.HasOne(d => d.MilestoneCodeNavigation)
                    .WithMany(p => p.ContactPurpose)
                    .HasForeignKey(d => d.MilestoneCode)
                    .HasConstraintName("FK_ROWM.Contact_Purpose_ROWM.Parcel_Status_MilestoneCode");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("Document", "ROWM");

                entity.HasIndex(e => e.ContactLogContactLogId)
                    .HasName("IX_ContactLog_ContactLogId");

                entity.HasIndex(e => e.DocumentPackagePackageId)
                    .HasName("IX_DocumentPackage_PackageId");

                entity.Property(e => e.DocumentId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.ClientTrackingNumber).HasMaxLength(100);

                entity.Property(e => e.ContactLogContactLogId).HasColumnName("ContactLog_ContactLogId");

                entity.Property(e => e.ContentType).HasMaxLength(100);

                entity.Property(e => e.DocumentPackagePackageId).HasColumnName("DocumentPackage_PackageId");

                entity.Property(e => e.DocumentType).HasMaxLength(200);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.Qcdate).HasColumnName("QCDate");

                entity.Property(e => e.RowmTrackingNumber).HasMaxLength(100);

                entity.Property(e => e.SourceFilename).HasMaxLength(500);

                entity.HasOne(d => d.ContactLogContactLog)
                    .WithMany(p => p.Document)
                    .HasForeignKey(d => d.ContactLogContactLogId)
                    .HasConstraintName("FK_ROWM.Document_ROWM.ContactLog_ContactLog_ContactLogId");

                entity.HasOne(d => d.DocumentNavigation)
                    .WithOne(p => p.InverseDocumentNavigation)
                    .HasForeignKey<Document>(d => d.DocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Document_Document");

                entity.HasOne(d => d.DocumentPackagePackage)
                    .WithMany(p => p.Document)
                    .HasForeignKey(d => d.DocumentPackagePackageId)
                    .HasConstraintName("FK_ROWM.Document_ROWM.DocumentPackage_DocumentPackage_PackageId");

                entity.HasOne(d => d.DocumentTypeNavigation)
                    .WithMany(p => p.Document)
                    .HasForeignKey(d => d.DocumentType)
                    .HasConstraintName("FK_Document_Document_Type");
            });

            modelBuilder.Entity<DocumentActivity>(entity =>
            {
                entity.HasKey(e => e.ActivityId);

                entity.ToTable("DocumentActivity", "ROWM");

                entity.HasIndex(e => e.AgentId)
                    .HasName("IX_AgentId");

                entity.HasIndex(e => e.ChildDocumentId)
                    .HasName("IX_ChildDocumentId");

                entity.HasIndex(e => e.ParentDocumentId)
                    .HasName("IX_ParentDocumentId");

                entity.Property(e => e.ActivityId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.Activity)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.DocumentActivity)
                    .HasForeignKey(d => d.AgentId)
                    .HasConstraintName("FK_ROWM.DocumentActivity_ROWM.Agent_AgentId");

                entity.HasOne(d => d.ChildDocument)
                    .WithMany(p => p.DocumentActivityChildDocument)
                    .HasForeignKey(d => d.ChildDocumentId)
                    .HasConstraintName("FK_ROWM.DocumentActivity_ROWM.Document_ChildDocumentId");

                entity.HasOne(d => d.ParentDocument)
                    .WithMany(p => p.DocumentActivityParentDocument)
                    .HasForeignKey(d => d.ParentDocumentId)
                    .HasConstraintName("FK_ROWM.DocumentActivity_ROWM.Document_ParentDocumentId");
            });

            modelBuilder.Entity<DocumentAgents>(entity =>
            {
                entity.HasKey(e => new { e.AgentAgentId, e.DocumentDocumentId });

                entity.ToTable("DocumentAgents", "ROWM");

                entity.HasIndex(e => e.AgentAgentId)
                    .HasName("IX_Agent_AgentId");

                entity.HasIndex(e => e.DocumentDocumentId)
                    .HasName("IX_Document_DocumentId");

                entity.Property(e => e.AgentAgentId).HasColumnName("Agent_AgentId");

                entity.Property(e => e.DocumentDocumentId).HasColumnName("Document_DocumentId");

                entity.HasOne(d => d.AgentAgent)
                    .WithMany(p => p.DocumentAgents)
                    .HasForeignKey(d => d.AgentAgentId)
                    .HasConstraintName("FK_ROWM.DocumentAgents_ROWM.Agent_Agent_AgentId");

                entity.HasOne(d => d.DocumentDocument)
                    .WithMany(p => p.DocumentAgents)
                    .HasForeignKey(d => d.DocumentDocumentId)
                    .HasConstraintName("FK_ROWM.DocumentAgents_ROWM.Document_Document_DocumentId");
            });

            modelBuilder.Entity<DocumentPackage>(entity =>
            {
                entity.HasKey(e => e.PackageId);

                entity.ToTable("DocumentPackage", "ROWM");

                entity.Property(e => e.PackageId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.PackageName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<DocumentType>(entity =>
            {
                entity.HasKey(e => e.DocTypeName);

                entity.ToTable("Document_Type", "ROWM");

                entity.HasIndex(e => e.MilestoneCode)
                    .HasName("IX_MilestoneCode");

                entity.Property(e => e.DocTypeName)
                    .HasMaxLength(200)
                    .ValueGeneratedNever();

                entity.Property(e => e.FolderPath).HasMaxLength(400);

                entity.Property(e => e.MilestoneCode).HasMaxLength(40);

                entity.HasOne(d => d.MilestoneCodeNavigation)
                    .WithMany(p => p.DocumentType)
                    .HasForeignKey(d => d.MilestoneCode)
                    .HasConstraintName("FK_ROWM.Document_Type_ROWM.Parcel_Status_MilestoneCode");
            });

            modelBuilder.Entity<LandownerScore>(entity =>
            {
                entity.HasKey(e => e.Score);

                entity.ToTable("Landowner_Score", "ROWM");

                entity.Property(e => e.Score).ValueGeneratedNever();

                entity.Property(e => e.Caption).HasMaxLength(50);
            });

            modelBuilder.Entity<Map>(entity =>
            {
                entity.ToTable("Map", "App");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.AgsUrl).HasMaxLength(2048);

                entity.Property(e => e.LayerId).HasMaxLength(10);
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.ToTable("Organization", "ROWM");

                entity.Property(e => e.OrganizationId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(200);
            });

            modelBuilder.Entity<Owner>(entity =>
            {
                entity.ToTable("Owner", "ROWM");

                entity.HasIndex(e => e.OwnerOwnerId)
                    .HasName("IX_Owner_OwnerId");

                entity.Property(e => e.OwnerId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.OwnerOwnerId).HasColumnName("Owner_OwnerId");

                entity.Property(e => e.OwnerType).HasMaxLength(50);

                entity.Property(e => e.PartyName).HasMaxLength(200);

                entity.HasOne(d => d.OwnerOwner)
                    .WithMany(p => p.InverseOwnerOwner)
                    .HasForeignKey(d => d.OwnerOwnerId)
                    .HasConstraintName("FK_ROWM.Owner_ROWM.Owner_Owner_OwnerId");
            });

            modelBuilder.Entity<OwnerDocuments>(entity =>
            {
                entity.HasKey(e => new { e.DocumentDocumentId, e.OwnerOwnerId });

                entity.ToTable("OwnerDocuments", "ROWM");

                entity.HasIndex(e => e.DocumentDocumentId)
                    .HasName("IX_Document_DocumentId");

                entity.HasIndex(e => e.OwnerOwnerId)
                    .HasName("IX_Owner_OwnerId");

                entity.Property(e => e.DocumentDocumentId).HasColumnName("Document_DocumentId");

                entity.Property(e => e.OwnerOwnerId).HasColumnName("Owner_OwnerId");

                entity.HasOne(d => d.DocumentDocument)
                    .WithMany(p => p.OwnerDocuments)
                    .HasForeignKey(d => d.DocumentDocumentId)
                    .HasConstraintName("FK_ROWM.OwnerDocuments_ROWM.Document_Document_DocumentId");

                entity.HasOne(d => d.OwnerOwner)
                    .WithMany(p => p.OwnerDocuments)
                    .HasForeignKey(d => d.OwnerOwnerId)
                    .HasConstraintName("FK_ROWM.OwnerDocuments_ROWM.Owner_Owner_OwnerId");
            });

            modelBuilder.Entity<Ownership>(entity =>
            {
                entity.ToTable("Ownership", "ROWM");

                entity.HasIndex(e => e.OwnerId)
                    .HasName("IX_OwnerId");

                entity.HasIndex(e => e.ParcelId)
                    .HasName("IX_ParcelId");

                entity.Property(e => e.OwnershipId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.OwnershipT).HasColumnName("Ownership_t");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Ownership)
                    .HasForeignKey(d => d.OwnerId)
                    .HasConstraintName("FK_ROWM.Ownership_ROWM.Owner_OwnerId");

                entity.HasOne(d => d.Parcel)
                    .WithMany(p => p.Ownership)
                    .HasForeignKey(d => d.ParcelId)
                    .HasConstraintName("FK_ROWM.Ownership_ROWM.Parcel_ParcelId");
            });

            modelBuilder.Entity<Parcel>(entity =>
            {
                entity.ToTable("Parcel", "ROWM");

                entity.HasIndex(e => e.ParcelParcelId)
                    .HasName("IX_Parcel_ParcelId");

                entity.HasIndex(e => e.ParcelStatusCode)
                    .HasName("IX_ParcelStatusCode");

                entity.HasIndex(e => e.RoeStatusCode)
                    .HasName("IX_RoeStatusCode");

                entity.Property(e => e.ParcelId).HasDefaultValueSql("(newsequentialid())");

                entity.Property(e => e.AssessorParcelNumber)
                    .IsRequired()
                    .HasColumnName("Assessor_Parcel_Number")
                    .HasMaxLength(128);

                entity.Property(e => e.CountyFips)
                    .IsRequired()
                    .HasColumnName("County_FIPS")
                    .HasMaxLength(5)
                    .IsUnicode(false);

                entity.Property(e => e.CountyName)
                    .HasColumnName("County_Name")
                    .HasMaxLength(50);

                entity.Property(e => e.FinalEasementOfferOfferAmount).HasColumnName("FinalEasementOffer_OfferAmount");

                entity.Property(e => e.FinalEasementOfferOfferDate).HasColumnName("FinalEasementOffer_OfferDate");

                entity.Property(e => e.FinalEasementOfferOfferNotes).HasColumnName("FinalEasementOffer_OfferNotes");

                entity.Property(e => e.FinalOptionOfferOfferAmount).HasColumnName("FinalOptionOffer_OfferAmount");

                entity.Property(e => e.FinalOptionOfferOfferDate).HasColumnName("FinalOptionOffer_OfferDate");

                entity.Property(e => e.FinalOptionOfferOfferNotes).HasColumnName("FinalOptionOffer_OfferNotes");

                entity.Property(e => e.FinalRoeofferOfferAmount).HasColumnName("FinalROEOffer_OfferAmount");

                entity.Property(e => e.FinalRoeofferOfferDate).HasColumnName("FinalROEOffer_OfferDate");

                entity.Property(e => e.FinalRoeofferOfferNotes).HasColumnName("FinalROEOffer_OfferNotes");

                entity.Property(e => e.InitialEasementOfferOfferAmount).HasColumnName("InitialEasementOffer_OfferAmount");

                entity.Property(e => e.InitialEasementOfferOfferDate).HasColumnName("InitialEasementOffer_OfferDate");

                entity.Property(e => e.InitialEasementOfferOfferNotes).HasColumnName("InitialEasementOffer_OfferNotes");

                entity.Property(e => e.InitialOptionOfferOfferAmount).HasColumnName("InitialOptionOffer_OfferAmount");

                entity.Property(e => e.InitialOptionOfferOfferDate).HasColumnName("InitialOptionOffer_OfferDate");

                entity.Property(e => e.InitialOptionOfferOfferNotes).HasColumnName("InitialOptionOffer_OfferNotes");

                entity.Property(e => e.InitialRoeofferOfferAmount).HasColumnName("InitialROEOffer_OfferAmount");

                entity.Property(e => e.InitialRoeofferOfferDate).HasColumnName("InitialROEOffer_OfferDate");

                entity.Property(e => e.InitialRoeofferOfferNotes).HasColumnName("InitialROEOffer_OfferNotes");

                entity.Property(e => e.LandownerScore).HasColumnName("Landowner_Score");

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ParcelParcelId).HasColumnName("Parcel_ParcelId");

                entity.Property(e => e.ParcelStatusCode).HasMaxLength(40);

                entity.Property(e => e.RoeStatusCode).HasMaxLength(40);

                entity.Property(e => e.SitusAddress).HasMaxLength(800);

                entity.Property(e => e.TrackingNumber)
                    .HasColumnName("Tracking_Number")
                    .HasMaxLength(100);

                entity.HasOne(d => d.ParcelParcel)
                    .WithMany(p => p.InverseParcelParcel)
                    .HasForeignKey(d => d.ParcelParcelId)
                    .HasConstraintName("FK_ROWM.Parcel_ROWM.Parcel_Parcel_ParcelId");

                entity.HasOne(d => d.ParcelStatusCodeNavigation)
                    .WithMany(p => p.Parcel)
                    .HasForeignKey(d => d.ParcelStatusCode)
                    .HasConstraintName("FK_ROWM.Parcel_ROWM.Parcel_Status_ParcelStatusCode");

                entity.HasOne(d => d.RoeStatusCodeNavigation)
                    .WithMany(p => p.Parcel)
                    .HasForeignKey(d => d.RoeStatusCode)
                    .HasConstraintName("FK_ROWM.Parcel_ROWM.Roe_Status_RoeStatusCode");
            });

            modelBuilder.Entity<ParcelContactInfo>(entity =>
            {
                entity.HasKey(e => new { e.ContactInfoContactId, e.ParcelParcelId });

                entity.ToTable("ParcelContactInfo", "ROWM");

                entity.HasIndex(e => e.ContactInfoContactId)
                    .HasName("IX_ContactInfo_ContactId");

                entity.HasIndex(e => e.ParcelParcelId)
                    .HasName("IX_Parcel_ParcelId");

                entity.Property(e => e.ContactInfoContactId).HasColumnName("ContactInfo_ContactId");

                entity.Property(e => e.ParcelParcelId).HasColumnName("Parcel_ParcelId");

                entity.HasOne(d => d.ContactInfoContact)
                    .WithMany(p => p.ParcelContactInfo)
                    .HasForeignKey(d => d.ContactInfoContactId)
                    .HasConstraintName("FK_dbo.ParcelContactInfoes_ROWM.ContactInfo_ContactInfo_ContactId");

                entity.HasOne(d => d.ParcelParcel)
                    .WithMany(p => p.ParcelContactInfo)
                    .HasForeignKey(d => d.ParcelParcelId)
                    .HasConstraintName("FK_dbo.ParcelContactInfoes_ROWM.Parcel_Parcel_ParcelId");
            });

            modelBuilder.Entity<ParcelContactLogs>(entity =>
            {
                entity.HasKey(e => new { e.ContactLogContactLogId, e.ParcelParcelId });

                entity.ToTable("ParcelContactLogs", "ROWM");

                entity.HasIndex(e => e.ContactLogContactLogId)
                    .HasName("IX_ContactLog_ContactLogId");

                entity.HasIndex(e => e.ParcelParcelId)
                    .HasName("IX_Parcel_ParcelId");

                entity.Property(e => e.ContactLogContactLogId).HasColumnName("ContactLog_ContactLogId");

                entity.Property(e => e.ParcelParcelId).HasColumnName("Parcel_ParcelId");

                entity.HasOne(d => d.ContactLogContactLog)
                    .WithMany(p => p.ParcelContactLogs)
                    .HasForeignKey(d => d.ContactLogContactLogId)
                    .HasConstraintName("FK_ROWM.ParcelContactLogs_ROWM.ContactLog_ContactLog_ContactLogId");

                entity.HasOne(d => d.ParcelParcel)
                    .WithMany(p => p.ParcelContactLogs)
                    .HasForeignKey(d => d.ParcelParcelId)
                    .HasConstraintName("FK_ROWM.ParcelContactLogs_ROWM.Parcel_Parcel_ParcelId");
            });

            modelBuilder.Entity<ParcelDocuments>(entity =>
            {
                entity.HasKey(e => new { e.DocumentDocumentId, e.ParcelParcelId });

                entity.ToTable("ParcelDocuments", "ROWM");

                entity.HasIndex(e => e.DocumentDocumentId)
                    .HasName("IX_Document_DocumentId");

                entity.HasIndex(e => e.ParcelParcelId)
                    .HasName("IX_Parcel_ParcelId");

                entity.Property(e => e.DocumentDocumentId).HasColumnName("Document_DocumentId");

                entity.Property(e => e.ParcelParcelId).HasColumnName("Parcel_ParcelId");

                entity.HasOne(d => d.DocumentDocument)
                    .WithMany(p => p.ParcelDocuments)
                    .HasForeignKey(d => d.DocumentDocumentId)
                    .HasConstraintName("FK_ROWM.ParcelDocuments_ROWM.Document_Document_DocumentId");

                entity.HasOne(d => d.ParcelParcel)
                    .WithMany(p => p.ParcelDocuments)
                    .HasForeignKey(d => d.ParcelParcelId)
                    .HasConstraintName("FK_ROWM.ParcelDocuments_ROWM.Parcel_Parcel_ParcelId");
            });

            modelBuilder.Entity<ParcelStatus>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("Parcel_Status", "ROWM");

                entity.Property(e => e.Code)
                    .HasMaxLength(40)
                    .ValueGeneratedNever();

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.ParentStatusCode).HasMaxLength(40);

                entity.HasOne(d => d.ParentStatusCodeNavigation)
                    .WithMany(p => p.InverseParentStatusCodeNavigation)
                    .HasForeignKey(d => d.ParentStatusCode)
                    .HasConstraintName("FK_Parcel_Status_Parent_Milestone");
            });

            modelBuilder.Entity<PreferredContactMode>(entity =>
            {
                entity.HasKey(e => e.Mode);

                entity.ToTable("Preferred_Contact_Mode", "ROWM");

                entity.Property(e => e.Mode)
                    .HasMaxLength(10)
                    .ValueGeneratedNever();
            });

            modelBuilder.Entity<RepresentationType>(entity =>
            {
                entity.HasKey(e => e.RelationTypeCode);

                entity.ToTable("Representation_Type", "ROWM");

                entity.Property(e => e.RelationTypeCode)
                    .HasMaxLength(20)
                    .ValueGeneratedNever();

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<RoeConditions>(entity =>
            {
                entity.HasKey(e => e.ConditionId);

                entity.ToTable("RoeConditions", "ROWM");

                entity.HasIndex(e => e.ParcelParcelId)
                    .HasName("IX_Parcel_ParcelId");

                entity.Property(e => e.ConditionId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Condition).IsRequired();

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ParcelParcelId).HasColumnName("Parcel_ParcelId");

                entity.HasOne(d => d.ContactNavigation)
                    .WithMany(p => p.RoeConditions)
                    .HasForeignKey(d => d.Contact)
                    .HasConstraintName("FK_CONDITIONS_CONTACTINFO");

                entity.HasOne(d => d.ParcelParcel)
                    .WithMany(p => p.RoeConditions)
                    .HasForeignKey(d => d.ParcelParcelId)
                    .HasConstraintName("FK_dbo.RoeConditions_ROWM.Parcel_Parcel_ParcelId");
            });

            modelBuilder.Entity<RoeStatus>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.ToTable("Roe_Status", "ROWM");

                entity.Property(e => e.Code)
                    .HasMaxLength(40)
                    .ValueGeneratedNever();

                entity.Property(e => e.Description).HasMaxLength(200);
            });

            modelBuilder.Entity<StatusActivity>(entity =>
            {
                entity.HasKey(e => e.ActivityId);

                entity.ToTable("StatusActivity", "ROWM");

                entity.HasIndex(e => e.AgentId)
                    .HasName("IX_AgentId");

                entity.HasIndex(e => e.ParentParcelId)
                    .HasName("IX_ParentParcelId");

                entity.Property(e => e.ActivityId).HasDefaultValueSql("(newid())");

                entity.Property(e => e.OrigianlParcelStatusCode).HasMaxLength(40);

                entity.Property(e => e.OriginalRoeStatusCode).HasMaxLength(40);

                entity.Property(e => e.ParcelStatusCode).HasMaxLength(40);

                entity.Property(e => e.RoeStatusCode).HasMaxLength(40);

                entity.HasOne(d => d.Agent)
                    .WithMany(p => p.StatusActivity)
                    .HasForeignKey(d => d.AgentId)
                    .HasConstraintName("FK_ROWM.StatusActivity_ROWM.Agent_AgentId");

                entity.HasOne(d => d.ParentParcel)
                    .WithMany(p => p.StatusActivity)
                    .HasForeignKey(d => d.ParentParcelId)
                    .HasConstraintName("FK_ROWM.StatusActivity_ROWM.Parcel_ParentParcelId");
            });
        }
    }
}
