using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

#nullable enable

namespace ROWM.Dal
{
    public class DeleteHelper
    {
        #region ctor
        private readonly ROWM_Context _ctx;
        public DeleteHelper(ROWM_Context c) => _ctx = c;
        #endregion

        public async Task<bool> DeleteOwner(Guid ownerId, Guid? parcelId, string act = "DELETE")
        {
            if (string.IsNullOrWhiteSpace(act))
                act = "DELETE";

            try
            {
                var owner = _ctx.Owners.Find(ownerId);
                _ = owner ?? throw new KeyNotFoundException($"cannot find owner {ownerId}");

                owner.IsDeleted = true;
                owner.LastModified = DateTime.Now;
                owner.ModifiedBy = act;

                if (parcelId == null)
                {
                    owner.Ownerships = new List<Ownership>();   // empty existing ownerships
                } 
                else
                {
                    var dead = owner.Ownerships.FirstOrDefault(ox => ox.ParcelId == parcelId);
                    if (dead != null)
                        owner.Ownerships.Remove(dead);
                }

                var touched = await _ctx.SaveChangesAsync();
                return touched > 0;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return false;
            }
        }

        public async Task< bool> DeleteContact(Guid contactId, string act = "DELETE")
        {
            if (string.IsNullOrWhiteSpace(act))
                act = "DELETE";

            try
            {
                var c = _ctx.ContactInfos.Find(contactId);
                _ = c ?? throw new KeyNotFoundException($"cannot find contactInfo {contactId}");

                c.IsDeleted = true;
                c.LastModified = DateTime.Now;
                c.ModifiedBy = act;

                await _ctx.SaveChangesAsync();
                return true;
            }
            catch ( Exception e)
            {
                Trace.TraceError(e.Message);
                return false;
            }
        }

        public async Task< bool> DeleteContactLog(Guid logId, string act = "DELETE")
        {
            if (string.IsNullOrWhiteSpace(act))
                act = "DELETE";

            try
            {
                var c = _ctx.ContactLogs.Find(logId);
                _ = c ?? throw new KeyNotFoundException($"cannot find log {logId}");

                c.IsDeleted = true;
                c.LastModified = DateTime.Now;
                c.ModifiedBy = act;

                //c.Parcel = new List<Parcel>();
                c.ParcelContactLogs.Clear();

                await _ctx.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return false;
            }
        }

        public async Task<bool> DeleteDocument(Guid docId, string act = "DELETE")
        {
            if (string.IsNullOrWhiteSpace(act))
                act = "DELETE";

            try
            {
                var d = _ctx.Documents.Find(docId);
                _ = d ?? throw new KeyNotFoundException($"cannot find document {docId}");

                d.IsDeleted = true;
                d.LastModified = DateTime.Now;
                d.ModifiedBy = act;

                d.OwnerDocuments.Clear();
                d.ParcelDocuments.Clear();

                await _ctx.SaveChangesAsync();
                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
                return false;
            }
        }
    }
}
