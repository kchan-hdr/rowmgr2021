namespace Tectra
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class b2h2_ContactsDocs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int contactId { get; set; }

        [StringLength(500)]
        public string FileName { get; set; }

        [StringLength(24)]
        public string FileType { get; set; }

        public int? FileLen { get; set; }

        [Column(TypeName = "image")]
        public byte[] FileData { get; set; }
    }
}
