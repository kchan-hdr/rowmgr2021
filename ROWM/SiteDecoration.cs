using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ROWM
{
    public interface SiteDecoration
    {
        string SiteTitle();
    }

    public class ReserviorSite : SiteDecoration
    {
        public string SiteTitle() => "Sites Reservior";
    }

    public class Atc6943: SiteDecoration
    {
        public string SiteTitle() => "ATC Line 6943";
    }
}
