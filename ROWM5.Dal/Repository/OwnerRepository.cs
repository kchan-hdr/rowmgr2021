﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

#nullable enable

namespace ROWM.Dal
{
    public class OwnerRepository
    {
        #region ctor
        private readonly ROWM_Context _ctx;

        public OwnerRepository(ROWM_Context c) => _ctx = c;
        #endregion

        IQueryable<Parcel> ActiveParcels() => _ctx.Parcels.Where(px => px.IsActive);
        IQueryable<Owner> ActiveOwners() => _ctx.Owners.Where(ox => !ox.IsDeleted);
        IQueryable<ContactInfo> ActiveContacts() => _ctx.ContactInfos.Where(cx => !cx.IsDeleted);
        IQueryable<ContactLog> ActiveLogs() => _ctx.ContactLogs.Where(lx => !lx.IsDeleted);
        IQueryable<Document> ActiveDocuments() => _ctx.Documents.Where(dx => !dx.IsDeleted);


        public async Task<Owner> GetOwner(Guid uid)
        {
            return await ActiveOwners()
                .Include(ox => ox.Ownerships.Select(o => o.Parcel))
                .Include(ox => ox.ContactInfos)
                    .ThenInclude(ocx => ocx.ContactInfoContactLogs)
                .FirstOrDefaultAsync(ox => ox.OwnerId == uid);
        }

        public async Task<IEnumerable<Owner>> FindOwner(string name)
        {
            return await ActiveOwners()
                .Include(ox => ox.ContactInfos).ThenInclude(ocx => ocx.ContactInfoContactLogs)
                .Where(ox => ox.PartyName.Contains(name))
                .ToArrayAsync();
        }

        public async Task<Parcel> GetParcel(string pid)
        {
            var p = await ActiveParcels()
                .Include(px => px.Ownerships.Select( o=>o.Owner.ContactLogs))
                .Include(px => px.ParcelContactLogs)
                .Include(px => px.ActionItems)
                .FirstOrDefaultAsync(px => px.TrackingNumber == pid);

            return p;
        }
        public async Task<List<Document>> GetDocumentsForParcel(string pid)
        {
            var p = await ActiveParcels().FirstOrDefaultAsync(px => px.TrackingNumber.Equals(pid));
            if ( p == null)
            {
                throw new ArgumentOutOfRangeException($"cannot find parcel <{pid}>");
            }

            var query = p.ParcelDocuments
                .Select(pdx => pdx.DocumentDocument)
                .Select(dx => new Document
            {                
                DocumentId = dx.DocumentId,
                DocumentType = dx.DocumentType,
                Title = dx.Title,
                DateRecorded = dx.DateRecorded,
                Created = dx.Created,
                LastModified = dx.LastModified,
                IsDeleted = dx.IsDeleted
            });

            return query.ToList();

            //var q = _ctx.Database.SqlQuery<DocumentH>("SELECT d.DocumentId, d.DocumentType, d.title FROM rowm.ParcelDocuments pd INNER JOIN rowm.Document d on pd.document_documentid = d.documentid WHERE pd.parcel_parcelId = @pid and d.IsDeleted = 0", new System.Data.SqlClient.SqlParameter("@pid", p.ParcelId));
            //var ds = await q.ToListAsync();
            //return ds.Select(dx => new Document { Title = dx.Title, DocumentId = dx.DocumentId, DocumentType = dx.DocumentType }).ToList();
        }
        #region Db dto
        public class DocumentH
        {
            public Guid DocumentId { get; set; }
            public string DocumentType { get; set; }
            public string Title { get; set; }
        }
        #endregion

        [Obsolete]
        public async Task<IEnumerable<StatusActivity>> GetStatusForParcel(string pid) => await GetStatusForParcel(pid, false);

