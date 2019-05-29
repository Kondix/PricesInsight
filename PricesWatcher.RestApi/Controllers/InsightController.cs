using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PricesWatcher.Database.Controllers;

namespace PricesWatcher.RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InsightController : ControllerBase
    {
        [HttpGet("{hotelCode}/{departureDate}")]
        public ActionResult<List<(Database.Domain.Offer, List<Database.Domain.Price>)>> GetHotelOffers(string hotelCode, DateTime departureDate)
        {
            var result = new List<(Database.Domain.Offer, List<Database.Domain.Price>)>();
            var hotelDataController = new HotelDataController();
            var hotel = hotelDataController.FindMatchingHotel(hotelCode);
            if (hotel != null)
            {
                var hotelOffers = hotelDataController.GetMatchingHotelOffers(hotel, departureDate);

                foreach (var hotelOffer in hotelOffers)
                {
                    var offerPrices = hotelDataController.GetOfferPrices(hotelOffer);
                    result.Add((hotelOffer, offerPrices.ToList()));
                }
            }
            return result;
        }
    }
}