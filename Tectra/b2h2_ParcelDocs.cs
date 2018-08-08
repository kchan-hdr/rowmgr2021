namespace Tectra
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class b2h2_ParcelDocs
    {
        [Key]
        public int nRecordOID { get; set; }

        public int? nGisId { get; set; }

        [StringLength(1200)]
        public string sNote { get; set; }

        [StringLength(100)]
        public string sFileName { get; set; }

        [StringLength(100)]
        public string sFileType { get; set; }

        public int? nFileLen { get; set; }

        [Column(TypeName = "image")]
        public byte[] imgFileData { get; set; }
    }
}
