using geographia.ags;
using Microsoft.EntityFrameworkCore;
using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    /// <summary>
    /// implement parcel status update. combines previous <see cref="UpdateParcelStatus"/> & <see cref="UpdateParcelStatus2"/>
    /// </summary>
    internal class UpdateParcelStatus_Ex
    {
        readonly ROWM_Context _Context;
        readonly OwnerRepository _repo;
        readonly IFeatureUpdate _feature;
        readonly DocTypes _docTypes;
        Dictionary<string,Contact_Purpose> _purposes;

        internal UpdateParcelStatus_Ex(ROWM_Context c, OwnerRepository r, IFeatureUpdate f, DocTypes dt) => (_Context, _repo, _feature, _docTypes) = (c, r, f, dt);

        internal async Task<bool> Update(IEnumerable<Parcel> parcels, Agent agent, string notes, DateTimeOffset dt, Document doc, ContactLog log)
        {
            var updated = false;

            if (!parcels.Any())
                return updated;

            if (doc == null && log == null)
                return updated;

            var proposed = FindTrigger(doc, log);
            if (proposed == null)
                return updated;

            var myTasks = new List<Task>();

            foreach( var parcel in parcels)
            {
                var (move, status) = CheckIfProgressed(parcel);
                if (move)
                {
                    // updates
                    myTasks.Add(_feature.UpdateFeature(parcel.Tracking_Number, status.DomainValue));

                    parcel.Parcel_Status = status;
                    parcel.LastModified = DateTimeOffset.UtcNow;
                    parcel.ModifiedBy = "TRIGGER";

                    updated = true;
                }

                WriteHistory(parcel, agent, notes, dt, proposed, move);
            }

            myTasks.Add(_Context.SaveChangesAsync());

            await Task.WhenAll(myTasks);

            return updated;
        }

        internal async Task<bool> Update(IEnumerable<string> parcels, Agent agent, string notes, DateTimeOffset dt, Document doc, ContactLog log)
        {
            if (doc == null && log == null)
                return false;

            var proposed = FindTrigger(doc, log);
            if (proposed == null)
                return false;

            if (!parcels.Any())
                return false;

            var ts = parcels.Select(px => _repo.GetParcel(px));
            var ps = await Task.WhenAll(ts);

            return await Update(ps, agent, notes, dt, doc, log);
        }

        /// <summary>
        /// check if status has progressed
        /// </summary>
        /// <param name="parcel"></param>
        /// <returns></returns>
        internal (bool, Parcel_Status) CheckIfProgressed(Parcel parcel)
        {
            var statux = new List<Parcel_Status>();

            // get all documents
            var mydocs = parcel.Document.Select(d => d.DocumentType)?.Distinct() ?? default;
            if (mydocs.Any())
                statux.AddRange(mydocs.Select(d => _docTypes.Find(d).Milestone).Where(mx => mx != null));

            // get all logs
            var myphases = parcel.ContactLog.Select(l => l.ProjectPhase).Distinct();
            if (myphases.Any())
            {
                var rr = myphases.Select(mp => FindPurpose(mp));
                if (rr.Any())
                    statux.AddRange(rr);
            }

            // nothing triggered
            if (!statux.Any())
                return (false, default);

            // get current status
            var currentParcel = parcel.Parcel_Status;
            var currentROE = parcel.Roe_Status;

            var proposed = statux.OrderByDescending(dt => dt.DisplayOrder).First();

            // is proposed status a valid move
            return ((proposed.DisplayOrder > currentParcel.DisplayOrder), proposed);
        }

        internal StatusActivity WriteHistory(Parcel p, Agent a, string notes, DateTimeOffset dt, Parcel_Status s, bool moved)
        {
            var history = new StatusActivity();

            history.ParentParcelId = p.ParcelId;
            history.ActivityDate = dt;
            history.AgentId = a.AgentId;
            history.Notes = notes;

            history.ParcelStatusCode = s.Code;

            if ( moved )
                history.OrigianlParcelStatusCode = p.ParcelStatusCode;

            _Context.Activities.Add(history);

            return history;
        }

        #region helpers
        internal Parcel_Status FindTrigger(Document d, ContactLog l)
        {
            if (d != null)
            {
                var myType = _docTypes.Find(d.DocumentType);
                return myType?.Milestone ?? default;
            }

            if (l != null)
            {
                var myPurpose = FindPurpose(l.ProjectPhase);
                return myPurpose ?? default;
            }

            return default;
        }

        Parcel_Status FindPurpose(string code)
        {
            if (_purposes == null)
                //_purposes = _repo.GetPurpose().GetAwaiter().GetResult().ToDictionary<Contact_Purpose, string>(p => p.PurposeCode);
                _purposes = _Context.Contact_Purpose.ToDictionary<Contact_Purpose, string>(p => p.PurposeCode);

            return (_purposes.ContainsKey(code))
                ? _purposes[code].Milestone
                : default;
        }
        #endregion
    }
}
