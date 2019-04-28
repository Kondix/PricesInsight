using System.Collections.Generic;

namespace PricesWatcher.Domain
{
    public class Offer
    {
        public string hotelCode;
        public string hotelname;
        public float hotelStandard;
        public string offerCode;
        public int duration;
        public int zoom;
        public string offerUrl;
        public List<Breadcrumb> breadcrumbs;
        public List<string> features;
        public int discountFullPrice;
        public int originalFullPrice;
        public int discountPerPersonPrice;
        public int originalPerPersonPrice;
        public string departureDate;
        public string returnDate;
        public string departureTime;
        public string departureAirport;
        public float latitude;
        public float longitude;
        public string imageUrl;
        public string boardType;
        public List<Promotion> promotions;
        public float tripAdvisorRating;
        public int tripAdvisorReviewsNo;
        public string participants;
        public string currency;
        public bool priceCheckingAvailable;
        public bool onWishlist;
        public bool soldOut;
        public bool promoted;
    }
}
