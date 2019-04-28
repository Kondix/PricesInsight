using System.Collections.Generic;

namespace PricesWatcher.Domain
{
    public class OffersResponse
    {
        public MetaData pagination;
        public List<Offer> offers;
    }
}
