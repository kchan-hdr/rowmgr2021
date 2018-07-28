using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    public class DocType
    {
        #region static
        static Lazy<IEnumerable<DocType>> _Master = new Lazy<IEnumerable<DocType>>(DocTypeLoad, System.Threading.LazyThreadSafetyMode.PublicationOnly);
        public static IEnumerable<DocType> AllTypes => _Master.Value.OrderBy(dt => dt.DisplayOrder);
        public static IEnumerable<DocType> Types => _Master.Value.Where(dt => dt.IsDisplayed).OrderBy(dt => dt.DisplayOrder);
        public static DocType Find(string n) => _Master.Value.SingleOrDefault(dt => dt.DocTypeName.Equals(n.Trim(), StringComparison.CurrentCultureIgnoreCase));
        public static DocType Default => _Master.Value.Single(dt => dt.DocTypeName.Equals("Other"));
        #endregion

        public string DocTypeName { get; set; }
        public string FolderPath { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsDisplayed { get; set; }

        public DocType() { }

        static IEnumerable<DocType> DocTypeLoad()
        {
            using (var ctx = new ROWM_Context(DbConnection.GetConnectionString()))
            {
                return ctx.Document_Type.AsNoTracking().Select(dt => new DocType { DocTypeName = dt.DocTypeName, DisplayOrder = dt.DisplayOrder, FolderPath = dt.FolderPath, IsDisplayed = dt.IsActive })    .ToArray();
            }
        }
    }
}
