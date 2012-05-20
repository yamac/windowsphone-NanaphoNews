using System.Collections.ObjectModel;
using NanaphoNews.Data;
using System;

namespace NanaphoNews.SampleData
{
    public class MainPageSampleData
    {
        private ObservableCollection<DummyPivot> _PivotItems = new ObservableCollection<DummyPivot>
        {
            new DummyPivot(),
            new DummyPivot()
        };
        public ObservableCollection<DummyPivot> PivotItems
        {
            get { return _PivotItems; }
        }
    }

    public class DummyPivot
    {
        public bool IsBusy
        {
            get { return true; }
        }

        public bool HasNextPage
        {
            get { return true; }
        }

        private ObservableCollection<FeedItem> _FeedItems = new ObservableCollection<FeedItem>
        {
            new FeedItem
            {
                FeedChannelAuthorName = "Author Author Author Author Author Author",
                FeedChannelTitle = "Title Title Title Title Title Title",
                Title = "Title Title Title Title Title Title",
                PublishedAt = DateTime.Parse("2555-12-25T15:55:55+0900"),
                Creator = "Creator Creator",
                Images = new string[] { "http://a2.twimg.com/profile_images/1382977966/yamac04_normal.jpg" },
            },
            new FeedItem
            {
                FeedChannelAuthorName = "Author Author",
                FeedChannelTitle = "Title Title Title Title Title Title",
                Title = "Title Title Title Title Title Title",
                PublishedAt = DateTime.Parse("2011-12-25T00:07:25+0900"),
                Creator = "Creator Creator",
            },
            /*
            new FeedItem
            {
                FeedChannelAuthorName = "Author Author",
                FeedChannelTitle = "Title Title Title Title Title Title",
                Title = "Title Title Title Title Title Title",
                PublishedAt = DateTime.Parse("2011-12-25T00:07:25+0900"),
                Creator = "Creator Creator",
                Images = new string[] { "http://a2.twimg.com/profile_images/1382977966/yamac04_normal.jpg" },
            },
            new FeedItem
            {
                FeedChannelAuthorName = "Author Author",
                FeedChannelTitle = "Title Title Title Title Title Title",
                Title = "Title Title Title Title Title Title",
                PublishedAt = DateTime.Parse("2011-12-25T00:07:25+0900"),
                Creator = "Creator Creator",
                Images = new string[] { "http://a2.twimg.com/profile_images/1382977966/yamac04_normal.jpg" },
            },
            */
        };
        public ObservableCollection<FeedItem> FeedItems
        {
            get { return _FeedItems; }
        }
    }
}
