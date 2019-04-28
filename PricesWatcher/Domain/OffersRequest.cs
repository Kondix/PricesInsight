using System;
using System.Collections.Generic;

namespace PricesWatcher.Domain
{
    public class OffersRequest
    {
        public string offerType;
        public List<string> departuresCodes;
        public List<string> destinationsCodes;
        public int numberOfAdults;
        public string departureDateFrom;
        public string departureDateTo;
        public int durationFrom;
        public int durationTo;
        public string site;
        public MetaData metaData;
        public List<Filter> filters;
    }

    public enum OfferType
    {
        BY_PLANE
    }
}
