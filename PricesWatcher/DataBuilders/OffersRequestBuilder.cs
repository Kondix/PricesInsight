using System.Collections.Generic;
using PricesWatcher.Domain;

namespace PricesWatcher.DataBuilders
{
    public static class OffersRequestBuilder
    {
        public static OffersRequest Build(List<string> departureCodes)
        {
            return new OffersRequest
            {
                offerType = OfferType.BY_PLANE.ToString(),
                departuresCodes = departureCodes,
                destinationsCodes = new List<string>(),
                numberOfAdults = 2,
                departureDateFrom = "01.07.2019",
                departureDateTo = "30.09.2019",
                durationFrom = 6,
                durationTo = 8,
                site = "wypoczynek/wyniki-wyszukiwania-samolot",
                metaData = new MetaData{page = 0, pageSize = 9999, sorting = "price"},
                filters = new List<Filter>
                {
                    new Filter {filterId = "board", selectedValues = new List<string> {"GT06-AI GT06-FBP"}},
                    new Filter {filterId = "amountRange", selectedValues = new List<string> {"defaultAmountRange"}},
                    new Filter {filterId = "minHotelCategory", selectedValues = new List<string> {"4s"}},
                    new Filter {filterId = "tripAdvisorRating", selectedValues = new List<string> {"4t"}},
                    new Filter {filterId = "beach_distance", selectedValues = new List<string> {"GT03-DIBE#ST03-DIRE"}},
                    new Filter {filterId = "facilities", selectedValues = new List<string> {"GT03-BEAC#ST03-SAND"}}
                }
            };
        }
    }
}
