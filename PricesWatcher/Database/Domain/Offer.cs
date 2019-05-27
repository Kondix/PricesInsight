using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PricesWatcher.Database.Domain
{
    public class Offer : IEquatable<Offer>
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

        public bool Equals(Offer other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return DepartureDateTime.Equals(other.DepartureDateTime) && ReturnDate.Equals(other.ReturnDate) && string.Equals(RoomName, other.RoomName) && string.Equals(RoomCode, other.RoomCode) && string.Equals(Board, other.Board) && string.Equals(Code, other.Code);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Offer) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = DepartureDateTime.GetHashCode();
                hashCode = (hashCode * 397) ^ ReturnDate.GetHashCode();
                hashCode = (hashCode * 397) ^ RoomName.GetHashCode();
                hashCode = (hashCode * 397) ^ RoomCode.GetHashCode();
                hashCode = (hashCode * 397) ^ Board.GetHashCode();
                hashCode = (hashCode * 397) ^ Code.GetHashCode();
                return hashCode;
            }
        }
    }
}
