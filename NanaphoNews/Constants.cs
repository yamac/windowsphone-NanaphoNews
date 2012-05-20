using Microsoft.Phone.Info;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using NanaphoNews.Data;

namespace NanaphoNews
{
    public static class Constants
    {
        public static class App
        {
            public const int ItemsPerPage = 10;
            public const int BasePage = 1;
            public const int MaxPage = 30;
        }

        public static class AppKey
        {
            public const string LastUpdate = "LastUpdate";
            public const string NotificationConfirmation = "NotificationConfirmation";
            public const string NotificationUuid = "NotificationUuid";
        }

        public static class MessageTokens
        {
            public const string InitializeCompleted = "InitializeCompleted";
        }

        public static class Net
        {
            private static string _UserAgent;
            public static string UserAgent
            {
                get
                {
                    if (_UserAgent != null)
                    {
                        return _UserAgent;
                    }
                     _UserAgent = string.Format(
                        "Mozilla/5.0 (iPhone; U; CPU iPhone OS 4_0 fake; compatible; MSIE 9.0; Windows Phone OS 7.5; Trident/5.0; IEMobile/9.0; {0}; {1})",
                        DeviceStatus.DeviceManufacturer,
                        DeviceStatus.DeviceName
                    );
                    return _UserAgent;
                }
            }
        }
    }
}
