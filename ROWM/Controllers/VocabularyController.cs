using geographia.ags;
using Microsoft.AspNetCore.Mvc;
using ROWM.Dal;
using SharePointInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ROWM.Controllers
{
    [Produces("application/json")]
    public class VocabularyController
    {
        private readonly ROWM_Context _Context;
        private readonly AppRepository _repo;
        private readonly DocTypes _docTypes;
        private readonly ISharePointCRUD _sp;
        private readonly IFeatureUpdate _ags;
        private readonly IRenderer _renderer;

        public VocabularyController(ROWM_Context c, AppRepository a, DocTypes d, ISharePointCRUD sp, IFeatureUpdate ags, IRenderer r)
        {
            _Context = c;
            _repo = a;
            _docTypes = d;
            _sp = sp;
            _ags = ags;
            _renderer = r;
        }

        [HttpGet("api/map")]
        public async Task<Map> GetMapConfiguration()
        {
            var r = new Map();

            r.Parcel_Fc = _repo.GetLayers().Where(lx => lx.LayerType == LayerType.Parcel).FirstOrDefault();
            r.Reference_MapLayer = _repo.GetLayers().Where(lx => lx.LayerType == LayerType.Reference).FirstOrDefault();

            var (t, d) = await _ags.Token();
            r.Token = t;
            r.Expiration = d;

            return r;
        }

        [HttpGet("api/vocabulary")]
        public Vocabulary Get()
        {
            var titles = _Context.DocumentTitlePicklist.ToArray();

            var agents = _Context.Agent.Where(a => a.IsActive);
            var channels = _Context.Contact_Channel.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder);
            var purposes = _Context.Contact_Purpose.Where(p => p.IsActive).OrderBy(p => p.DisplayOrder);
            var rels = _Context.Repesentation_Type.Where(r => r.IsActive).OrderBy(r => r.DisplayOrder);

            var pStatus = _Context.Parcel_Status.Where(p => p.IsActive && p.Category == "acquisition").OrderBy(p => p.DisplayOrder);
            var rStatus = _Context.Parcel_Status.Where(p => p.IsActive && p.Category == "roe").OrderBy(p => p.DisplayOrder);
            var cStatus = _Context.Parcel_Status.Where(p => p.IsActive && p.Category == "clearance").OrderBy(p => p.DisplayOrder);
            var oStatus = _Context.Parcel_Status.Where(p => p.IsActive && p.Category == "engagement").OrderBy(p => p.DisplayOrder);
            var llScore = _Context.Landowner_Score.Where(s => s.IsActive ?? false).OrderBy(s => s.DisplayOrder);

            return new Vocabulary(agents, channels, purposes, rels, pStatus, rStatus, cStatus, oStatus, llScore, titles);
        }

        [HttpGet("api/parcelStatus")]
        public async Task<IEnumerable<StatusDto>> GetParcelStatus()
        {
            var sym = await _renderer.GetDomainValues(11);

            var pStatus = _Context.Parcel_Status.AsNoTracking().Where(p => p.IsActive).OrderBy(p => p.DisplayOrder).ToList();

            var ss = from p in pStatus
                     join sy in sym on p.DomainValue equals sy.Code into rr
                     from rrx in rr.DefaultIfEmpty()
                     select new StatusDto(p.DisplayOrder ?? 0, p.Code, p.Description, p.ParentStatusCode, p.Category, rrx?.Hex ?? "#ffffff");

            return ss;
        }

        [HttpGet("api/DocTypes")]
        public IEnumerable<DocType> GetDocTypes() => _docTypes.Types;

        /// <summary>
        /// this is for site validation only. 
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        [HttpGet("SharePoint/{folder}")]
        public string GetSharePoint(string folder) => _sp.GetParcelFolderURL(folder, string.Empty);

        public class StatusDto
        {
            public int DisplayOrder { get; private set; }
            public string Code { get; private set; }
            public string Caption { get; private set; }
            public string Color { get; private set; }

            public string Category { get; private set; }
            public string ParentCode { get; private set; }

            internal StatusDto(int i, string value, string label, string parent, string category, string hex) => (DisplayOrder, Code, Caption, ParentCode, Category, Color) = (i, value, label, parent, category, hex);
        }
        #region lookups
        public class Lookup
        {
            public string Code { get; set; }
            public string Description { get; set; }
            public int DisplayOrder { get; set; }
        }

        public class Vocabulary
        {
            public IEnumerable<Lookup> Agents { get; set; }
            public IEnumerable<Lookup> Channels { get; set; }
            public IEnumerable<Lookup> Purposes { get; set; }
            public IEnumerable<Lookup> RelationTypes { get; set; }
            public IEnumerable<Lookup> ParcelStatus { get; set; }
            public IEnumerable<Lookup> RoeStatus { get; set; }
            public IEnumerable<Lookup> ClearanceStatus { get; set; }
            public IEnumerable<Lookup> OutreachStatus { get; set; }
            public IEnumerable<Lookup> Score { get; set; }
            public IEnumerable<DocumentTiltlePl> TitlePicklist { get; set; }

            internal Vocabulary(
                IEnumerable<Agent> agents,
                IEnumerable<Contact_Channel> channels,
                IEnumerable<Contact_Purpose> purposes,
                IEnumerable<Repesentation_Type> rels,
                IEnumerable<Parcel_Status> p,
                IEnumerable<Parcel_Status> r,
                IEnumerable<Parcel_Status> cl,
                IEnumerable<Parcel_Status> outreach,
                IEnumerable<Landowner_Score> s,
                IEnumerable<DocumentTiltlePl> titles)
            {
                Agents = agents.Select(a => new Lookup { Code = a.AgentId.ToString(), Description = a.AgentName });
                Channels = channels.Select(c => new Lookup { Code = c.ContactTypeCode, Description = c.Description, DisplayOrder = c.DisplayOrder });
                Purposes = purposes.Select(c => new Lookup { Code = c.PurposeCode, Description = c.Description, DisplayOrder = c.DisplayOrder });
                RelationTypes = rels.Select(c => new Lookup { Code = c.RelationTypeCode, Description = c.Description, DisplayOrder = c.DisplayOrder });

                ParcelStatus = p.Select(c => new Lookup { Code = c.Code, Description = c.Description, DisplayOrder = c.DisplayOrder ?? 0});
                RoeStatus = r.Select(c => new Lookup { Code = c.Code, DisplayOrder = c.DisplayOrder ?? 0, Description = c.Description });
                ClearanceStatus = cl.Select(c => new Lookup { Code = c.Code, DisplayOrder = c.DisplayOrder ?? 0, Description = c.Description });
                OutreachStatus = outreach.Select(c => new Lookup { Code = c.Code, DisplayOrder = c.DisplayOrder ?? 0, Description = c.Description });

                Score = s.Select(c => new Lookup { Code = c.Score.ToString(), DisplayOrder = c.DisplayOrder ?? 0, Description = c.Caption });

                TitlePicklist = titles;
            }
        }
        #endregion
        #region map layers
        public class Map
        {
            public MapConfiguration Parcel_Fc { get; set; }
            public MapConfiguration Reference_MapLayer { get; set; }
            public string Token { get; set; }
            public DateTimeOffset Expiration { get; set; }
        }
        #endregion
    }
}
