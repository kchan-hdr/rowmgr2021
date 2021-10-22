using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Austin_Costs
{
    [Table("Cost_Estimate", Schema ="Austin")]
    public class CostEstimate
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid EstimateId { get; set; }
        public string Line { get; set; }
        [Column("Design_Milestone")]
        public string DesignMilestone { get; set; }
        [Column("TCAD_Prop_ID")]
        public string PropId { get; set; }
        [Column("Acquisition_Interest")]
        public string AcquisitionInterest { get; set; }
        public int? Category { get; set; }
        [Column("Acq_Land_Cost")]
        public double? AcqLandCost { get; set; }
        [Column("Improvement_Cost")]
        public double? ImprovementCost { get; set; }
        public double? Damages { get; set; }
        public double? Oas { get; set; }
        [Column("Total_Acquisition_Cost")]
        public double? TotalAcquisitionCost { get; set; }
        [Column("Total_ROW_Cost")]
        public double? TotalRowCost { get; set; }
        [Column("Appraised_Value")]
        public double? AppraisedValue { get; set; }
    }
}
