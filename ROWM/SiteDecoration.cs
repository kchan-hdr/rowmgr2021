namespace ROWM
{
    public interface SiteDecoration
    {
        string SiteTitle();
    }

    public class Dw : SiteDecoration
    {
        public string SiteTitle() => "Denver Water";
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

    public class Wharton: SiteDecoration
    {
        public string SiteTitle() => "City of Wharton";
    }

    public class Dripping : SiteDecoration
    {
        public string SiteTitle() => "City of Dripping Springs";
    }
    
    public class Sh72: SiteDecoration
    {
        public string SiteTitle() => "Yoakum SH-72";
    }
    
    public class TxDotDemo : SiteDecoration
    {
        public string SiteTitle() => "TxDOT unknown";
    }

    public class Atp : SiteDecoration
    {
        public string SiteTitle() => "ATP";
    }
}
