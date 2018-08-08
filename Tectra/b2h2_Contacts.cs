namespace Tectra
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class b2h2_Contacts
    {
        public int id { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Date { get; set; }

        [StringLength(255)]
        public string Agent { get; set; }

        [StringLength(255)]
        public string OwnerRep { get; set; }

        [StringLength(255)]
        public string ContactMethod { get; set; }

        public bool? LsiFollowUp { get; set; }

        [Column(TypeName = "date")]
        public DateTime? LsiFollowUpDue { get; set; }

        public bool? IpcFollowUp { get; set; }

        [Column(TypeName = "date")]
        public DateTime? IpcFollowUpDue { get; set; }

        public string Notes { get; set; }

        public string Micrositing { get; set; }

        public string PertinentProjectData { get; set; }

        public int? parcelId { get; set; }

        public int? ownerId { get; set; }
    }
}
