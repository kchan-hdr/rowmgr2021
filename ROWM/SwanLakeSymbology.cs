using geographia.ags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;

namespace ROWM
{
    public class SwanLakeSymbology : IMapSymbology
    {
        public IEnumerable<DomainValue> RoeSymbols { get; private set; }
        public IEnumerable<DomainValue> ClearanceSymbols { get; private set; }
        public IEnumerable<DomainValue> AcquisitionSymbols { get; private set;  }
        public IEnumerable<DomainValue> OutreachSymbols { get; private set; }

        readonly IRenderer _renderer;
        bool hasSymbology = false;

        public SwanLakeSymbology(IRenderer r) => _renderer = r;


        public async Task<bool> ExtractSymbology()
        {
            if (this.hasSymbology)
                return true;

            this.RoeSymbols = await _renderer.GetDomainValues("parcel roe progress");
            this.ClearanceSymbols = new List<DomainValue>();
            this.AcquisitionSymbols = await _renderer.GetDomainValues("tract acquisition status");
            this.OutreachSymbols = new List<DomainValue>();

            return true;
        }
    }
}
