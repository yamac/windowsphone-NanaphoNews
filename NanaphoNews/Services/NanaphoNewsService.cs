using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using NanaphoNews.Data;
using Microsoft.Phone.Reactive;
using ICSharpCode.SharpZipLib.GZip;
using Microsoft.Phone.Notification;
using System.Runtime.Serialization;
using System.Text;

namespace NanaphoNews.Services
{
    public class NanaphoNewsService : INanaphoNewsService
    {
        private static class API
        {
#if DEBUG
            private const string Base = "http://apid.yamac.net/nanapho_news/v1.0/";
            private const string SecureBase = "https://secure.yamac.net/apid/nanapho_news/v1.0/";
#else
            private const string Base = "http://api.yamac.net/nanapho_news/v1.0/";
            private const string SecureBase = "https://secure.yamac.net/api/nanapho_news/v1.0/";
#endif
            private const string FeedBase = Base + "feed/";
            private const string DeviceBase = SecureBase + "device/";
            public const string FeedItems = FeedBase + "items";
            public const string DeviceRegister = DeviceBase + "register";
            public const string DeviceUnregister = DeviceBase + "unregister";
            public const string DeviceUpdate = DeviceBase + "update";
        }

        private static class Notification
        {
            public const string ChannelName = "NanaphoUpdates";
        }

        public NanaphoNewsService()
        {
        }

        public class GetFeedItemsResult
        {
            public FeedItem[] FeedItems { get; private set; }
            public bool HasNext { get; private set; }

            public GetFeedItemsResult(FeedItem[] items, bool hasNext)
            {
                FeedItems = items;
                HasNext = hasNext;
            }
        }

        public void GetFeedItems(int page, Action<GetFeedItemsResult, Exception> callback)
        {
            string uri = API.FeedItems;
            uri += "?rows=" + Constants.App.ItemsPerPage + "&page=" + page;
            //System.Diagnostics.Debug.WriteLine("GetFeedItems:" + uri);
            var req = WebRequest.CreateHttp(uri);
            req.UserAgent = Constants.Net.UserAgent;
            req.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";

            Observable
            .FromAsyncPattern<WebResponse>(req.BeginGetResponse, req.EndGetResponse)
            .Invoke()
            .Select<WebResponse, GetFeedItemsResult>(res =>
            {
                // ストリームを取得
                Stream stream = res.GetResponseStream();
                if (string.Equals("gzip", res.Headers[HttpRequestHeader.ContentEncoding], StringComparison.OrdinalIgnoreCase))
                {
                    stream = new GZipInputStream(stream);
                }

                // シリアライズ
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(FeedItem[]));

                // データ取得
                var items = (FeedItem[])serializer.ReadObject(stream);

                // ストリームを閉じる
                stream.Close();

                // 結果
                var result = new GetFeedItemsResult(items, (items.Count() == Constants.App.ItemsPerPage) && (page < Constants.App.MaxPage));

                return result;
            }
            )
            .ObserveOnDispatcher()
            .Subscribe(
                s =>
                {
                    callback(s, null);
                },
                e =>
                {
                    callback(null, e);
                }
            );
        }

        [DataContract]
        public class RegisterNotificationChannelResult
        {
            [DataContract]
            public class _Response
            {
                [DataMember(Name = "uuid")]
                public string Uuid { get; set; }
            }

            [DataMember(Name = "status_code")]
            public long StatusCode { get; set; }

            [DataMember(Name = "status_name")]
            public string StatusName { get; set; }

            [DataMember(Name = "response")]
            public _Response Response { get; set; }
        }

        public void RegisterNotificationChannel(string langCode, Action<RegisterNotificationChannelResult, Exception> callback)
        {
            System.Diagnostics.Debug.WriteLine("RegisterNotificationChannel");
            bool isNewChannel = false;

            HttpNotificationChannel notificationChannel;
            notificationChannel = HttpNotificationChannel.Find(Notification.ChannelName);
            if (notificationChannel == null)
            {
                isNewChannel = true;
                notificationChannel = new HttpNotificationChannel(Notification.ChannelName);
            }

            notificationChannel.ConnectionStatusChanged += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine("NotificationChannel_ConnectionStatusChanged:" + e.ConnectionStatus.ToString());
            };

