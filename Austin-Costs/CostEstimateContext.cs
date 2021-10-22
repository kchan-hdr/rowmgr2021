using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Austin_Costs
{
    public class CostEstimateContext : DbContext
    {
        public CostEstimateContext(string c) : base(c) { }

        public DbSet<CostEstimate> Estimates { get; set; }
    }
}
