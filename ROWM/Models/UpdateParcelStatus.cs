﻿using geographia.ags;
using ROWM.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    /// <summary>
    /// Implements parcel status update
    /// </summary>
    public class UpdateParcelStatus
    {
        readonly OwnerRepository repo;
        readonly IFeatureUpdate _featureUpdate;
        readonly ParcelStatusHelper _statusHelper;
        readonly ROWM_Context _context;


        Agent myAgent;
        IEnumerable<Parcel> myParcels;

        public DateTimeOffset StatusChangeDate { get; set; } = DateTimeOffset.Now;
        public string AcquisitionStatus { get; set; }
        public string RoeStatus { get; set; }
        public string RoeCondition { get; set; }
        public string Notes { get; set; }

        public string ModifiedBy { get; set; } = "UP";

        public UpdateParcelStatus(IEnumerable<Parcel> parcels, Agent agent, ROWM_Context context, OwnerRepository repository, IFeatureUpdate featureUpdate, ParcelStatusHelper h)
        {
            this.myAgent = agent;
            this.myParcels = parcels;

            this._context = context;
            this.repo = repository;
            this._statusHelper = h;
            this._featureUpdate = featureUpdate;
        }

        public async Task<int> Apply()
        {
            if (!this.myParcels.Any())
                return 0;


            var dt = DateTimeOffset.Now;

            var tks = new List<Task>();

            // foreach parcel
            foreach (var p in this.myParcels)
            {
                var dirty = false;
                var history = new StatusActivity();

                var pid = p.Assessor_Parcel_Number;

                if (this.AcquisitionStatus != null && p.ParcelStatusCode != this.AcquisitionStatus)
                {
                    history.OrigianlParcelStatusCode = p.ParcelStatusCode;
                    history.ParcelStatusCode = this.AcquisitionStatus;

                    p.ParcelStatusCode = this.AcquisitionStatus;
                    dirty = true;

                    var dv = _statusHelper.GetDomainValue(AcquisitionStatus);
                    tks.Add(this._featureUpdate.UpdateFeature(pid, dv));
                }

                if (this.RoeStatus != null && p.RoeStatusCode != this.RoeStatus)
                {
                    history.OriginalRoeStatusCode = p.RoeStatusCode;
                    history.RoeStatusCode = this.RoeStatus;

                    p.RoeStatusCode = this.RoeStatus;
                    dirty = true;

                    if (!string.IsNullOrWhiteSpace(RoeCondition))
                    {
                        p.Conditions.Add(new Dal.RoeCondition { Condition = RoeCondition, Created = dt, LastModified = dt, ModifiedBy = this.ModifiedBy });
                    }

                    var roeDV = _statusHelper.GetRoeDomainValue(RoeStatus);
                    tks.Add(string.IsNullOrWhiteSpace(RoeCondition) ?
                        _featureUpdate.UpdateFeatureRoe(pid, roeDV) : _featureUpdate.UpdateFeatureRoe_Ex(pid, roeDV, RoeCondition));
                }

                if (dirty)
                {
                    history.ParentParcelId = p.ParcelId;
                    history.AgentId = this.myAgent.AgentId;
                    history.ActivityDate = this.StatusChangeDate;

                    history.Notes = this.Notes;

                    p.LastModified = dt;
                    p.ModifiedBy = this.ModifiedBy;

                    this._context.Activities.Add(history);
                }
            }

            tks.Add(this._context.SaveChangesAsync());

            await Task.WhenAll(tks);

            return 0;
        }
    }
}
