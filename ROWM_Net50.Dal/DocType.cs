using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json.Serialization;

namespace ROWM.Dal
{
    public class DocType
    {
        public string DocTypeName { get; set; }
        public string Description { get; set; }
        public string FolderPath { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsDisplayed { get; set; }
        [JsonIgnore]
        public Parcel_Status Milestone { get; set; }

        public DocType() { }
    }

    public class DocTypes : Collection<DocType>
    {
        #region static
        Lazy<IEnumerable<DocType>> _Master;
        public IEnumerable<DocType> AllTypes => _Master.Value.OrderBy(dt => dt.DisplayOrder);
        public IEnumerable<DocType> Types => _Master.Value.Where(dt => dt.IsDisplayed).OrderBy(dt => dt.DisplayOrder);
        public DocType Find(string n) => _Master.Value.SingleOrDefault(dt => dt.DocTypeName.Equals(n.Trim(), StringComparison.CurrentCultureIgnoreCase));
        public DocType Default => _Master.Value.Single(dt => dt.DocTypeName.Equals("Other"));
        #endregion

        readonly ROWM_Context _ctx;

        public DocTypes(ROWM_Context c)
        {
            _ctx = c;

            _Master = new Lazy<IEnumerable<DocType>>(DocTypeLoad, System.Threading.LazyThreadSafetyMode.PublicationOnly);
        }

        IEnumerable<DocType> DocTypeLoad() => _ctx.Document_Type.AsNoTracking().Select(dt => new DocType { DocTypeName = dt.DocTypeName, Description = dt.Description, DisplayOrder = dt.DisplayOrder, FolderPath = dt.FolderPath, IsDisplayed = dt.IsActive, Milestone = dt.Milestone }).ToArray();
    }
}
