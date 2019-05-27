using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PricesWatcher.Interface.Tui.DataBuilders;
using PricesWatcher.Interface.Tui.Domain;
using PricesWatcher.Json;

namespace PricesWatcher.Interface.Tui
{
    public class RequestDispatcher
    {
        private const string BasicDomain = "https://www.tui.pl";

        public static async Task<AllOffersResponse> GetAllOffers(string hotelCode, RequestSender requestSender)
        {
            var allOffersRequest = AllOffersRequestBuilder.Build(hotelCode, "KTW");
            var allOffersResponse = await requestSender.Send(JsonConvert.SerializeObject(allOffersRequest),
                new Uri($"{BasicDomain}/hotel-cards/configurators/all-offers"));
            return JsonConvert.DeserializeObject<AllOffersResponse>(allOffersResponse);
        }

        public static async Task<OffersResponse> GetHotelOffers(RequestSender requestSender, string departure)
        {
            var offersRequest = OffersRequestBuilder.Build(new List<string> { departure });
            var offersResponse = await requestSender.Send(JsonConvert.SerializeObject(offersRequest),
                new Uri($"{BasicDomain}/search/offers"));
            return JsonConvert.DeserializeObject<OffersResponse>(offersResponse);
        }
    }
}
