using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PricesWatcher.Database;
using PricesWatcher.Database.Domain;
using PricesWatcher.Json;

namespace PricesWatcher
{
    public class Program
    {

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
                    var hotel = new Interface.Tui.Domain.Offer {hotelCode = Settings.Default.HotelCode, hotelStandard = 4 };
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

        private static async Task HandleHotel(Interface.Tui.Domain.Offer hotel, RequestSender requestSender)
        {
            try
            {
                using (var databaseContext = new DatabaseContext())
                {
                    var matchingHotel = Database.Controllers.HotelDataController.FindMatchingHotel(databaseContext, hotel) 
                                        ?? Database.Controllers.HotelDataController.CreateNewHotel(databaseContext, hotel);

                    Database.Controllers.HotelDataController.AddTripAdvisorHotelRating(databaseContext, hotel, matchingHotel);

                    var offersResponse = await Interface.Tui.RequestDispatcher.GetAllOffers(hotel.hotelCode, requestSender);

                    var hotelOffers = Database.Controllers.HotelDataController.GetOffersForHotel(databaseContext, matchingHotel).ToList();

                    foreach (var offer in offersResponse.offers)
                    {
                        var matchingHotelOffer = Database.Controllers.HotelDataController.FindMatchingHotelOffer(hotelOffers, offer) ??
                                                 Database.Controllers.HotelDataController.AddHotelOffer(databaseContext, matchingHotel, offer);
                        Database.Controllers.HotelDataController.AddHotelOfferPrice(databaseContext, matchingHotelOffer, offer);
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
    }
}
