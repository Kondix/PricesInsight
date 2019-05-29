using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PricesWatcher.Database.Domain;
using PricesWatcher.Interface.Tui.Domain;

namespace PricesWatcher.Database.Controllers
{
    public class HotelDataController
    {
        private readonly DatabaseContext _databaseContext = new DatabaseContext();

        public void AddHotelOfferPrice(Domain.Offer matchingHotelOffer, AllOffer offer)
        {
            _databaseContext.Prices.Add(new Price
            {
                Offer = matchingHotelOffer,
                DiscountPrice = offer.discountPrice,
                StandardPrice = offer.price,
                Timestamp = DateTime.UtcNow
            });
            _databaseContext.SaveChanges();
        }
        
        public IEnumerable<Price> GetOfferPrices(Domain.Offer offer)
        {
            return _databaseContext.Prices
                .Include(x => x.Offer)
                .Where(x => x.Offer.Id == offer.Id);
        }

        public Domain.Offer AddHotelOffer(Hotel matchingHotel, AllOffer offer)
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

            _databaseContext.Offers.Add(newHotelOffer);
            _databaseContext.SaveChanges();

            return newHotelOffer;
        }

        public Domain.Offer FindMatchingHotelOffer(IEnumerable<Domain.Offer> allOffersForHotel, AllOffer offer)
        {
            return allOffersForHotel
                .Where(x => x.Code == offer.offerCode)
                .Where(x => x.RoomCode == offer.roomCode)
                .Where(x => x.DepartureDateTime == DateTime.ParseExact(
                                $"{offer.departureDate} {offer.departureHours}", "yyyy-MM-dd HH:mm",
                                CultureInfo.InvariantCulture))
                .FirstOrDefault(x =>
                    x.ReturnDate == DateTime.ParseExact(offer.returnDate, "yyyy-MM-dd",
                        CultureInfo.InvariantCulture));
        }

        public IEnumerable<Domain.Offer> GetMatchingHotelOffers(Hotel hotel, DateTime departureDate)
        {
            return _databaseContext.Offers
                .Include(x => x.Hotel)
                .Where(x => x.Hotel.Id == hotel.Id)
                .Where(x => x.DepartureDateTime.Date == departureDate);
        }

        public IQueryable<Domain.Offer> GetOffersForHotel(Hotel matchingHotel)
        {
            return _databaseContext.Offers.Where(x => x.Hotel.Equals(matchingHotel));
        }

        public void AddTripAdvisorHotelRating(Interface.Tui.Domain.Offer hotel, Hotel matchingHotel)
        {
            var tripAdvisorRating = new TripAdvisorHotelRating
            {
                Hotel = matchingHotel,
                Timestamp = DateTime.UtcNow,
                TripAdvisorRating = hotel.tripAdvisorRating,
                TripAdvisorReviewsNo = hotel.tripAdvisorReviewsNo
            };

            _databaseContext.TripAdvisorHotelRatings.Add(tripAdvisorRating);
            _databaseContext.SaveChanges();
        }

        public Hotel CreateNewHotel(Interface.Tui.Domain.Offer hotel)
        {
            var matchingHotel = new Hotel { HotelCode = hotel.hotelCode, HotelStandard = hotel.hotelStandard };
            _databaseContext.Hotels.Add(matchingHotel);
            _databaseContext.SaveChanges();

            return matchingHotel;
        }

        public Hotel FindMatchingHotel(Interface.Tui.Domain.Offer hotel)
        {
            return FindMatchingHotel(hotel.hotelCode);
        }

        public Hotel FindMatchingHotel(string hotelCode)
        {
            return _databaseContext.Hotels.FirstOrDefault(x => x.HotelCode == hotelCode);
        }
    }
}
