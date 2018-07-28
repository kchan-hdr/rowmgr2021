using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public class ParcelGraph_Dto
    {
        public string ParcelId { get; set; }
        public string ParcelStatusCode { get; set; }
        public string ParcelStatus => this.ParcelStatusCode;        // to be removed
        public string RoeStatusCode { get; set; }
        public string SitusAddress { get; set; }
        public double Acreage { get; set; }

        public Compensation InitialROEOffer { get; set; }
        public Compensation FinalROEOffer { get; set; }
        public Compensation InitialOptionOffer { get; set; }
        public Compensation FinalOptionOffer { get; set; }
        public Compensation InitialEasementOffer { get; set; }
        public Compensation FinalEasementOffer { get; set; }


        public IEnumerable<OwnerDto> Owners { get; set; }
        public IEnumerable<ContactLogDto> ContactsLog { get; set; }
        public IEnumerable<DocumentHeader> Documents { get; set; }

        public class OwnerDto
        {
            public Guid OwnerId { get; set; }
            public string PartyName { get; set; }
            public IEnumerable<ParcelHeaderDto> OwnedParcel { get; set; }
            public IEnumerable<ContactInfoDto> Contacts { get; set; }
            public IEnumerable<ContactLogDto> ContactLogs { get; set; }
            public IEnumerable<DocumentHeader> Documents { get; set; }
        }

        public class ParcelHeaderDto
        {
            public string ParcelId { get; set; }
            public string SitusAddress { get; set; }
            public bool IsPrimaryOwner { get; set; }

            public Compensation InitialROEOffer { get; set; }
            public Compensation FinalROEOffer { get; set; }
            public Compensation InitialOptionOffer { get; set; }
            public Compensation FinalOptionOffer { get; set; }
            public Compensation InitialEasementOffer { get; set; }
            public Compensation FinalEasementOffer { get; set; }
        }

        public class ContactLogDto
        {
            public Guid ContactLogId { get; set; }
            public IEnumerable<string> ParcelIds { get; set; }
            public IEnumerable<ContactInfoDto> ContactIds { get; set; }
            public DateTimeOffset DateAdded { get; set; }
            public string ContactType { get; set; }
            public string AgentName { get; set; }
            public string Phase { get; set; }
            public string Title { get; set; }
            public string Notes { get; set; }
        }


        public class ContactInfoDto
        {
            public Guid ContactId { get; set; }
            public string ContactName { get; set; }
            public bool IsPrimary { get; set; }

            public string Relations { get; set; }

            public string OwnerFirstName { get; set; }
            public string OwnerLastName { get; set; }
            public string OwnerStreetAddress { get; set; }
            public string OwnerCity { get; set; }
            public string OwnerState { get; set; }
            public string OwnerZIP { get; set; }
            public string OwnerEmail { get; set; }
            public string OwnerHomePhone { get; set; }
            public string OwnerCellPhone { get; set; }
            public string OwnerWorkPhone { get; set; }
        }

        public class DocumentHeader
        {
            public string DocumentType { get; set; }
            public string DocumentTitle { get; set; }
            public string AgentName { get; set; }
            public Guid DocumentId { get; set; }
        }
    }
}
