using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PricesWatcher.Database;
using PricesWatcher.Database.Controllers;
using PricesWatcher.Database.Domain;
using PricesWatcher.Json;

namespace PricesWatcher
{
    public class Program
    {

        public static void Main(string[] args)
        {
           switch (Settings.Default.ProgramMode)
           {
               case "4":
               {
                   DisplayOfferPrices();
                   break;
               }
               case "9":
               {
                   var hotel = new Interface.Tui.Domain.Offer {hotelCode = Settings.Default.HotelCode, hotelStandard = 4 };
                   var task = HandleHotel(hotel, new RequestSender());
                   Task.WaitAll(task);
                   break;
               }
               case "D":
               {
                   var databaseContext = new DatabaseContext();
                   databaseContext.Prices.RemoveRange(databaseContext.Prices);
                   databaseContext.Offers.RemoveRange(databaseContext.Offers);
                   databaseContext.TripAdvisorHotelRatings.RemoveRange(databaseContext.TripAdvisorHotelRatings);
                   databaseContext.SaveChanges();
                   break;
               }
               case "M":
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
            var hotelCode = Settings.Default.HotelCode;
            var departureDate = Settings.Default.DepartureDate;

            var hotelDataController = new HotelDataController();
            var hotel = hotelDataController.FindMatchingHotel(hotelCode);
            if (hotel == null)
            {
                Console.WriteLine("Could not find hotel with given hotel code.");
                return;
            }

            var hotelOffers = hotelDataController.GetMatchingHotelOffers(hotel, departureDate);

            if (!hotelOffers.Any())
            {
                Console.WriteLine("Could not find hotel offer for given hotel code and departure date.");
                return;
            }

            foreach (var offer in hotelOffers)
            {
                Console.WriteLine(
                    $"Room code: {offer.RoomCode} Board: {offer.Board} Code: {offer.Code} Name: {offer.RoomName}");

                var offerPrices = hotelDataController.GetOfferPrices(offer);

                foreach (var price in offerPrices)
                {
                    Console.WriteLine(
                        $"{price.Timestamp.Date:g} Price: {price.StandardPrice} Discounted: {price.DiscountPrice}");
                }
            }
        }

        private static async Task HandleHotel(Interface.Tui.Domain.Offer hotel, RequestSender requestSender)
        {
            try
            {
                var hotelDataController = new HotelDataController();
                var matchingHotel = hotelDataController.FindMatchingHotel(hotel) 
                                    ?? hotelDataController.CreateNewHotel(hotel);

                hotelDataController.AddTripAdvisorHotelRating(hotel, matchingHotel);

                var offersResponse = await Interface.Tui.RequestDispatcher.GetAllOffers(hotel.hotelCode, requestSender);

                var hotelOffers = hotelDataController.GetOffersForHotel(matchingHotel).ToList();

                foreach (var offer in offersResponse.offers)
                {
                    var matchingHotelOffer = hotelDataController.FindMatchingHotelOffer(hotelOffers, offer) ??
                                             hotelDataController.AddHotelOffer(matchingHotel, offer);
                    hotelDataController.AddHotelOfferPrice(matchingHotelOffer, offer);
                }

                Console.WriteLine($"Finished processing hotel: {matchingHotel.Id} {DateTime.UtcNow}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                Console.WriteLine($"Inner exception: {e.InnerException}");
            }
        }
    }
}
