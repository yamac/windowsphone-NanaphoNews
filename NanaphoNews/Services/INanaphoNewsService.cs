using System;
using NanaphoNews.Data;

namespace NanaphoNews.Services
{
    public interface INanaphoNewsService
    {
        // Feed
        void GetFeedItems(int page, Action<NanaphoNewsService.GetFeedItemsResult, Exception> callback);
    
        // Notification
        void RegisterNotificationChannel(string langCode, Action<NanaphoNewsService.RegisterNotificationChannelResult, Exception> callback);
        void UnregisterNotificationChannel(string uuid, Action<NanaphoNewsService.UnregisterNotificationChannelResult, Exception> callback);
        void UpdateNotificationChannel(string uuid, string langCode, int[] channelIds, bool resetUnreads, Action<NanaphoNewsService.UpdateNotificationChannelResult, Exception> callback);
    }
}
