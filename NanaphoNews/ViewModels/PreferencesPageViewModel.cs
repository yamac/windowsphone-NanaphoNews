using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using NanaphoNews.Services;
using SimpleMvvmToolkit;

namespace NanaphoNews.ViewModels
{
    public class PreferencesPageViewModel : ViewModelBase<PreferencesPageViewModel>
    {
        #region Initialization and Cleanup
        /******************************
         * Initialization and Cleanup *
         ******************************/

        public PreferencesPageViewModel() { }

        public PreferencesPageViewModel(PhoneApplicationFrame app, INavigator navigator, Services.INanaphoNewsService service)
        {
            this.app = app;
            this.navigator = navigator;
            this.service = service;
            string uuid = Helpers.AppSettings.GetValueOrDefault<string>(Constants.AppKey.NotificationUuid, null);
            if (uuid != null)
            {
                IsReceiveNotification = true;
            }
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

        private bool _IsReceiveNotification = false;
        public bool IsReceiveNotification
        {
            get { return _IsReceiveNotification; }
            set
            {
                if (_IsReceiveNotification == value) return;
                _IsReceiveNotification = value;
                NotifyPropertyChanged(m => IsReceiveNotification);
            }
        }

        #endregion

        #region Commands
        /************
         * Commands *
         ************/

        public ICommand TweetAboutTheAppTapCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    ShareStatusTask shareStatusTask = new ShareStatusTask();
                    shareStatusTask.Status = Localization.AppResources.PreferencesPage_TwitterAccountForSupportValue + " ";
                    shareStatusTask.Show();
                }
                );
            }
        }

        public ICommand ReviewTheAppTapCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    MarketplaceDetailTask task = new MarketplaceDetailTask();
                    task.ContentIdentifier = null;
                    task.ContentType = MarketplaceContentType.Applications;
                    task.Show();
                }
                );
            }
        }

        public ICommand ReceiveNotificationCheckedCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    var isConfirmed = Helpers.AppSettings.GetValueOrDefault<bool>(Constants.AppKey.NotificationConfirmation, false);
                    if (isConfirmed)
                    {
                    }
                    else
                    {
                        var result =
                            MessageBox.Show(
                                Localization.AppResources.PreferencesPage_Notification_ReceiveNotification_ConfirmationText,
                                Localization.AppResources.PreferencesPage_Notification_ReceiveNotification_ConfirmationCaption,
                                MessageBoxButton.OKCancel
                            );
                        if (result.Equals(MessageBoxResult.OK))
                        {
                            isConfirmed = true;
                        }
                        else
                        {
                            IsReceiveNotification = false;
                        }
                    }
                    if (isConfirmed)
                    {
                        RegisterNotificationChannel();
                    }
                }
                );
            }
        }

        public ICommand ReceiveNotificationUncheckedCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    UnregisterNotificationChannel();
                }
                );
            }
        }

        #endregion

        #region Methods
        /***********
         * Methods *
         ***********/

        public void RegisterNotificationChannel()
        {
            IsBusy = true;
            CultureInfo uicc = Thread.CurrentThread.CurrentUICulture;
            service.RegisterNotificationChannel(Helpers.AppAttributes.Version, uicc.Name, RegisterNotificationChannelCompleted);
        }

        public void UnregisterNotificationChannel()
        {
            string uuid = Helpers.AppSettings.GetValueOrDefault<string>(Constants.AppKey.NotificationUuid, null);
            if (uuid != null)
            {
                IsBusy = true;
                service.UnregisterNotificationChannel(uuid, UnregisterNotificationChannelCompleted);
            }
        }

        #endregion

        #region Completion Callbacks
        /************************
         * Completion Callbacks *
         ************************/

        private void RegisterNotificationChannelCompleted(NanaphoNewsService.RegisterNotificationChannelResult result, Exception error)
        {
            if (!IsBusy)
            {
                return;
            }

            IsBusy = false;

            if (error != null)
            {
                IsReceiveNotification = false;
                NotifyError(Localization.AppResources.PreferencesPage_Error_NotificationRegistrationFailed, error);
                return;
            }

            Helpers.AppSettings.AddOrUpdateValue(Constants.AppKey.NotificationConfirmation, true);
            if (result != null)
            {
                Helpers.AppSettings.AddOrUpdateValue(Constants.AppKey.NotificationUuid, result.Response.Uuid);
            }
        }

        private void UnregisterNotificationChannelCompleted(NanaphoNewsService.UnregisterNotificationChannelResult result, Exception error)
        {
            if (!IsBusy)
            {
                return;
            }

            IsBusy = false;

            if (error != null)
            {
                NotifyError(Localization.AppResources.PreferencesPage_Error_NotificationRegistrationFailed, error);
                return;
            }

            Helpers.AppSettings.Remove(Constants.AppKey.NotificationUuid);
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
