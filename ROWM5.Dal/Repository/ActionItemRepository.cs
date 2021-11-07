using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

#nullable enable

namespace ROWM.Dal
{
    public interface IActionItemRepository
    {
        Task<IEnumerable<ActionItem>> GetActionItems(string parelId);
        Task<IEnumerable<ActionItem>> GetActionItemsWithHistory(string parelId);
        Task<ActionItem> GetActionItem(Guid itemId);
        Task<IEnumerable<ActionItem>> AddActionItem(string parelId, ActionItem item, DateTimeOffset activityDate);
        Task<ActionItem> UpdateActionItem(ActionItem item, DateTimeOffset activityDate);
    }

    public class ActionItemNoOp : IActionItemRepository
    {
        public Task<IEnumerable<ActionItem>> AddActionItem(string parelId, ActionItem item, DateTimeOffset activityDate) => Task.FromResult(Enumerable.Empty<ActionItem>());

        public Task<ActionItem> GetActionItem(Guid itemId) => default;

        public Task<IEnumerable<ActionItem>> GetActionItems(string parelId) => Task.FromResult(System.Linq.Enumerable.Empty<ActionItem>());

        public Task<IEnumerable<ActionItem>> GetActionItemsWithHistory(string parelId)
        {
            throw new NotImplementedException();
        }

        public Task<ActionItem> UpdateActionItem(ActionItem item, DateTimeOffset activityDate) => default;
    }

    public class ActionItemRepository : IActionItemRepository
    {
        #region ctor
        readonly ROWM_Context _ctx;
        readonly OwnerRepository _ownerRepository;

        public ActionItemRepository(ROWM_Context context, OwnerRepository ownerRepository) => (_ctx, _ownerRepository) = (context, ownerRepository);
        #endregion
        public async Task<IEnumerable<ActionItem>> AddActionItem(string parcelId, ActionItem item, DateTimeOffset activityDate)
        {
            var p = await _ownerRepository.GetParcel(parcelId);
            _ = p ?? throw new KeyNotFoundException($"cannot find parcel {parcelId}");

            item.ParcelId = p.ParcelId;
            item.ActionItemActivities = new List<ActionItemActivity>()
            {
                new ActionItemActivity
                {
                    Status = item.Status,
                    DueDate = item.DueDate,
                    Assigned = item.AssignedGroupId ?? Guid.Empty,
                    ActivityDate = activityDate
                }
            };

            _ctx.ActionItems.Add(item);

            var touched = await _ctx.SaveChangesAsync();
            if (touched <= 0)
                Trace.TraceWarning("write failed");

            return p.ActionItems;
        }

        public async Task<IEnumerable<ActionItem>> GetActionItems(string parcelId)
        {
            var p = await _ownerRepository.GetParcel(parcelId);
            _ = p ?? throw new KeyNotFoundException($"cannot find parcel {parcelId}");

            return p.ActionItems;
        }

        public Task<IEnumerable<ActionItem>> GetActionItemsWithHistory(string parcelId)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionItem> GetActionItem(Guid id) => await _ctx.ActionItems.FindAsync(id);

        public async Task<ActionItem> UpdateActionItem(ActionItem item, DateTimeOffset activityDate)
        {
            var axt = new ActionItemActivity
            {
                Action = item.Action,
                Status = item.Status,
                DueDate = item.DueDate,
                Assigned = item.AssignedGroupId ?? Guid.Empty,
                ActivityDate = activityDate
            };
            var acts = (ICollection<ActionItemActivity>)(item.ActionItemActivities ??= new List<ActionItemActivity>());

            if (acts.Any())
            {
                var last = item.ActionItemActivities.OrderByDescending(h => h.ActivityDate).First();
                if (last.Status != item.Status) 
                    axt.OriginalStatus = last.Status;                    

                if (last.DueDate != item.DueDate)
                    axt.OriginalDueDate = last.DueDate;

                if (last.Assigned != item.AssignedGroupId)
                    axt.OriginalAssigned = last.Assigned;

                if (last.Action != item.Action)
                    axt.OriginalAction = last.Action;
            }

            item.LastModified = DateTimeOffset.UtcNow;
            acts.Add(axt);

            var touched = await _ctx.SaveChangesAsync();
            if (touched <= 0)
                Trace.TraceWarning("write failed");

            return item;
        }
    }
}
