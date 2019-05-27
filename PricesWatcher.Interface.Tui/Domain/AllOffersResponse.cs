using System.Collections.Generic;

namespace PricesWatcher.Interface.Tui.Domain
{
    public class AllOffersResponse
    {
        public List<AllOffer> offers;
        public MetaData pagination;
    }
}
