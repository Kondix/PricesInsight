using PricesWatcher.Interface.Tui.Domain;

namespace PricesWatcher.Interface.Tui.DataBuilders
{
    public static class AllOffersRequestBuilder
    {
        public static AllOffersRequest Build(string hotelCode, string airportCode)
        {
            return new AllOffersRequest
            {
                adultsCount = 2,
                airportCode = airportCode,
                durationFrom = 6,
                durationTo = 8,
                hotelCode = hotelCode,
                pagination = new Pagination
                {
                    pageNo = 0,
                    pageSize = 9999
                },
                sort = new Sort
                {
                    field = "PRICE",
                    order = "ASCENDING"
                },
                tripType = "WS"
            };
        }
    }
}
