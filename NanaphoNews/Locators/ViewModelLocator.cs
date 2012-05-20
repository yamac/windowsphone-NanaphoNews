using Microsoft.Phone.Controls;
using SimpleMvvmToolkit;
using NanaphoNews.Services;
using NanaphoNews.ViewModels;
using NanaphoNews.Data;

namespace NanaphoNews.Locators
{
    public class ViewModelLocator
    {
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
                PhoneApplicationFrame app = App.Current.RootVisual as PhoneApplicationFrame;
                INavigator navigator = TheNavigator;
                INanaphoNewsService service = TheNanaphoNewsService;
                return new MainPageViewModel(app, navigator, service);
            }
        }

        public PreferencesPageViewModel PreferencesPageViewModel
        {
            get
            {
                PhoneApplicationFrame app = App.Current.RootVisual as PhoneApplicationFrame;
                INavigator navigator = TheNavigator;
                INanaphoNewsService service = TheNanaphoNewsService;
                return new PreferencesPageViewModel(app, navigator, service);
            }
        }
    }
}