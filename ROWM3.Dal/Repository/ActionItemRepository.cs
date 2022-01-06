using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public interface IActionItemRepository
    {
        Task<IEnumerable<ActionItem>> GetActionItems(string parelId);
        Task<IEnumerable<ActionItem>> GetActionItemsWithHistory(string parelId);
        Task<ActionItem> GetActionItem(Guid itemId);
        Task<ActionItem> GetFullItem(Guid itemId);
        Task<IEnumerable<ActionItem>> AddActionItem(string parelId, ActionItem item, DateTimeOffset activityDate, Guid? agentId);
        Task<ActionItem> UpdateActionItem(ActionItem item, DateTimeOffset activityDate, Guid? agentId);

        #region workaround
        Task<IEnumerable<Agent>> GetAgents();
        Task<Agent> UpdateAgent(Agent a);
        #endregion
    }

    public class ActionItemNoOp : IActionItemRepository
    {
        public Task<IEnumerable<ActionItem>> AddActionItem(string parelId, ActionItem item, DateTimeOffset activityDate, Guid? agentId) => Task.FromResult(Enumerable.Empty<ActionItem>());

        public Task<ActionItem> GetActionItem(Guid itemId) => default;

        public Task<ActionItem> GetFullItem(Guid itemId) => default;

        public Task<IEnumerable<ActionItem>> GetActionItems(string parelId) => Task.FromResult(System.Linq.Enumerable.Empty<ActionItem>());

        public Task<IEnumerable<ActionItem>> GetActionItemsWithHistory(string parelId)
        {
            throw new NotImplementedException();
        }

        public Task<ActionItem> UpdateActionItem(ActionItem item, DateTimeOffset activityDate, Guid? agentId) => default;

        public Task<IEnumerable<Agent>> GetAgents()
        {
            throw new NotImplementedException();
        }

        public Task<Agent> UpdateAgent(Agent a)
        {
            throw new NotImplementedException();
        }
    }

    public class ActionItemRepository : IActionItemRepository
    {
        readonly ROWM_Context _ctx;
        readonly OwnerRepository _ownerRepository;

        public ActionItemRepository(ROWM_Context context, OwnerRepository ownerRepository) => (_ctx, _ownerRepository) = (context, ownerRepository);

        public async Task<IEnumerable<ActionItem>> AddActionItem(string parelId, ActionItem item, DateTimeOffset activityDate, Guid? agentId)
        {
            var p = await _ownerRepository.GetParcel(parelId);
            if (p == null)
                throw new KeyNotFoundException();

            item.ParcelId = p.ParcelId;
            item.Activities = new List<ActionItemActivity>()
            {
                new ActionItemActivity
                {
                    Status = item.Status,
                    DueDate = item.DueDate,
                    Assigned = item.AssignedGroupId.Value,
                    ActivityDate = activityDate,
                    UpdateAgentId = agentId
                }
            };

            _ctx.ActionItem.Add(item);

            var touched = await _ctx.SaveChangesAsync();
            if (touched <= 0)
                Trace.TraceWarning("write failed");

            return p.ActionItems;
        }

        public async Task<IEnumerable<ActionItem>> GetActionItems(string parelId)
        {
            var p = await _ownerRepository.GetParcel(parelId);
            if (p == null)
                throw new KeyNotFoundException();

            return p.ActionItems;
        }

        public Task<IEnumerable<ActionItem>> GetActionItemsWithHistory(string parelId)
        {
            throw new NotImplementedException();
        }

        public async Task<ActionItem> GetActionItem(Guid itemId) => await _ctx.ActionItem.FindAsync(itemId);
        public async Task<ActionItem> GetFullItem(Guid itemId)
        {
            try
            {
                return await _ctx.ActionItem
                    //.Include(ax => ax.ParentParcel)
                    .Include(ax => ax.AssignedGroup.Members)
                    .Include(ax => ax.Activities)
                    .Include(ax => ax.Activities.Select(aa => aa.UpdateAgent))
                    .SingleOrDefaultAsync(ax => ax.ActionItemId == itemId);
            }
            catch ( Exception e)
            {
                throw;
            }
        }

        public async Task<ActionItem> UpdateActionItem(ActionItem item, DateTimeOffset activityDate, Guid? agentId)
        {
            _ = item ?? throw new ArgumentNullException(nameof(item));

            var axt = new ActionItemActivity
            {
                Action = item.Action,
                Status = item.Status,
                DueDate = item.DueDate,
                Assigned = item.AssignedGroupId.Value,
                ActivityDate = activityDate,
                UpdateAgentId = agentId
            };

            var acts = item.Activities ?? (item.Activities = new List<ActionItemActivity>());

            if (acts.Any())
            {
                var last = item.Activities.OrderByDescending(h => h.ActivityDate).First();
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

        #region helper
        public async Task<IEnumerable<Agent>> GetAgents() =>
            await _ctx.Agent.AsNoTracking().ToArrayAsync();

        public async Task<Agent> UpdateAgent(Agent a)
        {
            if (_ctx.Entry(a).State == EntityState.Detached)
            {
                _ctx.Agent.Attach(a);
                _ctx.Entry(a).State = EntityState.Modified;
            }

            await _ctx.SaveChangesAsync();
            return a;
        }
        #endregion
    }
}
