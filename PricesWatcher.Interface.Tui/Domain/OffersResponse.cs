using System.Collections.Generic;

namespace PricesWatcher.Interface.Tui.Domain
{
    public class OffersResponse
    {
        public MetaData pagination;
        public List<Offer> offers;
    }
}
