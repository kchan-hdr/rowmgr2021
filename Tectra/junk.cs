namespace Tectra
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("junk")]
    public partial class junk
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int nGisId { get; set; }

        [StringLength(255)]
        public string sParcelId { get; set; }

        [StringLength(500)]
        public string sNote { get; set; }

        public int? nRoeNdx { get; set; }

        public bool? nRoeLandSvy { get; set; }

        public bool? nRoeEng { get; set; }

        public bool? nRoeBio { get; set; }

        public bool? nRoeCultural { get; set; }

        [Column(TypeName = "date")]
        public DateTime? dRoeDate { get; set; }

        [StringLength(255)]
        public string sOwner1 { get; set; }

        [StringLength(255)]
        public string sOwner2 { get; set; }

        [StringLength(255)]
        public string sStreet1 { get; set; }

        [StringLength(255)]
        public string sStreet2 { get; set; }

        [StringLength(255)]
        public string sCity { get; set; }

        [StringLength(255)]
        public string sState { get; set; }

        [StringLength(255)]
        public string sCounty { get; set; }

        [StringLength(255)]
        public string sZip { get; set; }

        [StringLength(255)]
        public string sPhone { get; set; }
    }
}
