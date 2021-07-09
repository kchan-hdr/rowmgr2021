using com.hdr.rowmgr.Relocation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM.Controllers
{
    [Produces("application/json")]
    [ApiController]
    public class RelocationController : ControllerBase
    {
        readonly OwnerRepository _oRepo;
        readonly RelocationRepository _repo;
        readonly IRelocationCaseOps _caseOps;
        public RelocationController(OwnerRepository or, RelocationRepository r, IRelocationCaseOps ops) => (_oRepo, _repo, _caseOps) = (or, r, ops);

        [HttpGet("api/RelocationActivityTypes"), ResponseCache(Duration=60*60)]
        public async Task<IEnumerable<IRelocationActivityType>> GetTypes() => await _repo.GetActivityTypes();

        [HttpGet("api/RelocationActivityPicklist"), ResponseCache(Duration = 60 * 60)]
        public async Task<IEnumerable<ActivityTaskPick>> GetPicklist()
        {
            var acts = await _repo.GetActivityTypes();

            return acts.OrderBy(a => a.DisplayOrder)
                .SelectMany(a => a.GetTasks())
                .ToArray();
        }

        /// <summary>
        /// Get Relocation Cases by APN or Tracking Number
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        [HttpGet("api/parcels/{pid}/relocations")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RelocationDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRelocation(string pid)
        {
            var p = await _oRepo.GetParcel(pid);
            if (p == null)
                return NotFound($"bad parcel {pid}");

            var r = await _repo.GetRelocation(p.ParcelId);
            if (r==null)
                return NotFound($"bad parcel {pid}");

            // this needs to be configurable
            foreach( var c in r.RelocationCases)
                c.ParcelKey = p.Tracking_Number;

            return new JsonResult(new RelocationDto(r));
        }

        [HttpGet("api/parcels/{pId:Guid}/relocations")]
        [ProducesResponseType(StatusCodes.Status200OK, Type=typeof(RelocationDto))]
        public async Task<IActionResult> GetRelocation(Guid pId)
        {
            if (Guid.Empty == pId)
                return BadRequest();

            var r = await _repo.GetRelocation(pId);
            
            return new JsonResult(new RelocationDto(r));
        }

        [HttpPost("api/parcels/{pId:Guid}/relocations")]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(RelocationDto))]
        public async Task<IActionResult> AddRelocationCase(Guid pId, [FromBody] RequestCase rCase)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var r = await _caseOps.AddRelocationCase(pId, rCase.DisplaceeName, rCase.Eligibility, rCase.DisplaceeType, rCase.Hs, rCase.Rap);
            return new JsonResult(new RelocationDto(r));
        }

        [HttpPost("api/relocations/{rcId}/activities"), ActionName("GetReloCase")]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(RelocationDto))]
        public async Task<IActionResult> AddRelocationActivity(Guid rcId, [FromBody] RequestActivity act)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var r = await _caseOps.AddActivity(rcId
                , act.ActivityCode
                , act.Activity
                , act.Description
                , act.AgentId
                , act.ActivityDate
                , act.Notes
                , act.Money
                , act.YesNo);
            
            return new JsonResult(new RelocationDto(r));
        }
    }


    #region dto
    public class RelocationDto
    { 
        public Guid ParcelId { get; set; }
        public IEnumerable<RelocationCaseDto> Cases { get; set; }

        public RelocationDto() { }
        public RelocationDto(IParcelRelocation p)
        {
            this.ParcelId = p.ParcelId;
            Cases = p.RelocationCases.Select(c => new RelocationCaseDto(c));
        }
    }
    public class RelocationCaseDto
    {
        public Guid RelocationCaseId { get; set;  }

        public Guid? AgentId { get; set; }
        public int RelocationNumber { get; set; }
        public string Status { get; set; }
        public string DisplaceeType { get; set; }
        public string RelocationType { get; set; }

        public string DisplaceeName { get; set; }
        Guid? ContactInfoId { get; set; }


        //// details
        //IEnumerable<IRelocationEligibilityActivity> EligibilityHistory { get; }
        //IEnumerable<IRelocationDisplaceeActivity> DisplaceeActivities { get; }


        public string AcqFilenamePrefix { get; set; }
        public string CaseUrl { get; set; }

        public RelocationCaseDto(IRelocationCase c)
        {
            RelocationCaseId = c.RelocationCaseId;
            RelocationNumber = c.RelocationNumber;

            AcqFilenamePrefix = c.AcqFilenamePrefix;

            DisplaceeName = c.DisplaceeName;
            Status = Enum.GetName(typeof(RelocationStatus), c.Status);
            DisplaceeType = Enum.GetName(typeof(DisplaceeType), c.DisplaceeType);
            RelocationType = Enum.GetName(typeof(RelocationType), c.RelocationType);
        }
    }

    #region requests
    public class RequestCase
    {
        public string DisplaceeName { get; set; }
        public string Eligibility { get; set; }
        public string[] DisplaceeType { get; set; }
        //public RelocationType RelocationType { get; set; }
        public float? Hs { get; set; }
        public float? Rap { get; set; }
    }

    public class RequestActivity
    { 
        public string ActivityCode { get; set; }
        public DisplaceeActivity Activity { get; set; }
        public string Description { get; set; }
        public Guid AgentId { get; set; }
        public DateTimeOffset ActivityDate { get; set; }
        public string Notes { get; set; }
        public int? Money { get; set; }
        public bool? YesNo { get; set; }
    }
    #endregion
    #endregion
}