        public async Task<IEnumerable<StatusActivity>> GetStatusForParcel(string pid, bool all = false)
        {
            var p = await ActiveParcels()
                .Where(px => px.TrackingNumber.Equals(pid))
                .Select(px => px.ParcelId)
                .FirstOrDefaultAsync();

            if (p == Guid.Empty)
                throw new KeyNotFoundException($"cannot find parcel <{pid}>");

            var act = await _ctx.StatusActivities.Where(sx => sx.ParentParcelId == p).ToArrayAsync();

            if (all)
            {
                return act;
            } 
            else           
            {
                var q = from a in act
                        group a by a.StatusCode into ag
                        select ag.OrderByDescending(ax => ax.ActivityDate).Take(1);

                return q.SelectMany(qx => qx);
            }
        }
        public IEnumerable<string> GetParcels() => ActiveParcels().AsNoTracking().Select(px => px.AssessorParcelNumber);
        public IEnumerable<Parcel> GetParcels2() => ActiveParcels().Include(px => px.Ownerships.Select( o => o.Owner )).Include(px => px.RoeConditions).AsNoTracking();

        public async Task<Parcel> UpdateParcel (Parcel p)
        {
            if (_ctx.Entry<Parcel>(p).State == EntityState.Detached)
                _ctx.Entry<Parcel>(p).State = EntityState.Modified;

            await WriteDb();
            //if (await WriteDb() <= 0)
            //    throw new ApplicationException("update parcel failed");

            return p;
        }

        public async Task<Owner> AddOwner(string name, string first = "", string last = "", string address = "", string city = "", string state = "", string z = "", string email = "", string hfone = "", string wfone = "", string cfone = "",   bool primary = true )
        {
            var dt = DateTimeOffset.Now;

            var o = new Owner
            {
                Created = dt,
                PartyName = name,
                OwnerAddress = MakeAddress(address, city, state, z)
            };

            ///
            /// no longer automatically add a default contact
            /// 
            //var c = _ctx.ContactInfo.Create();
            //c.Created = dt;
            //c.IsPrimaryContact = primary;
            //c.FirstName = first;
            //c.LastName = last;
            //c.StreetAddress = address;
            //c.City = city;
            //c.State = state;
            //c.ZIP = z;
            //c.Email = email;
            //c.HomePhone = hfone;
            //c.CellPhone = cfone;
            //c.WorkPhone = wfone;
            
            //o.ContactInfo = new List<ContactInfo>();
            //o.ContactInfo.Add(c);

            _ctx.Owners.Add(o);

            if (await WriteDb() <= 0)
                throw new ApplicationException("Add owner failed");

            return o;
        }

