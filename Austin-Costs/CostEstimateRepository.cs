using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Austin_Costs
{
    public interface ICostEstimateRepository
    {
        IEnumerable<CostEstimate> Get(string key);
    }

    public class CostEstimateRepository : ICostEstimateRepository
    {
        readonly CostEstimateContext _ctx;
        public CostEstimateRepository(CostEstimateContext c) => (_ctx) = (c);



        public IEnumerable<CostEstimate> Get(string parcelKey) => _ctx.Estimates.Where(ex => ex.PropId == parcelKey);
    }
}
