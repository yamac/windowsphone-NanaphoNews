using Microsoft.Phone.Controls;
using SimpleMvvmToolkit;
using NanaphoNews.Services;
using NanaphoNews.ViewModels;
using NanaphoNews.Data;

namespace NanaphoNews.Locators
{
    public class ViewModelLocator
    {
        private PhoneApplicationFrame TheApp
        {
            get
            {
                return App.Current.RootVisual as PhoneApplicationFrame;
            }
        }

        private INavigator _TheNavigator;
        private INavigator TheNavigator
        {
            get
            {
                if (_TheNavigator == null)
                {
                    _TheNavigator = new Navigator();
                }
                return _TheNavigator;
            }
        }

        private static INanaphoNewsService _TheNanaphoNewsService;
        private INanaphoNewsService TheNanaphoNewsService
        {
            get
            {
                if (_TheNanaphoNewsService == null)
                {
                    _TheNanaphoNewsService = new NanaphoNewsService();
                }
                return _TheNanaphoNewsService;
            }
        }

        public MainPageViewModel MainPageViewModel
        {
            get
            {
                return new MainPageViewModel(TheApp, TheNavigator, TheNanaphoNewsService);
            }
        }

        public WebPageViewModel WebPageViewModel
        {
            get
            {
                return new WebPageViewModel();
            }
        }

        public PreferencesPageViewModel PreferencesPageViewModel
        {
            get
            {
                return new PreferencesPageViewModel(TheApp, TheNavigator, TheNanaphoNewsService);
            }
        }
    }
}