using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;

namespace ROWM.Dal
{
    public class OwnerRepository
    {
        #region ctor
        private readonly ROWM_Context _ctx;

        public OwnerRepository(ROWM_Context c = null)
        {
            if (c == null)
                _ctx = new ROWM_Context();
            else
                _ctx = c;
        }
        #endregion

        #region statistics
        public async Task<(int nParcels, int nOwners)> Snapshot()
        {
            var np = await _ctx.Parcels.CountAsync();
            var no = await _ctx.Owners.CountAsync();

            return (np, no);
        }

        public async Task<IEnumerable<SubTotal>> SnapshotParcelStatus()
        {
            var q = await (from p in _ctx.Parcels
                     group p by p.ParcelStatus into psg
                     select new { k = psg.Key, c = psg.Count() }).ToArrayAsync();

            var list = new List<SubTotal>();
            foreach (var psg in q)
            {
                list.Add(new SubTotal { Title = psg.k.Code, Count = psg.c });
            }

            return list;
        }

        public async Task<IEnumerable<SubTotal>> SnapshotRoeStatus()
        {
            return await (from p in _ctx.Parcels
             group p by p.RoeStatusCode into psg
             select new SubTotal { Title = psg.Key, Count = psg.Count() }).ToArrayAsync();
        }

        #region dto
        public class SubTotal
        {
            public string Title { get; set; }
            public int Count { get; set; }
        }
        #endregion
        #endregion
        public async Task<Owner> GetOwner(Guid uid)
        {
            return await _ctx.Owners
                .Include(ox => ox.OwnParcel)
                //.Include(ox => ox.ContactLogs)
                //.Include(ox => ox.ContactLogs.Select(ocx => ocx.ContactAgent))
                .Include(ox => ox.Contacts)
                .Include(ox => ox.Contacts.Select( ocx => ocx.ContactsLog))
                .Include(ox => ox.Documents)
                .FirstOrDefaultAsync(ox => ox.OwnerId == uid);
        }

        public async Task<IEnumerable<Owner>> FindOwner(string name)
        {
            return await _ctx.Owners
                .Include(ox => ox.Contacts)
                .Include(ox => ox.Contacts.Select(ocx => ocx.ContactsLog))
                .Include(ox => ox.Documents)
                .Where(ox => ox.PartyName.Contains(name)).ToArrayAsync();
        }

        public async Task<Parcel> GetParcel(string pid)
        {
            return await _ctx.Parcels
                .Include(px => px.Owners)
                .Include(px => px.Owners.Select( ox => ox.Owner.ContactLogs))
                .Include(px => px.ContactsLog)
                .Include(ox => ox.Documents)
                .FirstOrDefaultAsync(px => px.ParcelId == pid);
        }

        public IEnumerable<string> GetParcels() => _ctx.Parcels.AsNoTracking().Select(px => px.ParcelId);

        public async Task<Parcel> UpdateParcel (Parcel p)
        {
            if (_ctx.Entry<Parcel>(p).State == EntityState.Detached)
                _ctx.Entry<Parcel>(p).State = EntityState.Modified;

            if (await WriteDb() <= 0)
                throw new ApplicationException("update parcel failed");

            return p;
        }

        public async Task<Owner> AddOwner(string name, string first = "", string last = "", string address = "", string city = "", string state = "", string z = "", string email = "", string hfone = "", string wfone = "", string cfone = "",   bool primary = true )
        {
            var dt = DateTimeOffset.Now;

            var o = _ctx.Owners.Create();
            o.Created = dt;
            o.PartyName = name;

            var c = _ctx.Contacts.Create();
            c.Created = dt;
            c.IsPrimaryContact = primary;
            c.OwnerFirstName = first;
            c.OwnerLastName = last;
            c.OwnerStreetAddress = address;
            c.OwnerCity = city;
            c.OwnerState = state;
            c.OwnerZIP = z;
            c.OwnerEmail = email;
            c.OwnerHomePhone = hfone;
            c.OwnerCellPhone = cfone;
            c.OwnerWorkPhone = wfone;
            
            o.Contacts = new List<ContactInfo>();
            o.Contacts.Add(c);

            _ctx.Owners.Add(o);

            if (await WriteDb() <= 0)
                throw new ApplicationException("Add owner failed");

            return o;
        }

