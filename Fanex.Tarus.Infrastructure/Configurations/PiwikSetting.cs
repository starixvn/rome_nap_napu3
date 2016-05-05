namespace Fanex.Tarus.Infrastructure.Configurations
{
    using System.Configuration;

    public static class PiwikSetting
    {
        public static readonly string PiwikSiteId;
        public static readonly string PiwikUrl;
        public static readonly string PiwikScript;

        static PiwikSetting()
        {
            var appSettings = ConfigurationManager.AppSettings;

            PiwikSiteId = appSettings["PiwikSiteId"];

            if (PiwikEnabled)
            {
                PiwikUrl = appSettings["PiwikUrl"];
                PiwikScript = appSettings["PiwikScript"];
            }
        }

        public static bool PiwikEnabled
        {
            get
            {
                return !string.IsNullOrWhiteSpace(PiwikSiteId);
            }
        }
    }
}
