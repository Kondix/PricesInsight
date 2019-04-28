using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PricesWatcher.Database.Domain
{
    public class TripAdvisorHotelRating
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("HotelForeignKey")]
        public Hotel Hotel { get; set; }

        public float TripAdvisorRating { get; set; }
        public int TripAdvisorReviewsNo { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
