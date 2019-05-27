using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PricesWatcher.Database.Domain
{
    public class TripAdvisorHotelRating : IEquatable<TripAdvisorHotelRating>
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("HotelForeignKey")]
        public Hotel Hotel { get; set; }

        public float TripAdvisorRating { get; set; }
        public int TripAdvisorReviewsNo { get; set; }
        public DateTime Timestamp { get; set; }

        public bool Equals(TripAdvisorHotelRating other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Hotel, other.Hotel) && TripAdvisorRating.Equals(other.TripAdvisorRating) && TripAdvisorReviewsNo == other.TripAdvisorReviewsNo;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TripAdvisorHotelRating) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Hotel != null ? Hotel.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ TripAdvisorRating.GetHashCode();
                hashCode = (hashCode * 397) ^ TripAdvisorReviewsNo;
                return hashCode;
            }
        }
    }
}
