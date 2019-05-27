using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PricesWatcher.Database.Domain
{
    public class Price : IEquatable<Price>
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("OfferForeignKey")]
        public Offer Offer { get; set; }

        public float StandardPrice { get; set; }
        public float DiscountPrice { get; set; }
        public DateTime Timestamp { get; set; }

        public bool Equals(Price other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Offer.Equals(other.Offer) && StandardPrice.Equals(other.StandardPrice) && DiscountPrice.Equals(other.DiscountPrice);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Price) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Offer.GetHashCode();
                hashCode = (hashCode * 397) ^ StandardPrice.GetHashCode();
                hashCode = (hashCode * 397) ^ DiscountPrice.GetHashCode();
                return hashCode;
            }
        }
    }
}