            notificationChannel.ErrorOccurred += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine("NotificationChannel_ErrorOccurred:" + e.Message.ToString());
            };

            notificationChannel.HttpNotificationReceived += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine("NotificationChannel_HttpNotificationReceived:" + e.Notification.ToString());
            };

            notificationChannel.ChannelUriUpdated += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine("NotificationChannel_ChannelUriUpdated:" + e.ChannelUri.ToString());
                string uri = isNewChannel ? API.DeviceRegister : API.DeviceUpdate;
                System.Diagnostics.Debug.WriteLine(uri);
                string postDataStr;
                postDataStr = "mpns_channel_url=" + HttpUtility.UrlEncode(e.ChannelUri.ToString());
                if (langCode != null)
                {
                    postDataStr += "&language_code=" + langCode;
                }
                var req = WebRequest.CreateHttp(uri);
                req.UserAgent = Constants.Net.UserAgent;
                req.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";

                Observable
                .FromAsyncPattern<Stream>(req.BeginGetRequestStream, req.EndGetRequestStream)
                .Invoke()
                .SelectMany(stream =>
                {
                    // POSTデータ
                    var postData = Encoding.UTF8.GetBytes(postDataStr);

                    // 書き込み
                    stream.Write(postData, 0, postData.Length);

                    // ストリームを閉じる
                    stream.Close();

                    // 連結
                    return
                        Observable
                        .FromAsyncPattern<WebResponse>(req.BeginGetResponse, req.EndGetResponse)
                        .Invoke();
                })
                .Select<WebResponse, RegisterNotificationChannelResult>(res =>
                {
                    // ストリームを取得
                    Stream stream = res.GetResponseStream();
                    if (string.Equals("gzip", res.Headers[HttpRequestHeader.ContentEncoding], StringComparison.OrdinalIgnoreCase))
                    {
                        stream = new GZipInputStream(stream);
                    }

                    // シリアライズ
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RegisterNotificationChannelResult));

                    // データ取得
                    var result = (RegisterNotificationChannelResult)serializer.ReadObject(stream);

                    // ストリームを閉じる
                    stream.Close();

                    // 結果
                    return result;
                }
                )
                .ObserveOnDispatcher()
                .Subscribe(
                    s2 =>
                    {
                        callback(s2, null);
                    },
                    e2 =>
                    {
                        notificationChannel.Close();
                        callback(null, e2);
                    }
                );
            };

            //notificationChannel.ShellToastNotificationReceived += new EventHandler<NotificationEventArgs>(PushChannel_ShellToastNotificationReceived);

            if (isNewChannel)
            {
                notificationChannel.Open();
                notificationChannel.BindToShellToast();
                notificationChannel.BindToShellTile();
            }
            else
            {
                callback(null, null);
            }
        }

        [DataContract]
        public class UnregisterNotificationChannelResult
        {
            [DataMember(Name = "status_code")]
            public long StatusCode { get; set; }

            [DataMember(Name = "status_name")]
            public string StatusName { get; set; }
        }

        public void UnregisterNotificationChannel(string uuid, Action<UnregisterNotificationChannelResult, Exception> callback)
        {
            System.Diagnostics.Debug.WriteLine("UnregisterNotificationChannel");
            HttpNotificationChannel notificationChannel;
            notificationChannel = HttpNotificationChannel.Find(Notification.ChannelName);
            if (notificationChannel != null)
            {
                notificationChannel.ConnectionStatusChanged += (sender, e) =>
                {
                    System.Diagnostics.Debug.WriteLine("NotificationChannel_ConnectionStatusChanged:" + e.ConnectionStatus.ToString());
                };

                notificationChannel.ErrorOccurred += (sender, e) =>
                {
                    System.Diagnostics.Debug.WriteLine("NotificationChannel_ErrorOccurred:" + e.Message.ToString());
                };

                notificationChannel.Close();

                string uri = API.DeviceUnregister;
                System.Diagnostics.Debug.WriteLine(uri);
                string postDataStr = "uuid=" + HttpUtility.UrlEncode(uuid);
                var req = WebRequest.CreateHttp(uri);
                req.UserAgent = Constants.Net.UserAgent;
                req.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";

                Observable
                .FromAsyncPattern<Stream>(req.BeginGetRequestStream, req.EndGetRequestStream)
                .Invoke()
                .SelectMany(stream =>
                {
                    // POSTデータ
                    var postData = Encoding.UTF8.GetBytes(postDataStr);

                    // 書き込み
                    stream.Write(postData, 0, postData.Length);

                    // ストリームを閉じる
                    stream.Close();

                    // 連結
                    return
                        Observable
                        .FromAsyncPattern<WebResponse>(req.BeginGetResponse, req.EndGetResponse)
                        .Invoke();
                })
                .Select<WebResponse, UnregisterNotificationChannelResult>(res =>
                {
                    // ストリームを取得
                    Stream stream = res.GetResponseStream();
                    if (string.Equals("gzip", res.Headers[HttpRequestHeader.ContentEncoding], StringComparison.OrdinalIgnoreCase))
                    {
                        stream = new GZipInputStream(stream);
                    }

                    // シリアライズ
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(UnregisterNotificationChannelResult));

                    // データ取得
                    var result = (UnregisterNotificationChannelResult)serializer.ReadObject(stream);

                    // ストリームを閉じる
                    stream.Close();

                    // 結果
                    return result;
                }
                )
                .ObserveOnDispatcher()
                .Subscribe(
                    s2 =>
                    {
                        callback(s2, null);
                    },
                    e2 =>
                    {
                        callback(null, e2);
                    }
                );
            }
            else
            {
                callback(null, null);
            }
        }

        [DataContract]
        public class UpdateNotificationChannelResult
        {
            [DataMember(Name = "status_code")]
            public long StatusCode { get; set; }

            [DataMember(Name = "status_name")]
            public string StatusName { get; set; }
        }

        public void UpdateNotificationChannel(string uuid, string langCode, int[] channelIds, bool resetUnreads, Action<UpdateNotificationChannelResult, Exception> callback)
        {
            System.Diagnostics.Debug.WriteLine("UpdateNotificationChannel");
            string uri = API.DeviceUpdate;
            System.Diagnostics.Debug.WriteLine(uri);
            string postDataStr;
            postDataStr = "uuid=" + HttpUtility.UrlEncode(uuid);
            if (langCode != null)
            {
                postDataStr += "&language_code=" + langCode;
            }
            if (channelIds != null)
            {
                postDataStr += "&notification_channel_id=" + string.Join(",", channelIds);
            }
            if (resetUnreads)
            {
                postDataStr += "&unread_count=0";
            }
            var req = WebRequest.CreateHttp(uri);
            req.UserAgent = Constants.Net.UserAgent;
            req.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";

            Observable
            .FromAsyncPattern<Stream>(req.BeginGetRequestStream, req.EndGetRequestStream)
            .Invoke()
            .SelectMany(stream =>
            {
                // POSTデータ
                var postData = Encoding.UTF8.GetBytes(postDataStr);

                // 書き込み
                stream.Write(postData, 0, postData.Length);

                // ストリームを閉じる
                stream.Close();

                // 連結
                return
                    Observable
                    .FromAsyncPattern<WebResponse>(req.BeginGetResponse, req.EndGetResponse)
                    .Invoke();
            })
            .Select(res =>
            {
                // ストリームを取得
                Stream stream = res.GetResponseStream();
                if (string.Equals("gzip", res.Headers[HttpRequestHeader.ContentEncoding], StringComparison.OrdinalIgnoreCase))
                {
                    stream = new GZipInputStream(stream);
                }

                // シリアライズ
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(UpdateNotificationChannelResult));

                // データ取得
                var result = (UpdateNotificationChannelResult)serializer.ReadObject(stream);

                // ストリームを閉じる
                stream.Close();

                // 結果
                return result;
            })
            .ObserveOnDispatcher()
            .Subscribe(
                s2 =>
                {
                    callback(s2, null);
                },
                e2 =>
                {
                    callback(null, e2);
                }
            );
        }
    }
}