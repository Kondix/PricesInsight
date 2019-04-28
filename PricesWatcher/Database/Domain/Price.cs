using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PricesWatcher.Database.Domain
{
    public class Price
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("OfferForeignKey")]
        public Offer Offer { get; set; }

        public float StandardPrice { get; set; }
        public float DiscountPrice { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
