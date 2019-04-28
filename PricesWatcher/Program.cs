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
            Console.WriteLine("Type 1 for hotel database update.");
            Console.WriteLine("Type 2 for hotel search.");
            Console.WriteLine("Type 3 for prices difference check.");
            Console.WriteLine("Type D for database clear.");

            while (true)
            {
                var input = Console.ReadLine();
                if (input == "1")
                {
                    HandleHotelDatabaseUpdate(args).ConfigureAwait(false).GetAwaiter().GetResult();
                    break;
                }
                if (input == "2")
                {
                    HotelSearch(new DateTime(2019, 07, 13), new DateTime(2019, 07, 17));
                    break;
                }
                if (input == "3")
                {
                    var databaseContext = new DatabaseContext();
                    foreach (var offer in databaseContext.Offers)
                    {
                        var pricesForOffer = databaseContext.Prices
                            .Include(x => x.Offer)
                            .Where(x => x.Offer.Id == offer.Id);

                        var discountPrice = pricesForOffer.FirstOrDefault()?.DiscountPrice;

                        if (discountPrice != null && pricesForOffer.Any(x => x.DiscountPrice != discountPrice))
                        {
                            Console.WriteLine("Discount price differs.");
                        }
                    }
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
                Console.WriteLine("Wrong input. Try again.");
            }

        }

        private static void HotelSearch(DateTime startDate, DateTime endTime)
        {
            var databaseContext = new DatabaseContext();

            var topOffers = databaseContext.Prices
                .Where(x => x.Timestamp.Date == DateTime.UtcNow.Date)
                .Join(databaseContext.Offers.Include(x => x.Hotel), price => price.Offer.Id, offer => offer.Id, (price, offer) => new {price, offer})
                .Where(x => x.offer.DepartureDateTime > startDate)
                .Where(x => x.offer.DepartureDateTime < endTime)
                .Where(x => x.offer.Board == "All Inclusive")
                .OrderBy(x => x.price.DiscountPrice)
                .Take(100).ToList();

            foreach (var offer in topOffers)
            {
                Console.WriteLine($"Found hotel: {offer.offer.Hotel.HotelCode} on date: {offer.offer.DepartureDateTime} - {offer.offer.ReturnDate} for price: {offer.price.DiscountPrice} offer code: {offer.offer.Code}");
            }

            Console.ReadLine();
        }

        public static async Task HandleHotelDatabaseUpdate(string[] args)
        {
            var requestSender = new RequestSender();
            var hotels = await GetHotelOffers(requestSender, "KTW");
            var tasks = new List<Task>();
            foreach (var hotel in hotels.offers)
            {
                tasks.Add(HandleHotel(hotel, requestSender));
            }

            Task.WaitAll(tasks.ToArray());
        }

        private static async Task HandleHotel(Domain.Offer hotel, RequestSender requestSender)
        {
            var databaseContext = new DatabaseContext();
            try
            {
                var matchingHotel = databaseContext.Hotels.FirstOrDefault(x => x.HotelCode == hotel.hotelCode);

                if (matchingHotel == null)
                {
                    matchingHotel = new Hotel {HotelCode = hotel.hotelCode, HotelStandard = hotel.hotelStandard};
                    databaseContext.Hotels.Add(matchingHotel);
                }

                await databaseContext.TripAdvisorHotelRatings.AddAsync(new TripAdvisorHotelRating
                {
                    Hotel = matchingHotel,
                    Timestamp = DateTime.UtcNow,
                    TripAdvisorRating = hotel.tripAdvisorRating,
                    TripAdvisorReviewsNo = hotel.tripAdvisorReviewsNo
                });

                var offersResponse = await GetAllOffers(hotel.hotelCode, requestSender);
                var allOffersForHotel = databaseContext.Offers.Where(x => x.Hotel == matchingHotel);
                foreach (var offer in offersResponse.offers)
                {
                    var matchingHotelOffer = allOffersForHotel
                        .Where(x => x.RoomCode == offer.roomCode)
                        .Where(x => x.DepartureDateTime == DateTime.ParseExact(
                                        $"{offer.departureDate} {offer.departureHours}", "yyyy-MM-dd HH:mm",
                                        CultureInfo.InvariantCulture))
                        .FirstOrDefault(x =>
                            x.ReturnDate == DateTime.ParseExact(offer.returnDate, "yyyy-MM-dd",
                                CultureInfo.InvariantCulture));

                    if (matchingHotelOffer == null)
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
                        matchingHotelOffer = newHotelOffer;
                    }

                    await databaseContext.Prices.AddAsync(new Price
                    {
                        Offer = matchingHotelOffer,
                        DiscountPrice = offer.discountPrice,
                        StandardPrice = offer.price,
                        Timestamp = DateTime.UtcNow
                    });
                }

                databaseContext.SaveChanges();
                Console.WriteLine($"Finished processing hotel: {matchingHotel.Id} {DateTime.UtcNow}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                Console.WriteLine($"Inner exception: {e.InnerException}");
            }
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
