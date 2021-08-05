namespace OSItemIndex.Updater
{
    public class Constants
    {
        // Our user-agent all requests use so we can easily be identified
        public const string ObserverUserAgent = "OSItemIndex + OSItemIndex.Updater/1.00 + github.com/OSItemIndex + Twinki#0001";
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
            public const string Api = "https://prices.runescape.wiki/api/v1/osrs";
            public const string Latest = "/latest";
            public const string FiveMinute = "/5m";
            public const string OneHour = "/1h";
        }
    }
}
