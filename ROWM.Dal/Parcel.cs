using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    [Table("Parcel", Schema = "ROWM")]
    public class Parcel
    {
        /// <summary>
        /// ID might be the same as APN
        /// </summary>
        [Key]
        public string ParcelId { get; set; }

        [StringLength(800)]
        public string SitusAddress { get; set; }

        [ForeignKey("ParcelStatus"), StringLength(20)]
        public string ParcelStatusCode { get; set; }
        public virtual ParcelStatus_Master ParcelStatus { get; set; }

        [ForeignKey("RoeStatus"), StringLength(20)]
        public string RoeStatusCode { get; set; }
        public virtual RoeStatus_Master RoeStatus { get; set; }

        public double Acreage { get; set; }

        // public DbGeography Shape { get; set; }

        // ownership
        public virtual ICollection<Ownership> Owners { get; set; }

        // contact logs
        public virtual ICollection<ContactLog> ContactsLog { get; set; }

        public virtual ICollection<Document> Documents { get; set; }

        // roe 
        public Compensation InitialROEOffer { get; set; }
        public Compensation FinalROEOffer { get; set; }

        // Option
        public Compensation InitialOptionOffer { get; set; }
        public Compensation FinalOptionOffer { get; set; }

        // Easement
        public Compensation InitialEasementOffer { get; set; }
        public Compensation FinalEasementOffer { get; set; }

        // audit
        [Required]
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset LastModified { get; set; }
        [StringLength(50)]
        public string ModifiedBy { get; set; }
    }

    [ComplexType]
    public class Compensation
    {
        public DateTimeOffset OfferDate { get; set; }
        public Double OfferAmount { get; set; }
        public string OfferNotes { get; set; }
    }
}
