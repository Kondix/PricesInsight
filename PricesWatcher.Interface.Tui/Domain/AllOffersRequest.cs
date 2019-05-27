namespace PricesWatcher.Interface.Tui.Domain
{
    public class AllOffersRequest
    {
        public string hotelCode;

        public string tripType;

        public int adultsCount;

        public string airportCode;

        public int durationFrom;

        public int durationTo;

        public Pagination pagination;

        public Sort sort;
    }
}
