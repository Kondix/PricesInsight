using System;
using System.ComponentModel.DataAnnotations;

namespace PricesWatcher.Database.Domain
{
    public class Hotel : IEquatable<Hotel>
    {
        [Key]
        public int Id { get; set; }

        public string HotelCode { get; set; }
        public float HotelStandard { get; set; }

        public bool Equals(Hotel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(HotelCode, other.HotelCode) && HotelStandard.Equals(other.HotelStandard);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Hotel) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((HotelCode != null ? HotelCode.GetHashCode() : 0) * 397) ^ HotelStandard.GetHashCode();
            }
        }
    }
}
