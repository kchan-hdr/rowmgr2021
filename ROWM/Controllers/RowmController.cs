using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ROWM.Dal;

namespace ROWM.Controllers
{
    [Produces("application/json")]
    [Route("api")]
    public class RowmController : Controller
    {
        OwnerRepository _repo;

        public RowmController(OwnerRepository r)
        {
            _repo = r;
        }
    
        [Route("owners/{id:Guid}"), HttpGet]
        public async Task<Owner> GetOwner(Guid id)
        {
            return await _repo.GetOwner(id);
        }

        [Route("owners"), HttpGet]
        public async Task<IEnumerable<Owner>> FindOwner(string name)
        {
            return await _repo.FindOwner(name);
        }

        [Route("parcels"), HttpGet]
        public IEnumerable<string> GetAllParcels()
        {
            return _repo.GetParcels();
        }

        [Route("parcels/{pid}"), HttpGet]
        public async Task<Parcel> GetParcel(string pid)
        {
            return await _repo.GetParcel(pid);
        }

        [Route("parcels/{pid}/logs"), HttpPost]
        public async Task<IActionResult> AddContactLog(string pid, [FromBody] LogRequest logRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(this.ModelState);

            var p = await _repo.GetParcel(pid);
            var a = await _repo.GetAgent(logRequest.AgentName);     // always "Agent 99"

            var r = await _repo.RecordContact(p, a, logRequest.Notes, logRequest.DateAdded, logRequest.Phase);
            return Json(r);
        }

        [Route("owners/{id}/logs"), HttpPost]
        public async Task<IActionResult> AddContactLog(Guid id, [FromBody] LogRequest logRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(this.ModelState);

            var o = await _repo.GetOwner(id);
            var a = await _repo.GetAgent(logRequest.AgentName);     // always "Agent 99"

            var r = await _repo.RecordOwnerContact(o, a, logRequest.Notes, logRequest.DateAdded, logRequest.Phase);
            return Json(r);
        }
    }

    #region request dto
    public class LogRequest
    {
        public Guid AgentId { get; set; }
        public string AgentName { get; set; }
        public DateTimeOffset DateAdded { get; set; }
        public string Notes { get; set; }
        public string Phase { get; set; }
    }
    #endregion
}