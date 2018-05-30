using com.hdr.Rowm.Sunflower;
using Microsoft.AspNetCore.Mvc;
using ROWM.Dal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM.Controllers
{
    [Produces("application/json")]
    public class VocabularyController
    {
        private readonly ROWM_Context _Context;

        public VocabularyController(ROWM_Context c) => _Context = c;

        [HttpGet("api/vocabulary")]
        public Vocabulary Get()
        {
            var agents = _Context.Agents.Where(a => a.IsActive);
            var channels = _Context.Channels.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder);
            var purposes = _Context.Purposes.Where(p => p.IsActive).OrderBy(p => p.DisplayOrder);
            var rels = _Context.Representations.Where(r => r.IsActive).OrderBy(r => r.DisplayOrder);

            var pStatus = _Context.ParcelStatus.Where(p => p.IsActive).OrderBy(p => p.DisplayOrder);
            var rStatus = _Context.RoeStatus.Where(p => p.IsActive).OrderBy(p => p.DisplayOrder);

            return new Vocabulary(agents,channels,purposes,rels, pStatus, rStatus);
        }

        [HttpGet("api/DocTypes")]
        public IEnumerable<DocType> GetDocTypes() => DocType.Types;

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

            internal Vocabulary(
                IEnumerable<Agent> agents, 
                IEnumerable<Channel_Master> channels, 
                IEnumerable<Purpose_Master> purposes, 
                IEnumerable<Representation> rels,
                IEnumerable<ParcelStatus_Master> p,
                IEnumerable<RoeStatus_Master> r)
            {
                Agents = agents.Select(a => new Lookup { Code = a.AgentId.ToString(), Description = a.AgentName });
                Channels = channels.Select(c => new Lookup { Code = c.ContactTypeCode, Description = c.Description, DisplayOrder = c.DisplayOrder });
                Purposes = purposes.Select(c => new Lookup { Code = c.PurposeCode, Description = c.Description, DisplayOrder = c.DisplayOrder });
                RelationTypes = rels.Select(c => new Lookup { Code = c.RelationTypeCode, Description = c.Description, DisplayOrder = c.DisplayOrder });

                ParcelStatus = p.Select(c => new Lookup { Code = c.Code, Description = c.Description, DisplayOrder = c.DisplayOrder });
                RoeStatus = r.Select(c => new Lookup { Code = c.Code, DisplayOrder = c.DisplayOrder, Description = c.Description });
            }
        }
    }
}
