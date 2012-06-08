using System;
using System.Windows.Input;
using Microsoft.Phone.Tasks;
using NanaphoNews.Data;
using NanaphoNews.Navigation;
using SimpleMvvmToolkit;

namespace NanaphoNews.ViewModels
{
    public class WebPageViewModel : ViewModelBase<WebPageViewModel>
    {
        #region Initialization and Cleanup
        /******************************
         * Initialization and Cleanup *
         ******************************/

        public WebPageViewModel()
        {
            feedItem = (FeedItem)NavigationService.NavigationArgs;
            WebPageUri = new Uri(feedItem.Link);
        }

        #endregion

        #region Notifications
        /*****************
         * Notifications *
         *****************/

        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;

        #endregion

        #region Services
        /************
         * Services *
         ************/

        #endregion

        #region Properties
        /**************
         * Properties *
         **************/

        private FeedItem feedItem;

        private Uri _WebPageUri;
        public Uri WebPageUri
        {
            get { return _WebPageUri; }
            set
            {
                if (_WebPageUri == value) return;
                _WebPageUri = value;
                NotifyPropertyChanged(m => WebPageUri);
            }
        }

        #endregion

        #region Commands
        /************
         * Commands *
         ************/

        public ICommand OpenWithBrowserCommand
        {
            get
            {
                return new DelegateCommand(
                () =>
                {
                    var task = new WebBrowserTask();
                    task.Uri = new Uri(feedItem.Link);
                    task.Show();
                }
                );
            }
        }

        public ICommand ShareThePageCommand
        {
            get
            {
                return new DelegateCommand(
                () =>
                {
                    var task = new ShareLinkTask();
                    task.LinkUri = new Uri(feedItem.Link);
                    task.Title = feedItem.Title + " - " + feedItem.FeedChannelTitle;
                    task.Show();
                }
                );
            }
        }

        #endregion

        #region Methods
        /***********
         * Methods *
         ***********/

        #endregion

        #region Completion Callbacks
        /************************
         * Completion Callbacks *
         ************************/

        #endregion

        #region Helpers
        /***********
         * Helpers *
         ***********/

        private void NotifyError(string message, Exception error)
        {
            Notify(ErrorNotice, new NotificationEventArgs<Exception>(message, error));
        }

        #endregion
    }
}
