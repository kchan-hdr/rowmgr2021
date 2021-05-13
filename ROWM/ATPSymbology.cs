using geographia.ags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM
{
    public interface IMapSymbology
    {
        IEnumerable<DomainValue> RoeSymbols { get; }
        IEnumerable<DomainValue> ClearanceSymbols { get; }
        IEnumerable<DomainValue> AcquisitionSymbols { get; }

        Task<bool> ExtractSymbology();

    }

    public class AtpSymbology : IMapSymbology
    {
        public IEnumerable<DomainValue> RoeSymbols { get; private set; }
        public IEnumerable<DomainValue> ClearanceSymbols { get; private set; }
        public IEnumerable<DomainValue> AcquisitionSymbols { get; private set;  }

        readonly IRenderer _renderer;
        bool hasSymbology = false;

        public AtpSymbology(IRenderer r) => _renderer = r;


        public async Task<bool> ExtractSymbology()
        {
            if (this.hasSymbology)
                return true;

            this.RoeSymbols = await _renderer.GetDomainValues(10);
            this.ClearanceSymbols = await _renderer.GetDomainValues(10);
            this.AcquisitionSymbols = await _renderer.GetDomainValues(11);

            return true;
        }
    }
}