        static string MakeAddress( string address, string city, string state, string zip)
        {
            char[] trimmer = { ',', ' ' };

            if (string.IsNullOrWhiteSpace(address) && string.IsNullOrWhiteSpace(city) && string.IsNullOrWhiteSpace(state) && string.IsNullOrWhiteSpace(zip))
                return string.Empty;

            return $"{address}, {city} {state} {zip}".Trim(trimmer);
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

        public IEnumerable<Ownership> GetContacts() => _ctx.Parcels.Where(p => p.IsActive).SelectMany(p => p.Ownerships);

        public IEnumerable<ContactLog> GetLogs() => ActiveLogs().Where(c => c.ParcelContactLogs.Any(p => p.ParcelParcel.IsActive));

        //public async Task<IEnumerable<DocHead>> GetDocs()
        //{
        //    try
        //    {
        //        // for performance
        //        var qstr = "SELECT d.DocumentId, d.Title, d.ContentType, d.ReceivedDate, d.SentDate, d.DeliveredDate, d.SignedDate, d.DateRecorded, d.ClientTrackingNumber, d.CheckNo, p.Assessor_Parcel_Number as 'Parcel_ParcelId' FROM rowm.ParcelDocuments pd INNER JOIN Rowm.Document d on pd.Document_DocumentId = d.DocumentId INNER JOIN Rowm.Parcel p ON pd.Parcel_ParcelId = p.ParcelId where p.IsActive = 1 and d.IsDeleted = 0";
        //        var q = _ctx.Database.SqlQuery<DocHead>(qstr);
                
        //        return await q.ToListAsync();
        //    }
        //    catch ( Exception e)
        //    {
        //        throw;
        //    }
        //}

        public class DocHead
        {
            public Guid DocumentId { get; set; }
            public string Title { get; set; }
            public string ContentType { get; set; }
            public DateTimeOffset? ReceivedDate { get; set; }
            public DateTimeOffset? SentDate { get; set; }
            public DateTimeOffset? DeliveredDate { get; set; }
            public DateTimeOffset? SignedDate { get; set; }
            public DateTimeOffset? DateRecorded { get; set; }
            public string ClientTrackingNumber { get; set; }
            public string CheckNo { get; set; }
            public string Parcel_ParcelId { get; set; }
        }

        public async Task<ContactLog> AddContactLog(IEnumerable<string> pids, IEnumerable<Guid> cids, ContactLog log)
        {
            var dt = DateTimeOffset.Now;

            _ctx.ContactLogs.Add(log);

            if (pids != null && pids.Count() > 0)
            {
                foreach (var pid in pids)
                {
                    var px = await _ctx.Parcels.SingleOrDefaultAsync(pxid => pxid.AssessorParcelNumber.Equals(pid) && pxid.IsActive );
                    if (px == null)
                        Trace.TraceWarning($"invalid parcel {pid}");

                    log.ParcelContactLogs.Add(new ParcelContactLog { ContactLogContactLog = log, ParcelParcel = px });
                }
            }

            if (cids != null && cids.Count() > 0)
            {
                foreach (var cid in cids)
                {
                    var cx = await _ctx.ContactInfos.SingleOrDefaultAsync(oxid => oxid.ContactId.Equals(cid));
                    if (cx == null)
                        Trace.TraceWarning($"invalid contact {cid}");

                    log.ContactInfoContactLogs.Add(new ContactInfoContactLog { ContactLogContactLog = log, ContactInfoContactId = cid });
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

            var existingPids = log.ParcelContactLogs.Select(px => px.ParcelParcel).Select(p => p.AssessorParcelNumber).ToList();
            var existingCids = log.ContactInfoContactLogs.Select(sx => sx.ContactInfoContact).Select(c => c.ContactId).ToList();

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
                    {
                        Trace.TraceWarning($"invalid parcel {pid}");
                        continue;
                    }

                    //log.Parcel.Remove(px);
                }
            }

            if (deletedCids != null && deletedCids.Count() > 0)
            {
                foreach (var cid in deletedCids)
                {
                    var cx = await _ctx.ContactInfos.SingleOrDefaultAsync(oxid => oxid.ContactId.Equals(cid));
                    if (cx == null)
                    {
                        Trace.TraceWarning($"invalid contact {cid}");
                        continue;
                    }

                    //log.ContactInfo.Remove(cx);
                }
            }

            // Add new parcels & contacts
            if (newPids != null && newPids.Count() > 0)
            {
                foreach (var pid in newPids)
                {
                    var px = await _ctx.Parcels.SingleOrDefaultAsync(pxid => pxid.ParcelId.Equals(pid));
                    if (px == null)
                    {
                        Trace.TraceWarning($"invalid parcel {pid}");
                        continue;
                    }
                    log.ParcelContactLogs.Add(new ParcelContactLog { ContactLogContactLog = log, ParcelParcel = px });
                }
            }

            if (newCids != null && newCids.Count() > 0)
            {
                foreach (var cid in newCids)
                {
                    var cx = await _ctx.ContactInfos.SingleOrDefaultAsync(oxid => oxid.ContactId.Equals(cid));
                    if (cx == null)
                    {
                        Trace.TraceWarning($"invalid contact {cid}");
                        continue;
                    }

                    log.ContactInfoContactLogs.Add(new ContactInfoContactLog { ContactLogContactLog = log, ContactInfoContactId = cid });
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

            var log = new ContactLog();
            log.Created = dt;
            log.ContactAgent = a;
            log.Notes = notes;
            log.DateAdded = date;
            log.ProjectPhase = phase;

            p.ParcelContactLogs.Add(new ParcelContactLog { ParcelParcel = p, ContactLogContactLog = log });

            _ctx.ContactLogs.Add(log);

            if (await WriteDb() <= 0)
                throw new ApplicationException("Record Contact failed");

            return p;
        }

        [Obsolete("use add contactlog")]
        public async Task<Owner> RecordOwnerContact(Owner o, Agent a, string notes, DateTimeOffset date, string phase)
        {
            //var dt = DateTimeOffset.Now;

            //var log = new ContactLog();
            //log.Created = dt;
            //log.Agent = a;
            //log.Notes = notes;
            //log.DateAdded = date;
            //log.ProjectPhase = phase;

            //o.ContactLog.Add(log);

            //_ctx.ContactLog.Add(log);

            //if (await WriteDb() <= 0)
            //    throw new ApplicationException("Record Contact failed");

            return o;
        }

        #region statics lookup
        public async Task<IEnumerable<ParcelStatus>> GetParcelStatus() => await _ctx.ParcelStatuses.Where(s => s.IsActive).AsNoTracking().ToListAsync();
        public async Task<IEnumerable<ContactPurpose>> GetPurpose() => await _ctx.ContactPurposes.Include(p => p.MilestoneCodeNavigation).Where(p => p.IsActive).AsNoTracking().ToListAsync();
        #endregion

        #region documents
        public Document GetDocument(Guid id) => _ctx.Documents.Find(id);

        public async Task<Document> UpdateDocument(Document d)
        {
            if (_ctx.Entry(d).State == EntityState.Deleted)
                _ctx.Entry(d).State = EntityState.Modified;

            var a = new DocumentActivity();
            a.ChildDocument = d;
            a.ParentDocumentId = d.DocumentId;      //// model-first naming
            a.ActivityDate = DateTimeOffset.Now;
            a.Activity = "Updated Tracking";
            a.Agent = d.DocumentAgents.FirstOrDefault()?.AgentAgent ?? default;

            _ctx.DocumentActivities.Add(a);

            if (await WriteDb() <= 0)
                throw new ApplicationException("document meta-data edit failed");

            return d;
        }

        public async Task<Document> Store(string title, string document_t, string content_t, string fname, Guid agentId, byte[] content)
        {
            var d = new Document();
            d.Content = content;
            d.Title = title;
            d.DocumentType = document_t;
            d.SourceFilename = fname;
            d.ContentType = content_t;
            d.Created = DateTimeOffset.Now;

            var a =  new DocumentActivity();
            a.ChildDocument = d;
            a.ActivityDate = DateTimeOffset.Now;
            a.Activity = "Uploaded";
            a.AgentId = agentId;
            a.ParentDocumentId = d.DocumentId;

            _ctx.Documents.Add(d);
            _ctx.DocumentActivities.Add(a);

            if (await WriteDb() <= 0)
                throw new ApplicationException("document upload failed");

            return d;
        }
        #endregion
        #region row agents
        public async Task<Agent> GetAgent(Guid id)
        {
            var a = await _ctx.Agents.FindAsync(id);
            if (a == null)
                a = await GetDefaultAgent();

            return a;
        }
        public async Task<Agent> GetAgent(string name) => await _ctx.Agents.FirstOrDefaultAsync(ax => ax.AgentName.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        public async Task<Agent> GetDefaultAgent() => await _ctx.Agents.FirstOrDefaultAsync(ax => ax.AgentName.Equals("DEFAULT"));

        public async Task<IEnumerable<Agent>> GetAgents() =>
            await _ctx.Agents.AsNoTracking()
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
