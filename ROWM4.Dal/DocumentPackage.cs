using System;
using System.Collections.Generic;

namespace ROWM
{
    public partial class DocumentPackage
    {
        public DocumentPackage()
        {
            Appraisal = new HashSet<Appraisal>();
            Document = new HashSet<Document>();
        }

        public Guid PackageId { get; set; }
        public string PackageName { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        public string ModifiedBy { get; set; }

        public ICollection<Appraisal> Appraisal { get; set; }
        public ICollection<Document> Document { get; set; }
    }
}
