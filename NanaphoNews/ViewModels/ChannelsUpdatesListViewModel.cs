using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using NanaphoNews.Data;
using NanaphoNews.Navigation;
using NanaphoNews.Services;
using SimpleMvvmToolkit;

namespace NanaphoNews.ViewModels
{
    public class ChannelsUpdatesListViewModel : ViewModelBase<ChannelsUpdatesListViewModel>
    {
        #region Initialization and Cleanup
        /******************************
         * Initialization and Cleanup *
         ******************************/

        public ChannelsUpdatesListViewModel() { }

        public ChannelsUpdatesListViewModel(PhoneApplicationFrame app, INavigator navigator, INanaphoNewsService service)
        {
            this.app = app;
            this.navigator = navigator;
            this.service = service;
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

        PhoneApplicationFrame app;
        INavigator navigator;
        INanaphoNewsService service;

        #endregion

        #region Properties
        /**************
         * Properties *
         **************/

        private bool read = false;
        private int page = Constants.App.BasePage;

        private bool _IsBusy = false;
        public bool IsBusy
        {
            get { return _IsBusy; }
            set
            {
                if (_IsBusy == value) return;
                _IsBusy = value;
                NotifyPropertyChanged(m => IsBusy);
            }
        }

        private bool _HasNextPage = false;
        public bool HasNextPage
        {
            get { return _HasNextPage; }
            set
            {
                if (_HasNextPage == value) return;
                _HasNextPage = value;
                NotifyPropertyChanged(m => HasNextPage);
            }
        }

        private ObservableCollection<FeedItem> _FeedItems = new ObservableCollection<FeedItem>();
        public ObservableCollection<FeedItem> FeedItems
        {
            get { return _FeedItems; }
            set
            {
                if (_FeedItems == value) return;
                _FeedItems = value;
                NotifyPropertyChanged(m => FeedItems);
            }
        }

        #endregion

        #region Commands
        /************
         * Commands *
         ************/

        public ICommand ListSelectionChangedCommand
        {
            get
            {
                return new DelegateCommand<LongListSelector>((e) =>
                {
                    if (e.SelectedItem != null)
                    {
                        NavigationService.Navigate(new Uri("/Views/WebPage.xaml", UriKind.Relative), e.SelectedItem);
                        e.SelectedItem = null;
                    }
                }
                );
            }
        }

        public ICommand ListStretchingBottomCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    LoadFeedItems(false, true);
                }
                );
            }
        }

        #endregion

        #region Methods
        /***********
         * Methods *
         ***********/

        public void LoadFeedItems(bool clear, bool next)
        {
            if (IsBusy) return;

            if (clear)
            {
                HasNextPage = true;
                FeedItems.Clear();
                page = Constants.App.BasePage;
            }
            else
            {
                if (read)
                {
                    if (!next) return;
                    if (!HasNextPage) return;
                    page++;
                } else {
                    HasNextPage = true;
                }
            }

            IsBusy = true;

            service.GetFeedItems(page, LoadFeedItemsCompleted);
        }

        #endregion

        #region Completion Callbacks
        /************************
         * Completion Callbacks *
         ************************/

        protected void LoadFeedItemsCompleted(NanaphoNewsService.GetFeedItemsResult result, Exception error)
        {
            if (!IsBusy)
            {
                return;
            }

            IsBusy = false;
            read = true;

            if (error != null)
            {
                NotifyError(Localization.AppResources.MainPage_Error_FailedToGetAllFeedGroupsAndChannels, error);
                return;
            }
            
            foreach (var item in result.FeedItems)
            {
                FeedItems.Add(item);
            }
            HasNextPage = result.HasNext;
        }

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