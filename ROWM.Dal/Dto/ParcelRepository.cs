using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    using AutoMapper.QueryableExtensions;

    public class ParcelRepository
    {
        private readonly ROWM_Context _ctx;

        public ParcelRepository(ROWM_Context c = null)
        {
            _ctx = c == null ? new ROWM_Context() : c;
        }

        public ParcelGraph_Dto GetParcel(string pid) => _ctx.Parcels.Where( px=> px.ParcelId.Equals(pid))
                .ProjectToFirstOrDefault<ParcelGraph_Dto>();        // optimize the fields returned from the query. mostly don't download the pdf, thus save tons of time.
        

        #region mapper configure
        static ParcelRepository()
        {
            Mapper.Initialize(conf => {
                conf.CreateMap<Parcel, ParcelGraph_Dto>();
                conf.CreateMap<Parcel, ParcelGraph_Dto.ParcelHeaderDto>();
                conf.CreateMap<Ownership, ParcelGraph_Dto.OwnerDto>()
                    .ForMember(d => d.PartyName, o => o.MapFrom(s=> s.Owner.PartyName));
                conf.CreateMap<ContactInfo, ParcelGraph_Dto.ContactInfoDto>()
                    .ForMember(d=> d.ContactName, o => o.MapFrom(s=>$"{s.OwnerLastName}, {s.OwnerFirstName}"));
                conf.CreateMap<ContactLog, ParcelGraph_Dto.ContactLogDto>()
                    .ForMember(d => d.AgentName, o=>o.MapFrom(s=>s.ContactAgent.AgentName));
                conf.CreateMap<Document, ParcelGraph_Dto.DocumentHeader>()
                    .ForMember(d => d.DocumentTitle, o => o.MapFrom(s => s.Title))
                    .ForMember(d => d.AgentName, o => o.MapFrom(s => s.Agents.FirstOrDefault().AgentName));
            });
        }
        #endregion
    }
}
