using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.hdr.Rowm.Sunflower
{
    /// <summary>
    /// Document Type lookup. TODO: table-driven
    /// </summary>
    public class DocType
    {
        #region static
        static IEnumerable<DocType> _Master;
        public static IEnumerable<DocType> Types => _Master.Where(dt => dt.IsDisplayed).OrderBy(dt => dt.DisplayOrder);
        public static DocType Find(string n) => _Master.SingleOrDefault(dt => dt.DocTypeName.Equals(n.Trim(), StringComparison.CurrentCultureIgnoreCase));
        public static DocType Default => _Master.Single(dt => dt.DocTypeName.Equals("Other"));
        #endregion

        public string DocTypeName { get; }
        public string FolderPath { get; }
        public int DisplayOrder { get; }
        public bool IsDisplayed { get; }

        internal DocType(string n, string p, int order, bool d = true)
        {
            this.DocTypeName = n;
            this.FolderPath = p;
            this.DisplayOrder = order;
            this.IsDisplayed = d;
        }

        static DocType()
        {
            var line = 0;
            var _docTypes = new List<DocType>();

            _docTypes.Add(new DocType("Other", "4.3.7 Reference", 1, false));

            _docTypes.Add(new DocType("ROE Package Original", "4.3.1 ROE/3 Final Sent to LO", ++line));
            _docTypes.Add(new DocType("ROE Package Updated", "4.3.1 ROE/3 Final Sent to LO", ++line));
            _docTypes.Add(new DocType("ROE Package Received by Owner", "4.3.1 ROE/4 Signed", ++line));
            _docTypes.Add(new DocType("ROE Package Signed", "4.3.1 ROE/4 Signed", ++line));
            _docTypes.Add(new DocType("ROE Sent to Client", "4.3.1 ROE/4 Signed", 5));

            line = 10;
            _docTypes.Add(new DocType("Market Study", "4.3.7 Reference", ++line));
            _docTypes.Add(new DocType("Survey", "4.3.7 Reference", ++line));
            _docTypes.Add(new DocType("Appraisal", "4.3.7 Reference", ++line));

            line = 20;
            _docTypes.Add(new DocType("Option Offer Package Original", "4.3.2 Option Agreement/1 Option Agreement Working", ++line, false));
            _docTypes.Add(new DocType("Option Offer Package Updated", "4.3.2 Option Agreement/2 Option Agreement QC", ++line, false));
            _docTypes.Add(new DocType("Option Offer Package Received by Owner", "4.3.2 Option Agreement/4 Option Agreement Signed Recorded Payment", ++line, false));
            _docTypes.Add(new DocType("Option Offer Package Signed", "4.3.2 Option Agreement/4 Option Agreement Signed Recorded Payment", ++line, false));
            _docTypes.Add(new DocType("Option Offer Package Sent to Client", "4.3.2 Option Agreement/4 Option Agreement Signed Recorded Payment", ++line, false));
            _docTypes.Add(new DocType("Option Compensation Check Cut", "4.3.2 Option Agreement/4 Option Agreement Signed Recorded Payment", ++line, false));
            _docTypes.Add(new DocType("Option Documents Recorded", "4.3.2 Option Agreement/4 Option Agreement Signed Recorded Payment", ++line, false));
            _docTypes.Add(new DocType("Option Compensation Received by Owner", "4.3.2 Option Agreement/4 Option Agreement Signed Recorded Payment", ++line, false));

            line = 30;
            _docTypes.Add(new DocType("Acquistion Offer Package Original", "4.3.3 Easement/3 Easement Final Sent to LO", ++line));
            _docTypes.Add(new DocType("Acquistion Offer Package Updated", "4.3.3 Easement/3 Easement Final Sent to LO", ++line));
            _docTypes.Add(new DocType("Acquisition Notice of Intent Package", "4.3.3 Easement/3 Easement Final Sent to LO", ++line));
            _docTypes.Add(new DocType("Acquistion Offer Package Received by Owner", "4.3.3 Easement/4  Easement Signed Recorded Payment", ++line));
            _docTypes.Add(new DocType("Acquisition Final Offer Package", "4.3.3 Easement/3 Easement Final Sent to LO", ++line));
            _docTypes.Add(new DocType("Acquistion Offer Package Signed", "4.3.3 Easement/4 Easement Signed Recorded Payment", ++line));
            _docTypes.Add(new DocType("Acquistion Offer Packet Sent to Client", "4.3.3 Easement/4 Easement Signed Recorded Payment", ++line));
            _docTypes.Add(new DocType("Acquisition Compensation Check", "4.3.3 Easement/4 Easement Signed Recorded Payment", ++line));
            _docTypes.Add(new DocType("Acquisition Documents Recorded", "4.3.3 Easement/4 Easement Signed Recorded Payment", ++line));
            _docTypes.Add(new DocType("Acquisition Compensation Received by Owner", "4.3.3 Easement/4 Easement Signed Recorded Payment", ++line));
            _docTypes.Add(new DocType("Acquisition Fully Signed Compenation Agreement", "4.3.3 Easement/4 Easement Signed Recorded Payment", ++line));
            _docTypes.Add(new DocType("Acquisition Fully Signed Easement Agreement", "4.3.3 Easement/4 Easement Signed Recorded Payment", ++line));
            _docTypes.Add(new DocType("Acquisition Recorded Easement Agreement", "4.3.3 Easement/4 Easement Signed Recorded Payment", ++line));

            line = 50;
            _docTypes.Add(new DocType("Construction Damages Packet Original", "4.3.4 Restoration", ++line));
            _docTypes.Add(new DocType("Construction Damages Packet Updated", "4.3.4 Restoration", ++line));
            _docTypes.Add(new DocType("Construction Damages Packet Signed", "4.3.4 Restoration", ++line));
            _docTypes.Add(new DocType("Construction Damages Packet Sent to Client", "4.3.4 Restoration", ++line));
            _docTypes.Add(new DocType("Construction Damages Check", "4.3.4 Restoration", ++line));
            _docTypes.Add(new DocType("Construction Damages Compensation Received by Owner", "4.3.4 Restoration", ++line));

            _Master = _docTypes;
        }
        /*
         * copied from SharePointCRUD
         * 
            _docTypes = new Dictionary<string, string>();

            _docTypes.Add("Other", "4.3.7 Reference");
            _docTypes.Add("ROE Package Original", "4.3.1 ROE/3 Final Sent to LO");
            _docTypes.Add("ROE Package Updated", "4.3.1 ROE/3 Final Sent to LO");
            _docTypes.Add("ROE Package Received by Owner", "4.3.1 ROE/4 Signed");
            _docTypes.Add("ROE Package Signed", "4.3.1 ROE/4 Signed");
            _docTypes.Add("ROE Sent to Client", "4.3.1 ROE/4 Signed");

            _docTypes.Add("Market Study", "4.3.7 Reference");
            _docTypes.Add("Survey", "4.3.7 Reference");
            _docTypes.Add("Appraisal", "4.3.7 Reference");

            _docTypes.Add("Option Offer Package Original", "4.3.2 Option Agreement/1 Option Agreement Working");
            _docTypes.Add("Option Offer Package Updated", "4.3.2 Option Agreement/2 Option Agreement QC");
            _docTypes.Add("Option Offer Package Received by Owner", "4.3.2 Option Agreement/4 Option Agreement Signed Recorded Payment");
            _docTypes.Add("Option Offer Package Signed", "4.3.2 Option Agreement/4 Option Agreement Signed Recorded Payment");
            _docTypes.Add("Option Offer Package Sent to Client", "4.3.2 Option Agreement/4 Option Agreement Signed Recorded Payment");
            _docTypes.Add("Option Compensation Check Cut", "4.3.2 Option Agreement/4 Option Agreement Signed Recorded Payment");
            _docTypes.Add("Option Documents Recorded", "4.3.2 Option Agreement/4 Option Agreement Signed Recorded Payment");
            _docTypes.Add("Option Compensation Received by Owner", "4.3.2 Option Agreement/4 Option Agreement Signed Recorded Payment");

            _docTypes.Add("Acquistion Offer Package Original", "4.3.3 Easement/3 Easement Final Sent to LO");
            _docTypes.Add("Acquistion Offer Package Updated", "4.3.3 Easement/3 Easement Final Sent to LO");
            _docTypes.Add("Acquisition Notice of Intent Package", "4.3.3 Easement/3 Easement Final Sent to LO");
            _docTypes.Add("Acquistion Offer Package Received by Owner", "4.3.3 Easement/4  Easement Signed Recorded Payment");
            _docTypes.Add("Acquisition Final Offer Package", "4.3.3 Easement/3 Easement Final Sent to LO");
            _docTypes.Add("Acquistion Offer Package Signed", "4.3.3 Easement/4  Easement Signed Recorded Payment");
            _docTypes.Add("Acquistion Offer Packet Sent to Client", "4.3.3 Easement/4  Easement Signed Recorded Payment");
            _docTypes.Add("Acquisition Compensation Check Cut", "4.3.3 Easement/4  Easement Signed Recorded Payment");
            _docTypes.Add("Acquisition Documents Recorded", "4.3.3 Easement/4  Easement Signed Recorded Payment");
            _docTypes.Add("Acquisition Compensation Received by Owner", "4.3.3 Easement/4  Easement Signed Recorded Payment");

            _docTypes.Add("Construction Damages Packet Original", "4.3.4 Restoration");
            _docTypes.Add("Construction Damages Packet Updated", "4.3.4 Restoration");
            _docTypes.Add("Construction Damages Packet Signed", "4.3.4 Restoration");
            _docTypes.Add("Construction Damages Packet Sent to Client", "4.3.4 Restoration");
            _docTypes.Add("Construction Damages Check Cut", "4.3.4 Restoration");
            _docTypes.Add("Construction Damages Compensation Received by Owner", "4.3.4 Restoration");

        */
    }
}
