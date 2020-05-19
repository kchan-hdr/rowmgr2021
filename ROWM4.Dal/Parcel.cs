using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class Parcel
    {
        public Parcel()
        {
            Appraisal = new HashSet<Appraisal>();
            InverseParcelParcel = new HashSet<Parcel>();
            Ownership = new HashSet<Ownership>();
            ParcelContactInfo = new HashSet<ParcelContactInfo>();
            ParcelContactLogs = new HashSet<ParcelContactLogs>();
            ParcelDocuments = new HashSet<ParcelDocuments>();
            RoeConditions = new HashSet<RoeConditions>();
            StatusActivity = new HashSet<StatusActivity>();
        }

        public Guid ParcelId { get; set; }
        public string AssessorParcelNumber { get; set; }
        public string CountyFips { get; set; }
        public string CountyName { get; set; }
        public string SitusAddress { get; set; }
        public double? Acreage { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public string ModifiedBy { get; set; }
        public DateTimeOffset? InitialRoeofferOfferDate { get; set; }
        public double? InitialRoeofferOfferAmount { get; set; }
        public string InitialRoeofferOfferNotes { get; set; }
        public DateTimeOffset? FinalRoeofferOfferDate { get; set; }
        public double? FinalRoeofferOfferAmount { get; set; }
        public string FinalRoeofferOfferNotes { get; set; }
        public DateTimeOffset? InitialOptionOfferOfferDate { get; set; }
        public double? InitialOptionOfferOfferAmount { get; set; }
        public string InitialOptionOfferOfferNotes { get; set; }
        public DateTimeOffset? FinalOptionOfferOfferDate { get; set; }
        public double? FinalOptionOfferOfferAmount { get; set; }
        public string FinalOptionOfferOfferNotes { get; set; }
        public DateTimeOffset? InitialEasementOfferOfferDate { get; set; }
        public double? InitialEasementOfferOfferAmount { get; set; }
        public string InitialEasementOfferOfferNotes { get; set; }
        public DateTimeOffset? FinalEasementOfferOfferDate { get; set; }
        public double? FinalEasementOfferOfferAmount { get; set; }
        public string FinalEasementOfferOfferNotes { get; set; }
        public string ParcelStatusCode { get; set; }
        public string RoeStatusCode { get; set; }
        public bool IsActive { get; set; }
        public int? LandownerScore { get; set; }
        public string TrackingNumber { get; set; }
        public Guid? ParcelParcelId { get; set; }
        public bool IsDeleted { get; set; }

        public Parcel ParcelParcel { get; set; }
        public ParcelStatus ParcelStatusCodeNavigation { get; set; }
        public RoeStatus RoeStatusCodeNavigation { get; set; }
        public ICollection<Appraisal> Appraisal { get; set; }
        public ICollection<Parcel> InverseParcelParcel { get; set; }
        public ICollection<Ownership> Ownership { get; set; }
        public ICollection<ParcelContactInfo> ParcelContactInfo { get; set; }
        public ICollection<ParcelContactLogs> ParcelContactLogs { get; set; }
        public ICollection<ParcelDocuments> ParcelDocuments { get; set; }
        public ICollection<RoeConditions> RoeConditions { get; set; }
        public ICollection<StatusActivity> StatusActivity { get; set; }
    }
}
