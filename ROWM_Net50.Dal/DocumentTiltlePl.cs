using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ROWM.Dal
{
    /// <summary>
    /// this is a one-off picklist for document titles. ATP equates title to "type". and wants to control it.
    /// </summary>
    [Table("Document_Title_Picklist", Schema ="Austin")]
    public class DocumentTiltlePl
    {
        [Key, Column("Title_Id")]
        public int Id { get; private set; }
        [Column("Document_Type")]
        public string DocumentType { get; private set; }
        [Column("Document_Title")]
        public string DocumentTitle { get; private set; }
    }
}
