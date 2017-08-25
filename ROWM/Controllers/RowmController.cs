using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ROWM.Dal;
using System.Diagnostics;
using System.IO;

namespace ROWM.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class RowmController : Controller
    {
        static readonly string _APP_NAME = "ROWM";

        #region ctor
        OwnerRepository _repo;

        public RowmController(OwnerRepository r)
        {
            _repo = r;
        }
        #endregion
        #region owner
        [Route("owners/{id:Guid}"), HttpGet]
        public async Task<OwnerDto> GetOwner(Guid id)
        {
            return new OwnerDto(await _repo.GetOwner(id));
        }

        [Route("owners"), HttpGet]
        public async Task<IEnumerable<OwnerDto>> FindOwner(string name)
        {
            return (await _repo.FindOwner(name))
                .Select(ox => new OwnerDto(ox));
        }
        #region contacts
        [Route("owners/{id:Guid}/contacts"), HttpPost]
        public async Task<IActionResult> AddContact(Guid id, [FromBody]ContactRequest info)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dt = DateTimeOffset.Now;

            var o = await _repo.GetOwner(id);
            o.Contacts.Add(new ContactInfo
            {
                OwnerFirstName = info.OwnerFirstName,
                OwnerLastName = info.OwnerLastName,

                OwnerStreetAddress = info.OwnerStreetAddress,
                OwnerCity = info.OwnerCity,
                OwnerState = info.OwnerState,
                OwnerZIP = info.OwnerZIP,

                OwnerEmail = info.OwnerEmail,
                OwnerCellPhone = info.OwnerCellPhone,
                OwnerWorkPhone = info.OwnerWorkPhone,
                OwnerHomePhone = info.OwnerHomePhone,

                IsPrimaryContact = info.IsPrimaryContact,
                Representation = info.Relations,

                Created = dt,
                LastModified = dt,
                ModifiedBy = _APP_NAME
            });

            return Json(new OwnerDto(await _repo.UpdateOwner(o)));
        }

        [Route("owners/{id:Guid}/contacts/{cinfo}"), HttpPut]
        public async Task<IActionResult> UpdateContact(Guid id, Guid cinfo, [FromBody]ContactRequest info)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var dt = DateTimeOffset.Now;

            var o = await _repo.GetOwner(id);
            var c = o.Contacts.Single(cx => cx.ContactId == cinfo);

            c.OwnerFirstName = info.OwnerFirstName;
            c.OwnerLastName = info.OwnerLastName;

            c.OwnerStreetAddress = info.OwnerStreetAddress;
            c.OwnerCity = info.OwnerCity;
            c.OwnerState = info.OwnerState;
            c.OwnerZIP = info.OwnerZIP;

            c.OwnerEmail = info.OwnerEmail;
            c.OwnerCellPhone = info.OwnerCellPhone;
            c.OwnerWorkPhone = info.OwnerWorkPhone;
            c.OwnerHomePhone = info.OwnerHomePhone;

            c.IsPrimaryContact = info.IsPrimaryContact;
            c.Representation = info.Relations;

            c.LastModified = dt;
            c.ModifiedBy = _APP_NAME;

            return Json(new ContactInfoDto(await _repo.UpdateContact(c)));
        }
        #endregion
        #endregion
        #region parcel
        [Route("parcels"), HttpGet]
        public IEnumerable<string> GetAllParcels()
        {
            return _repo.GetParcels();
        }

        [Route("parcels/{pid}"), HttpGet]
        public async Task<ParcelGraph> GetParcel(string pid)
        {
            return new ParcelGraph( await _repo.GetParcel(pid));
        }
        #region offer
        [Route("parcels/{pid}/initialOffer"), HttpPut]
        [ProducesResponseType(typeof(ParcelGraph), 202)]
        public async Task<IActionResult> UpdateInitialOffer(string pid, [FromBody]OfferRequest offer)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var p = await _repo.GetParcel(pid);

            p.InitialOffer = offer.OfferDate;
            p.InitialOfferAmount = offer.Amount;
            p.InitialOfferNotes = offer.Notes;

            p.LastModified = DateTimeOffset.Now;
            p.ModifiedBy = _APP_NAME;

            return Ok(new ParcelGraph(await _repo.UpdateParcel(p)));
        }

        [Route("parcels/{pid}/finalOffer"), HttpPut]
        [ProducesResponseType(typeof(ParcelGraph), 202)]
        public async Task<IActionResult> UpdateFinalOffer(string pid, [FromBody]OfferRequest offer)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var p = await _repo.GetParcel(pid);

            p.FinalOffer = offer.OfferDate;
            p.FinalOfferAmount = offer.Amount;
            p.FinalOfferNotes = offer.Notes;

            p.LastModified = DateTimeOffset.Now;
            p.ModifiedBy = _APP_NAME;

            return Json(new ParcelGraph(await _repo.UpdateParcel(p)));
        }
        #endregion
        #endregion
        #region logs
        [Route("parcels/{pid}/logs"), HttpPost]
        public async Task<IActionResult> AddContactLog(string pid, [FromBody] LogRequest logRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(this.ModelState);

            var dt = DateTimeOffset.Now;

            var p = await _repo.GetParcel(pid);
            var a = await _repo.GetAgent(logRequest.AgentName);

            var l = new ContactLog
            {
                ContactAgent = a,
                ContactChannel = logRequest.Channel,
                ProjectPhase = logRequest.Phase,
                DateAdded = logRequest.DateAdded,
                Title = logRequest.Title,
                Notes = logRequest.Notes,
                Created = dt,
                LastModified = dt,
                ModifiedBy = _APP_NAME,
                Parcels = new List<Parcel> { p },
                Contacts = new List<ContactInfo>()
            };

            var log = await _repo.AddContactLog(logRequest.ParcelIds, logRequest.ContactIds, l);
            return Json(new ContactLogDto(log));
        }
        [Route("parcels/{pid}/logs/{lid}"), HttpPost]
        public async Task<IActionResult> UpdateContactLog(string pid, Guid lid, [FromBody] LogRequest logRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(this.ModelState);

            var dt = DateTimeOffset.Now;

            var p = await _repo.GetParcel(pid);
            var a = await _repo.GetAgent(logRequest.AgentName);
            var l = p.ContactsLog.Single(cx => cx.ContactLogId == lid);

            l.ContactAgent = a;
            l.ContactChannel = logRequest.Channel;
            l.ProjectPhase = logRequest.Phase;
            l.DateAdded = logRequest.DateAdded;
            l.Title = logRequest.Title;
            l.Notes = logRequest.Notes;
            //l.Created = dt;
            l.LastModified = dt;
            l.ModifiedBy = _APP_NAME;
            //l.Parcels = new List<Parcel> { p };
            //l.Contacts = new List<ContactInfo>();

            var log = await _repo.UpdateContactLog(l);
            return Json(new ContactLogDto(log));
        }
        #endregion
        #region agents
        [Route("agents"), HttpGet]
        public async Task<IEnumerable<AgentDto>> GetAgents()
        {
            var a = await _repo.GetAgents();
            return a.Select( ax => new AgentDto(ax));
        }
        #endregion
        #region statistics
        [HttpGet("statistics")]
        public async Task<StatisticsDto> GetStatistics()
        {
            var s = await _repo.Snapshot();
            return new StatisticsDto
            {
                NumberOfOwners = s.nOwners,
                NumberOfParcels = s.nParcels
            };
        }
        #endregion
    }

    #region request dto
    public class LogRequest
    {
        public Guid AgentId { get; set; }
        public string AgentName { get; set; }
        public string Phase { get; set; }
        public List<string> ParcelIds { get; set; }
        public List<Guid> ContactIds { get; set; }
        public string Channel { get; set; }
        public DateTimeOffset DateAdded { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }

    }

    public class ContactRequest
    {
        public string OwnerFirstName { get; set; } = "";
        public string OwnerLastName { get; set; } = "";

        public string OwnerStreetAddress { get; set; } = "";
        public string OwnerCity { get; set; } = "";
        public string OwnerState { get; set; } = "";
        public string OwnerZIP { get; set; } = "";

        public string OwnerEmail { get; set; } = "";
        public string OwnerCellPhone { get; set; } = "";
        public string OwnerWorkPhone { get; set; } = "";
        public string OwnerHomePhone { get; set; } = "";

        public bool IsPrimaryContact { get; set; } = false;
        public string Relations { get; set; } = "";
    }
    #endregion
    #region offer dto
    public class OfferRequest
    {
        public DateTimeOffset OfferDate { get; set; }
        public double Amount { get; set; }
        public string Notes { get; set; }
    }
    #endregion
    #region dto
    public class StatisticsDto
    {
        public int NumberOfParcels { get; set; }
        public int NumberOfOwners { get; set; }
    }
    public class AgentDto
    {
        public Guid AgentId { get; set; }
        public string AgentName { get; set; }
        public IEnumerable<ContactLogDto> ContactsLog { get; set; }
        public IEnumerable<DocumentHeader> Documents { get; set; }

        internal AgentDto(Agent a)
        {
            AgentId = a.AgentId;
            AgentName = a.AgentName;
            ContactsLog = a.Logs.Select(cx => new ContactLogDto(cx));
            Documents = a.Documents.Select(dx => new DocumentHeader(dx));
        }
    }
    public class ContactLogDto
    {
        public IEnumerable<string> ParcelIds { get; set; }
        public IEnumerable<ContactInfoDto> ContactIds { get; set; }
        public DateTimeOffset DateAdded { get; set; }
        public string ContactType { get; set; }
        public string AgentName { get; set; }
        public string Phase { get; set; }
        public string Title { get; set; }
        public string Notes { get; set; }

        internal ContactLogDto(ContactLog log)
        {
            AgentName = log.ContactAgent?.AgentName ?? "";
            DateAdded = log.DateAdded;
            ContactType = log.ContactChannel;

            ParcelIds = log.Parcels.Select(px => px.ParcelId);
            ContactIds = log.Contacts.Select(cx => new ContactInfoDto(cx));

            Phase = log.ProjectPhase;
            Title = log.Title;
            Notes = log.Notes;
        }
    }

    public class OwnerHeader
    {
        public Guid OwnerId { get; set; }
        public string PartyName { get; set; }
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

        internal ContactInfoDto(ContactInfo c)
        {
            ContactId = c.ContactId;
            ContactName = $"{c.OwnerFirstName ?? ""} {c.OwnerLastName ?? ""}";
            IsPrimary = c.IsPrimaryContact;
            Relations = c.Representation;

            OwnerFirstName = c.OwnerFirstName;
            OwnerLastName = c.OwnerLastName;
            OwnerStreetAddress = c.OwnerStreetAddress;
            OwnerCity = c.OwnerCity;
            OwnerState = c.OwnerState;
            OwnerZIP = c.OwnerZIP;
            OwnerEmail = c.OwnerEmail;
            OwnerCellPhone = c.OwnerCellPhone;
            OwnerWorkPhone = c.OwnerWorkPhone;
            OwnerHomePhone = c.OwnerHomePhone;
        }
    }

    public class OwnerDto
    {
        public Guid OwnerId { get; set; }
        public string PartyName { get; set; }
        public IEnumerable<ParcelHeaderDto> OwnedParcel { get; set; }
        public IEnumerable<ContactInfoDto> Contacts { get; set; }
        public IEnumerable<ContactLogDto> ContactLogs { get; set; }
        public IEnumerable<DocumentHeader> Documents { get; set; }

        public OwnerDto( Owner o)
        {
            OwnerId = o.OwnerId;
            PartyName = o.PartyName;
            OwnedParcel = o.OwnParcel.Select(ox=> new ParcelHeaderDto(ox));
            Contacts = o.Contacts.Select(cx => new ContactInfoDto(cx));
            ContactLogs = o.Contacts
                .Where( cx => cx.ContactsLog != null )
                .SelectMany( cx => cx.ContactsLog.Select( cxl => new ContactLogDto(cxl ))); //  o.ContactLogs.Select(cx => new ContactLogDto(cx));
            Documents = o.Documents.Select(dx => new DocumentHeader(dx));
        }
    }

    public class ParcelHeaderDto
    {
        public string ParcelId { get; set; }
        public string SitusAddress { get; set; }
        public bool IsPrimaryOwner { get; set; }

        internal ParcelHeaderDto(Ownership o)
        {
            ParcelId = o.ParcelId;
            SitusAddress = o.Parcel.SitusAddress;
            IsPrimaryOwner = o.Ownership_t == Ownership.OwnershipType.Primary;
        }
    }
    #endregion
    #region parcel graph
    public class ParcelGraph
    {
        public string ParcelId { get; set; }
        public string ParcelStatus { get; set; }
        public string SitusAddress { get; set; }
        public double Acreage { get; set; }

        public DateTimeOffset FinalOffer { get; set; }
        public double FinalOfferAmount { get; set; }
        public string FinalOfferNotes { get; set; }

        public DateTimeOffset InitialOffer { get; set; }
        public double InitialOfferAmount { get; set; }
        public string InitialOfferNotes { get; set; }

        public IEnumerable<OwnerDto> Owners { get; set; }
        public IEnumerable<ContactLogDto> ContactsLog { get; set; }
        public IEnumerable<DocumentHeader> Documents { get; set; }

        internal ParcelGraph( Parcel p)
        {
            ParcelId = p.ParcelId;
            ParcelStatus = Enum.GetName(typeof(Parcel.RowStatus), p.ParcelStatus);
            SitusAddress = p.SitusAddress;
            
            Acreage = p.Acreage;
            FinalOffer = p.FinalOffer;
            FinalOfferAmount = p.FinalOfferAmount;
            FinalOfferNotes = p.FinalOfferNotes;
            InitialOffer = p.InitialOffer;
            InitialOfferAmount = p.InitialOfferAmount;
            InitialOfferNotes = p.InitialOfferNotes;

            Owners = p.Owners.Select( ox => new OwnerDto(ox.Owner));
            ContactsLog =  p.ContactsLog.Select( cx => new ContactLogDto(cx));
            Documents = p.Documents.Select(dx => new DocumentHeader(dx));
        }
    }
    #endregion
}