using System.Windows;
using Coding4Fun.Phone.Controls;
using Microsoft.Phone.Controls;

namespace NanaphoNews.Views
{
    public partial class WebPage : PhoneApplicationPage
    {
        public WebPage()
        {
            InitializeComponent();
        }

        private void PhoneApplicationPage_BackKeyPress(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MainWebBrowser.CanGoBack)
            {
                MainWebBrowser.GoBack();
                e.Cancel = true;
            }
        }

        private void MainWebBrowser_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            var toast = new ToastPrompt
            {
                Title = Localization.AppResources.WebPage_Error_NavigationFailedTitle,
                Message = Localization.AppResources.WebPage_Error_NavigationFailedMessage,
                TextWrapping = TextWrapping.Wrap,
            };
            toast.Show();
        }
    }
}