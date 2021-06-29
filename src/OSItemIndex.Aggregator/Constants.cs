namespace OSItemIndex.Aggregator
{
    public class Constants
    {
        // Our user-agent all requests use so we can easily be identified
        public const string ObserverUserAgent = "OSItemIndex + OSItemIndex.Services/1.00 + github.com/OSItemIndex + Twinki#0001";
    }

    public class Endpoints
    {
        public class OsItemIndex
        {
            public const string Api = "http://localhost:5001";
        }

        public class OsrsBox
        {
            public const string Project = "https://release-monitoring.org/api/project/32210"; // pypi project id 32210
            public const string ItemsComplete = "https://www.osrsbox.com/osrsbox-db/items-complete.json"; // static json-api
        }

        public class Realtime
        {
            public const string PriceLatest = "https://prices.runescape.wiki/api/v1/osrs/latest";
            public const string PriceFiveMin = "https://prices.runescape.wiki/api/v1/osrs/5m";
            public const string PriceOneHour = "https://prices.runescape.wiki/api/v1/osrs/1h";
        }
    }
}
