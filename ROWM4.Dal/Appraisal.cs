using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class Appraisal
    {
        public Guid AppraisalId { get; set; }
        public Guid ParcelId { get; set; }
        public string Appraiser { get; set; }
        public string AppraisalFirm { get; set; }
        public DateTime AppraisalDate { get; set; }
        public DateTime? ReportDate { get; set; }
        public Guid? ReportId { get; set; }
        public decimal? AppraisedValue { get; set; }
        public string AppraisedValueType { get; set; }
        public double? AppraisedAcrage { get; set; }
        public string AppraisalConditions { get; set; }
        public Guid? AgentId { get; set; }
        public string Reviewer { get; set; }
        public DateTime? ReviewerApprovalDate { get; set; }

        public Agent Agent { get; set; }
        public Parcel Parcel { get; set; }
        public DocumentPackage Report { get; set; }
    }
}
