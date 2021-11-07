using Microsoft.EntityFrameworkCore;

namespace Austin_Costs
{
    public class CostEstimateContext : DbContext
    {
        public CostEstimateContext(DbContextOptions o) : base(o) { }

        public DbSet<CostEstimate> Estimates { get; set; }
    }
}
