using Microsoft.EntityFrameworkCore;
using System;

namespace ROWM.Dal
{
    [Owned()]
    public class Compensation
    {
        public DateTimeOffset OfferDate { get; set; }
        public Double OfferAmount { get; set; }
        public string OfferNotes { get; set; }
    }
}
