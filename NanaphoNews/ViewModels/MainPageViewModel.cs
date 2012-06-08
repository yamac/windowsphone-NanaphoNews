using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using NanaphoNews.Navigation;
using NanaphoNews.Services;
using SimpleMvvmToolkit;

namespace NanaphoNews.ViewModels
{
    public class MainPageViewModel : ViewModelBase<MainPageViewModel>
    {
        #region Initialization and Cleanup
        /******************************
         * Initialization and Cleanup *
         ******************************/

        public MainPageViewModel() { }

        public MainPageViewModel(PhoneApplicationFrame app, INavigator navigator, Services.INanaphoNewsService service)
        {
            RegisterToReceiveMessages(Constants.MessageTokens.InitializeCompleted, OnInitializeCompleted);
            this.app = app;
            this.navigator = navigator;
            this.service = service;
            if (!IsInDesignMode)
            {
                DateTime lastUpdate = Helpers.AppSettings.GetValueOrDefault<DateTime>(Constants.AppKey.LastUpdate, DateTime.MinValue);
                SendMessage(Constants.MessageTokens.InitializeCompleted, new NotificationEventArgs());
            }
        }

        #endregion

        #region Notifications
        /*****************
         * Notifications *
         *****************/

        public event EventHandler<NotificationEventArgs<Exception>> ErrorNotice;

        private void OnInitializeCompleted(object sender, NotificationEventArgs e)
        {
            Helpers.AppSettings.AddOrUpdateValue(Constants.AppKey.LastUpdate, DateTime.Now);

            string uuid = Helpers.AppSettings.GetValueOrDefault<string>(Constants.AppKey.NotificationUuid, null);
            if (uuid != null)
            {
                CultureInfo uicc = Thread.CurrentThread.CurrentUICulture;
                service.UpdateNotificationChannel(uuid, Helpers.AppAttributes.Version, uicc.Name, null, true, UpdateNotificationChannelCompleted);
            }

            LoadPivotItem();

            ShellTile shellTile = ShellTile.ActiveTiles.First();
            StandardTileData shellTileData = new StandardTileData()
            {
                Title = null,
                BackgroundImage = null,
                Count = 0,
            };
            shellTile.Update(shellTileData);
        }

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

        private ChannelsUpdatesListViewModel _NanaphoChannelsUpdatesListViewModel = null;
        public ChannelsUpdatesListViewModel NanaphoChannelsUpdatesListViewModel
        {
            get { return _NanaphoChannelsUpdatesListViewModel; }
            set
            {
                if (_NanaphoChannelsUpdatesListViewModel == value) return;
                _NanaphoChannelsUpdatesListViewModel = value;
                NotifyPropertyChanged(m => NanaphoChannelsUpdatesListViewModel);
            }
        }

        #endregion

        #region Commands
        /************
         * Commands *
         ************/

        public ICommand RefreshCommand
        {
            get
            {
                return new DelegateCommand<Pivot>(
                (e) =>
                {
                    NanaphoChannelsUpdatesListViewModel.LoadFeedItems(true, false);
                }
                ,
                (e) =>
                {
                    if (IsBusy)
                    {
                        return false;
                    }
                    if (NanaphoChannelsUpdatesListViewModel == null)
                    {
                        return false;
                    }
                    return !NanaphoChannelsUpdatesListViewModel.IsBusy;
                }
                );
            }
        }

        public ICommand GotoPreferencesPageCommand
        {
            get
            {
                return new DelegateCommand(
                () =>
                {
                    NavigationService.Navigate(new Uri("/Views/PreferencesPage.xaml", UriKind.Relative));
                }
                ,
                () =>
                {
                    return !IsBusy;
                }
                );
            }
        }

        #endregion

        #region Methods
        /***********
         * Methods *
         ***********/

        private void LoadPivotItem()
        {
            if (NanaphoChannelsUpdatesListViewModel == null)
            {
                NanaphoChannelsUpdatesListViewModel =
                    new ChannelsUpdatesListViewModel(app, navigator, service);
                NanaphoChannelsUpdatesListViewModel.ErrorNotice += OnErrorNotice;
                NanaphoChannelsUpdatesListViewModel.LoadFeedItems(true, false);
            }
        }

        #endregion

        #region Completion Callbacks
        /************************
         * Completion Callbacks *
         ************************/

        void UpdateNotificationChannelCompleted(NanaphoNewsService.UpdateNotificationChannelResult result, Exception error)
        {
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

        private void OnErrorNotice(object sender, NotificationEventArgs<Exception> e)
        {
            Notify(ErrorNotice, e);
        }

        #endregion
    }
}