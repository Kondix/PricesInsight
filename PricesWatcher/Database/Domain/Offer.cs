using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PricesWatcher.Database.Domain
{
    public class Offer
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("HotelForeignKey")]
        public Hotel Hotel { get; set; }

        public DateTime DepartureDateTime { get; set; }
        public DateTime ReturnDate { get; set; }
        public string RoomName { get; set; }
        public string RoomCode { get; set; }
        public string Board { get; set; }
        public string Code { get; set; }
    }
}