        public async Task<Owner> UpdateOwner(Owner o)
        {
            if (_ctx.Entry<Owner>(o).State == EntityState.Detached)
                _ctx.Entry<Owner>(o).State = EntityState.Modified;

            if (await WriteDb() <= 0)
                throw new ApplicationException("Update owner failed");

            return o;
        }

        public async Task<ContactInfo> UpdateContact(ContactInfo c)
        {
            if (_ctx.Entry<ContactInfo>(c).State == EntityState.Detached)
                _ctx.Entry<ContactInfo>(c).State = EntityState.Modified;

            if (await WriteDb() <= 0)
                throw new ApplicationException("Add owner failed");

            return c;
        }

        public async Task<ContactLog> AddContactLog(IEnumerable<string> pids, IEnumerable<Guid> cids, ContactLog log)
        {
            var dt = DateTimeOffset.Now;

            _ctx.ContactLogs.Add(log);

            if (pids != null && pids.Count() > 0)
            {
                foreach (var pid in pids)
                {
                    var px = await _ctx.Parcels.SingleOrDefaultAsync(pxid => pxid.ParcelId.Equals(pid));
                    if (px == null)
                        Trace.TraceWarning($"invalid parcel {pid}");
                    log.Parcels.Add(px);
                }
            }

            if (cids != null && cids.Count() > 0)
            {
                foreach (var cid in cids)
                {
                    var cx = await _ctx.Contacts.SingleOrDefaultAsync(oxid => oxid.ContactId.Equals(cid));
                    if (cx == null)
                        Trace.TraceWarning($"invalid contact {cid}");
                    log.Contacts.Add(cx);
                }
            }

            if (await WriteDb() <= 0)
                throw new ApplicationException("Add log failed");

            return log;
        }

        public async Task<ContactLog> UpdateContactLog(IEnumerable<string> pids, IEnumerable<Guid> cids, ContactLog log)
        {
            if (_ctx.Entry<ContactLog>(log).State == EntityState.Detached)
            {
                _ctx.Entry<ContactLog>(log).State = EntityState.Modified;
            }

            var existingPids = log.Parcels.Select(p => p.ParcelId).ToList();
            var existingCids = log.Contacts.Select(c => c.ContactId).ToList();

            // Find Deleted & added parcels & contacts
            var deletedPids = existingPids.Except(pids);
            var newPids = pids.Except(existingPids);
            var deletedCids = existingCids.Except(cids);
            var newCids = cids.Except(existingCids);

            // Remove deleted parcels & contacts
            if (deletedPids != null && deletedPids.Count() > 0)
            {
                foreach (var pid in deletedPids)
                {
                    var px = await _ctx.Parcels.SingleOrDefaultAsync(pxid => pxid.ParcelId.Equals(pid));
                    if (px == null)
                        Trace.TraceWarning($"invalid parcel {pid}");
                    log.Parcels.Remove(px);
                }
            }

            if (deletedCids != null && deletedCids.Count() > 0)
            {
                foreach (var cid in deletedCids)
                {
                    var cx = await _ctx.Contacts.SingleOrDefaultAsync(oxid => oxid.ContactId.Equals(cid));
                    if (cx == null)
                        Trace.TraceWarning($"invalid contact {cid}");
                    log.Contacts.Remove(cx);
                }
            }

            // Add new parcels & contacts
            if (newPids != null && newPids.Count() > 0)
            {
                foreach (var pid in newPids)
                {
                    var px = await _ctx.Parcels.SingleOrDefaultAsync(pxid => pxid.ParcelId.Equals(pid));
                    if (px == null)
                        Trace.TraceWarning($"invalid parcel {pid}");
                    log.Parcels.Add(px);
                }
            }

            if (newCids != null && newCids.Count() > 0)
            {
                foreach (var cid in newCids)
                {
                    var cx = await _ctx.Contacts.SingleOrDefaultAsync(oxid => oxid.ContactId.Equals(cid));
                    if (cx == null)
                        Trace.TraceWarning($"invalid contact {cid}");
                    log.Contacts.Add(cx);
                }
            }


            if (await WriteDb() <= 0)
                throw new ApplicationException("update contact log failed");

            return log;
        }

