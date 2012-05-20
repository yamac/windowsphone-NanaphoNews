using System.Runtime.Serialization;
using SimpleMvvmToolkit;
using System;

namespace NanaphoNews.Data
{
    [DataContract]
    public class FeedItem : ModelBase<FeedItem>, Helpers.IResourceDataTemplate
    {
        private static class FeedItemResourceKey
        {
            public const string NormalTemplate = "FeedItemNormalTemplate";
            public const string ImageTemplate = "FeedItemImageTemplate";
        }

        [DataMember(Name = "id")]
        public long Id { get; set; }

        [DataMember(Name = "feed_channel_id")]
        public long FeedChannelId { get; set; }

        [DataMember(Name = "feed_channel_feed_group_id")]
        public long FeedChannelFeedGroupId { get; set; }

        [DataMember(Name = "feed_channel_feed_link")]
        public string FeedChannelFeedLink { get; set; }

        [DataMember(Name = "feed_channel_author_name")]
        public string FeedChannelAuthorName { get; set; }

        [DataMember(Name = "feed_channel_link")]
        public string FeedChannelLink { get; set; }

        [DataMember(Name = "feed_channel_title")]
        public string FeedChannelTitle { get; set; }

        [DataMember(Name = "link")]
        public string Link { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "published_at")]
        public DateTime PublishedAt { get; set; }

        [DataMember(Name = "creator")]
        public string Creator { get; set; }

        [DataMember(Name = "images")]
        public string[] Images { get; set; }

        [IgnoreDataMember]
        public string ResourceKey
        {
            get
            {
                if (Images != null && Images.Length > 0)
                {
                    return FeedItemResourceKey.ImageTemplate;
                }
                return FeedItemResourceKey.NormalTemplate;
            }
        }
    }
}
