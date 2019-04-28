using System.ComponentModel.DataAnnotations;

namespace PricesWatcher.Database.Domain
{
    public class Hotel
    {
        [Key]
        public int Id { get; set; }

        public string HotelCode { get; set; }
        public float HotelStandard { get; set; }
    }
}
