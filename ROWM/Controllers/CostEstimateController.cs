using Austin_Costs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM.Controllers
{
    [ApiController]
    public class CostEstimateController : ControllerBase
    {
        readonly ICostEstimateRepository _repo;

        public CostEstimateController(ICostEstimateRepository r) => _repo = r;

        [HttpGet("api/parcels/{parcelKey}/costs")]
        [ProducesDefaultResponseType(typeof(IEnumerable<CostEstimate>))]
        public IActionResult Get(string parcelKey)
        {
            var estimate = _repo.Get(parcelKey);
            if (!estimate.Any())
                return BadRequest($"parcel key not found {parcelKey}");

            return new JsonResult(estimate);
        }

        [HttpGet("api/parcels/{parcelKey}/costs/trend")]
        [ProducesDefaultResponseType(typeof(CostsTrend))]
        public IActionResult GetTrend(string parcelKey)
        {
            var estimate = _repo.Get(parcelKey);
            if (!estimate.Any())
                return BadRequest($"parcel key not found {parcelKey}");

            return new JsonResult(new CostsTrend(estimate));
        }
    }


    public class CostsTrend
    {
        public IEnumerable<CostsDto> Costs { get; set; }

        CostsDto Costs15 { get; set; } = new CostsDto { Milestone = "15%" };
        CostsDto Costs30 { get; set; } = new CostsDto { Milestone = "30%" };
        CostsDto Costs60 { get; set; } = new CostsDto { Milestone = "60%" };
        CostsDto Costs90 { get; set; } = new CostsDto { Milestone = "90%" };
        CostsDto CostsFinal { get; set; } = new CostsDto { Milestone = "final" };

        public CostsDto Latest { get { return Costs15; } }

        public CostsTrend(IEnumerable<CostEstimate> e)
        {
            foreach (var est in e)
            {
                if (est.DesignMilestone == "15%")
                    Costs15 = new CostsDto { Milestone = est.DesignMilestone, TotalAcqCosts = est.TotalAcquisitionCost, TotalRowCosts = est.TotalRowCost };
            }

            Costs = new[] { Costs15, Costs30, Costs60, Costs90, CostsFinal };
        }
    }

    public class CostsDto
    {
        public string Milestone { get; set; }
        public double? TotalAcqCosts { get; set; }
        public double? TotalRowCosts { get; set; }
    }
}
