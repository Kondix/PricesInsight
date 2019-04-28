using System.Collections.Generic;

namespace PricesWatcher.Domain
{
    public class AllOffersResponse
    {
        public List<AllOffer> offers;
        public MetaData pagination;
    }
}
