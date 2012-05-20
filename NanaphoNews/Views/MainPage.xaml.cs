using System;
using System.Windows;
using Microsoft.Phone.Controls;
using SimpleMvvmToolkit;
using Coding4Fun.Phone.Controls;
using NanaphoNews.ViewModels;

namespace NanaphoNews.Views
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            var vm = (MainPageViewModel)DataContext;
            vm.ErrorNotice += OnErrorNotice;
        }

        public void OnErrorNotice(object sender, NotificationEventArgs<Exception> e)
        {
            System.Diagnostics.Debug.WriteLine("OnErrorNotify:" + (e.Data != null ? e.Data.ToString() : "null"));
            var toast = new ToastPrompt
            {
                Message = (e.Message != null ? e.Message : null),
                TextWrapping = TextWrapping.Wrap,
            };
            toast.Show();
        }
    }
}