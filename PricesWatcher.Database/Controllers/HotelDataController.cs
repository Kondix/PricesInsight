using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PricesWatcher.Database.Domain;
using PricesWatcher.Interface.Tui.Domain;

namespace PricesWatcher.Database.Controllers
{
    public class HotelDataController
    {
        public static void AddHotelOfferPrice(DatabaseContext databaseContext, Domain.Offer matchingHotelOffer, AllOffer offer)
        {
            databaseContext.Prices.Add(new Price
            {
                Offer = matchingHotelOffer,
                DiscountPrice = offer.discountPrice,
                StandardPrice = offer.price,
                Timestamp = DateTime.UtcNow
            });
            databaseContext.SaveChanges();
        }

        public static Domain.Offer AddHotelOffer(DatabaseContext databaseContext, Hotel matchingHotel, AllOffer offer)
        {
            var newHotelOffer = new Domain.Offer
            {
                Hotel = matchingHotel,
                DepartureDateTime = DateTime.ParseExact($"{offer.departureDate} {offer.departureHours}",
                    "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                ReturnDate = DateTime.ParseExact(offer.returnDate, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture),
                RoomCode = offer.roomCode,
                RoomName = offer.roomName,
                Board = offer.boardName,
                Code = offer.offerCode
            };

            databaseContext.Offers.Add(newHotelOffer);
            databaseContext.SaveChanges();

            return newHotelOffer;
        }

        public static Domain.Offer FindMatchingHotelOffer(IEnumerable<Domain.Offer> allOffersForHotel, AllOffer offer)
        {
            var matchingHotelOffer = allOffersForHotel
                .Where(x => x.Code == offer.offerCode)
                .Where(x => x.RoomCode == offer.roomCode)
                .Where(x => x.DepartureDateTime == DateTime.ParseExact(
                                $"{offer.departureDate} {offer.departureHours}", "yyyy-MM-dd HH:mm",
                                CultureInfo.InvariantCulture))
                .FirstOrDefault(x =>
                    x.ReturnDate == DateTime.ParseExact(offer.returnDate, "yyyy-MM-dd",
                        CultureInfo.InvariantCulture));
            return matchingHotelOffer;
        }

        public static IQueryable<Domain.Offer> GetOffersForHotel(DatabaseContext databaseContext, Hotel matchingHotel)
        {
            return databaseContext.Offers.Where(x => x.Hotel.Equals(matchingHotel));
        }

        public static void AddTripAdvisorHotelRating(DatabaseContext databaseContext, Interface.Tui.Domain.Offer hotel, Hotel matchingHotel)
        {
            var tripAdvisorRating = new TripAdvisorHotelRating
            {
                Hotel = matchingHotel,
                Timestamp = DateTime.UtcNow,
                TripAdvisorRating = hotel.tripAdvisorRating,
                TripAdvisorReviewsNo = hotel.tripAdvisorReviewsNo
            };

            databaseContext.TripAdvisorHotelRatings.Add(tripAdvisorRating);
            databaseContext.SaveChanges();
        }

        public static Hotel CreateNewHotel(DatabaseContext databaseContext, Interface.Tui.Domain.Offer hotel)
        {
            var matchingHotel = new Hotel { HotelCode = hotel.hotelCode, HotelStandard = hotel.hotelStandard };
            databaseContext.Hotels.Add(matchingHotel);
            databaseContext.SaveChanges();

            return matchingHotel;
        }

        public static Hotel FindMatchingHotel(DatabaseContext databaseContext, Interface.Tui.Domain.Offer hotel)
        {
            return databaseContext.Hotels.FirstOrDefault(x => x.HotelCode == hotel.hotelCode);
        }
    }
}