        [Obsolete("use add contactlog")]
        public async Task<Parcel> RecordContact(Parcel p, Agent a, string notes, DateTimeOffset date, string phase)
        {
            var dt = DateTimeOffset.Now;

            var log = _ctx.ContactLogs.Create();
            log.Created = dt;
            log.ContactAgent = a;
            log.Notes = notes;
            log.DateAdded = date;
            log.ProjectPhase = phase;

            p.ContactsLog.Add(log);

            _ctx.ContactLogs.Add(log);

            if (await WriteDb() <= 0)
                throw new ApplicationException("Record Contact failed");

            return p;
        }

        [Obsolete("use add contactlog")]
        public async Task<Owner> RecordOwnerContact(Owner o, Agent a, string notes, DateTimeOffset date, string phase)
        {
            var dt = DateTimeOffset.Now;

            var log = _ctx.ContactLogs.Create();
            log.Created = dt;
            log.ContactAgent = a;
            log.Notes = notes;
            log.DateAdded = date;
            log.ProjectPhase = phase;

            o.ContactLogs.Add(log);

            _ctx.ContactLogs.Add(log);

            if (await WriteDb() <= 0)
                throw new ApplicationException("Record Contact failed");

            return o;
        }
        #region documents
        public Document GetDocument(Guid id) => _ctx.Documents.Find(id);

        public async Task<Document> UpdateDocument(Document d)
        {
            if (_ctx.Entry(d).State == System.Data.Entity.EntityState.Deleted)
                _ctx.Entry(d).State = System.Data.Entity.EntityState.Modified;

            var a = _ctx.DocumentActivities.Create();
            a.ParentDocument = d;
            a.ActivityDate = DateTimeOffset.Now;
            a.Activity = "Updated Tracking";
            a.Agent = d.Agents.FirstOrDefault();        // TODO:

            _ctx.DocumentActivities.Add(a);

            if (await WriteDb() <= 0)
                throw new ApplicationException("document meta-data edit failed");

            return d;
        }

        public async Task<Document> Store(string title, string document_t, string content_t, string fname, Guid agentId, byte[] content)
        {
            var d = _ctx.Documents.Create();
            d.Content = content;
            d.Title = title;
            d.DocumentType = document_t;
            d.SourceFilename = fname;
            d.ContentType = content_t;
            d.Created = DateTimeOffset.Now;

            var a = _ctx.DocumentActivities.Create();
            a.ParentDocument = d;
            a.ActivityDate = DateTimeOffset.Now;
            a.Activity = "Uploaded";
            a.AgentId = agentId;

            _ctx.Documents.Add(d);
            _ctx.DocumentActivities.Add(a);

            if (await WriteDb() <= 0)
                throw new ApplicationException("document upload failed");

            return d;
        }
        #endregion
        #region row agents
        public async Task<Agent> GetAgent(string name) => await _ctx.Agents.FirstOrDefaultAsync(ax => ax.AgentName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        public async Task<IEnumerable<Agent>> GetAgents() =>
            await _ctx.Agents.AsNoTracking()
                .Include(ax => ax.Logs)
                .Include(ax => ax.Documents)
                .ToArrayAsync();
         
        #endregion
        #region helpers
        internal async Task<int> WriteDb()
        {
            if ( _ctx.ChangeTracker.HasChanges())
            {
                try
                {
                    return await _ctx.SaveChangesAsync();
                }
                catch ( Exception e )
                {
                    Trace.TraceError(e.Message);
                    throw;
                }
            }
            else
            {
                return 0;
            }
        }
        #endregion
    }
}
