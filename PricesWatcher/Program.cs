using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PricesWatcher.Database;
using PricesWatcher.Database.Domain;
using PricesWatcher.DataBuilders;
using PricesWatcher.Domain;
using Offer = PricesWatcher.Database.Domain.Offer;

namespace PricesWatcher
{
    public class Program
    {
        private const string BasicDomain = "https://www.tui.pl";

        public static void Main(string[] args)
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("Type 4 for offer prices display.");
            Console.WriteLine("Type 9 for to update Side hotel.");
            Console.WriteLine("Type D for database clear.");
            Console.WriteLine("Type M for clear database duplicates.");

            while (true)
            {
               var input = Settings.Default.ProgramMode;
               if (input == "4")
               {
                   DisplayOfferPrices();
                   break;
               }

                if (input == "9")
                {
                    var hotel = new Domain.Offer {hotelCode = Settings.Default.HotelCode, hotelStandard = 4 };
                    var task = HandleHotel(hotel, new RequestSender());
                    Task.WaitAll(task);
                    break;
                }
                if (input == "D")
                {
                    var databaseContext = new DatabaseContext();
                    databaseContext.Prices.RemoveRange(databaseContext.Prices);
                    databaseContext.Offers.RemoveRange(databaseContext.Offers);
                    databaseContext.TripAdvisorHotelRatings.RemoveRange(databaseContext.TripAdvisorHotelRatings);
                    databaseContext.SaveChanges();
                    break;
                }

                if (input == "M")
                {
                    var databaseContext = new DatabaseContext();
                    var hotels = databaseContext.Hotels;

                    var pricesToDelete = new List<Price>();

                    foreach (var hotel in hotels)
                    {
                        var hotelOffers = databaseContext.Offers
                            .Include(x => x.Hotel)
                            .Where(x => x.Hotel.Id == hotel.Id);

                        foreach (var hotelOffer in hotelOffers)
                        {
                            var offerPrices = databaseContext.Prices
                                .Include(x => x.Offer)
                                .Where(x => x.Offer.Id == hotelOffer.Id);

                            var uniqueOfferPrices = offerPrices.ToHashSet();
                            pricesToDelete.AddRange(offerPrices.Except(uniqueOfferPrices));
                        }
                    }

                    Console.WriteLine($"Will remove {pricesToDelete.Count} duplicated prices.");
                    databaseContext.Prices.RemoveRange(pricesToDelete);
                    databaseContext.SaveChanges();
                    break;
                }
            }
        }

        private static void DisplayOfferPrices()
        {
            while (true)
            {
                var hotelCode = Settings.Default.HotelCode;
                var departureDate = Settings.Default.DepartureDate;

                var databaseContext = new DatabaseContext();
                var hotel = databaseContext.Hotels.FirstOrDefault(x => x.HotelCode == hotelCode);
                if (hotel == null)
                {
                    Console.WriteLine("Could not find hotel with given hotel code.");
                    continue;
                }

                var hotelOffers = databaseContext.Offers
                    .Include(x => x.Hotel)
                    .Where(x => x.Hotel.Id == hotel.Id)
                    .Where(x => x.DepartureDateTime.Date == departureDate);

                if (!hotelOffers.Any())
                {
                    Console.WriteLine("Could not find hotel offer for given hotel code and departure date.");
                    continue;
                }

                foreach (var offer in hotelOffers)
                {
                    Console.WriteLine(
                        $"Room code: {offer.RoomCode} Board: {offer.Board} Code: {offer.Code} Name: {offer.RoomName}");

                    var offerPrices = databaseContext.Prices
                        .Include(x => x.Offer)
                        .Where(x => x.Offer.Id == offer.Id);

                    foreach (var price in offerPrices)
                    {
                        Console.WriteLine(
                            $"{price.Timestamp.Date:g} Price: {price.StandardPrice} Discounted: {price.DiscountPrice}");
                    }
                }

                Console.WriteLine("Continue searching? (Y/N)");
                var decision = Console.ReadLine();
                if (decision != "Y")
                {
                    break;
                }
            }
        }

        private static async Task HandleHotel(Domain.Offer hotel, RequestSender requestSender)
        {
            try
            {
                using (var databaseContext = new DatabaseContext())
                {
                    var matchingHotel = FindMatchingHotel(databaseContext, hotel) ?? CreateNewHotel(databaseContext, hotel);

                    AddTripAdvisorHotelRating(databaseContext, hotel, matchingHotel);

                    var offersResponse = await GetAllOffers(hotel.hotelCode, requestSender);

                    var hotelOffers = GetOffersForHotel(databaseContext, matchingHotel).ToList();

                    foreach (var offer in offersResponse.offers)
                    {
                        var matchingHotelOffer = FindMatchingHotelOffer(hotelOffers, offer) ??
                                                 AddHotelOffer(databaseContext, matchingHotel, offer);
                        AddHotelOfferPrice(databaseContext, matchingHotelOffer, offer);
                    }

                    Console.WriteLine($"Finished processing hotel: {matchingHotel.Id} {DateTime.UtcNow}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                Console.WriteLine($"Inner exception: {e.InnerException}");
            }
        }

        private static void AddHotelOfferPrice(DatabaseContext databaseContext, Offer matchingHotelOffer, AllOffer offer)
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

        private static Offer AddHotelOffer(DatabaseContext databaseContext, Hotel matchingHotel, AllOffer offer)
        {
            var newHotelOffer = new Offer
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

        private static Offer FindMatchingHotelOffer(IEnumerable<Offer> allOffersForHotel, AllOffer offer)
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

        private static IQueryable<Offer> GetOffersForHotel(DatabaseContext databaseContext, Hotel matchingHotel)
        {
            return databaseContext.Offers.Where(x => x.Hotel.Equals(matchingHotel));
        }

        private static void AddTripAdvisorHotelRating(DatabaseContext databaseContext, Domain.Offer hotel, Hotel matchingHotel)
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

        private static Hotel CreateNewHotel(DatabaseContext databaseContext, Domain.Offer hotel)
        {
            var matchingHotel = new Hotel {HotelCode = hotel.hotelCode, HotelStandard = hotel.hotelStandard};
            databaseContext.Hotels.Add(matchingHotel);
            databaseContext.SaveChanges();

            return matchingHotel;
        }

        private static Hotel FindMatchingHotel(DatabaseContext databaseContext, Domain.Offer hotel)
        {
            return databaseContext.Hotels.FirstOrDefault(x => x.HotelCode == hotel.hotelCode);
        }

        private static async Task<AllOffersResponse> GetAllOffers(string hotelCode, RequestSender requestSender)
        {
            var allOffersRequest = AllOffersRequestBuilder.Build(hotelCode, "KTW");
            var allOffersResponse = await requestSender.Send(JsonConvert.SerializeObject(allOffersRequest),
                new Uri($"{BasicDomain}/hotel-cards/configurators/all-offers"));
            return JsonConvert.DeserializeObject<AllOffersResponse>(allOffersResponse);
        }

        private static async Task<OffersResponse> GetHotelOffers(RequestSender requestSender, string departure)
        {
            var offersRequest = OffersRequestBuilder.Build(new List<string> {departure});
            var offersResponse = await requestSender.Send(JsonConvert.SerializeObject(offersRequest),
                new Uri($"{BasicDomain}/search/offers"));
            return JsonConvert.DeserializeObject<OffersResponse>(offersResponse);
        }
    }
}
