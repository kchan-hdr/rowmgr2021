using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ROWM.Dal;
using ROWM.Models;

namespace ROWM.Controllers
{
    [ApiController]
    public class SH72Controller : ControllerBase
    {
        readonly OwnerRepository _repo;

        public SH72Controller(OwnerRepository r) => _repo = r;

        [Route("parcels/{pid}/n94"), HttpGet]
        public async Task<IActionResult> GetN94Report(string pid)
        {
            var p = await _repo.GetParcel(pid);
            if (p == null)
                return BadRequest();

            var h = new N94Helper();
            var working = await h.Generate(p);

            return File(working.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"{pid} N-94.docx");
        }
    }
}
