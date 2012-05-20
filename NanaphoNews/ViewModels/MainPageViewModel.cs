using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using NanaphoNews.Data;
using NanaphoNews.Navigation;
using NanaphoNews.Services;
using Microsoft.Phone.Controls;
using SimpleMvvmToolkit;
using Microsoft.Phone.Shell;
using System.Linq;

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
            ObservableCollection<MainPagePivotItemViewModel> items = new ObservableCollection<MainPagePivotItemViewModel>();
            MainPagePivotItemViewModel vm;
            vm = new MainPagePivotItemViewModel(app, navigator, service);
            vm.ErrorNotice += OnErrorNotice;
            items.Add(vm);
            PivotItems = items;
            initialized = true;
            Helpers.AppSettings.AddOrUpdateValue(Constants.AppKey.LastUpdate, DateTime.Now);
            PivotItems[0].LoadFeedItems(true, false);
            PivotItemSelectedIndex = 0;

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

        private bool initialized = false;

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

        private ObservableCollection<MainPagePivotItemViewModel> _PivotItems = new ObservableCollection<MainPagePivotItemViewModel>();
        public ObservableCollection<MainPagePivotItemViewModel> PivotItems
        {
            get { return _PivotItems; }
            set
            {
                if (_PivotItems == value) return;
                _PivotItems = value;
                NotifyPropertyChanged(m => PivotItems);
            }
        }

        private int _PivotItemSelectedIndex = -1;
        public int PivotItemSelectedIndex
        {
            get { return _PivotItemSelectedIndex; }
            set
            {
                if (_PivotItemSelectedIndex == value) return;
                _PivotItemSelectedIndex = value;
                NotifyPropertyChanged(m => PivotItemSelectedIndex);
            }
        }

        #endregion

        #region Commands
        /************
         * Commands *
         ************/

        public ICommand PivotSelectionChangedCommand
        {
            get
            {
                return new DelegateCommand<Pivot>(
                (e) =>
                {
                    (e.SelectedItem as MainPagePivotItemViewModel).LoadFeedItems(false, false);
                }
                );
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new DelegateCommand<Pivot>(
                (e) =>
                {
                    (e.SelectedItem as MainPagePivotItemViewModel).LoadFeedItems(true, false);
                }
                ,
                (e) =>
                {
                    if (IsBusy)
                    {
                        return false;
                    }
                    if (!initialized)
                    {
                        return true;
                    }
                    if (e.SelectedItem == null)
                    {
                        return false;
                    }
                    return !(e.SelectedItem as MainPagePivotItemViewModel).IsBusy;
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

        private void OnErrorNotice(object sender, NotificationEventArgs<Exception> e)
        {
            Notify(ErrorNotice, e);
        }

        #endregion
    }
}