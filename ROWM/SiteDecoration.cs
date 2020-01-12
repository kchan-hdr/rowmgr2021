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

    public class B2H: SiteDecoration
    {
        public string SiteTitle() => "B2H";
    }
    
    public class Atc6943: SiteDecoration
    {
        public string SiteTitle() => "ATC Line 6943";
    }

    public class Atc862: SiteDecoration
    {
        public string SiteTitle() => "ATC Line 862";
    }

    public class AtcChc: SiteDecoration
    {
        public string SiteTitle() => "ATC Cardinal-Hickory Creek";
    }
}
