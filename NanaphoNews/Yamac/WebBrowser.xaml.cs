using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Yamac.Controls
{
    public partial class WebBrowser : UserControl
    {
        private bool isNavigating;
        private int numNavigates;
        private bool isLoadCompleted;
        private Stack<NavigationHistory> historyGoBackStack;
        private Stack<NavigationHistory> historyGoForwardStack;
        public string UserAgent { get; set; }

        private class NavigationHistory
        {
            public Uri Uri { get; set; }
            public int NavigationCount { get; set; }
        }

        #region Public methods

        public WebBrowser()
        {
            InitializeComponent();
            TheWebBrowser.IsScriptEnabled = true;
            isNavigating = false;
            isLoadCompleted = true;
            historyGoBackStack = new Stack<NavigationHistory>();
            historyGoForwardStack = new Stack<NavigationHistory>();
        }

        public void Navigate(Uri uri)
        {
            TheWebBrowser.Navigate(uri);
        }

        public void GoBack()
        {
            try
            {
                if (!isLoadCompleted)
                {
                    //TheWebBrowser.Navigate(new Uri("javascript:;"));
                }
                isNavigating = true;
                //NavigationHistory history = historyGoBackStack.Pop();
                TheWebBrowser.Navigate(new Uri("javascript:history.back();"));
                //historyGoForwardStack.Push(history);
                //CanGoBack = (historyGoBackStack.Count >= 2 ? true : false);
                //CanGoForward = (historyGoForwardStack.Count >= 1 ? true : false);
            }
            catch
            {
            }
        }

        public void GoForward()
        {
            try
            {
                if (!isLoadCompleted)
                {
                    //TheWebBrowser.Navigate(new Uri("javascript:;"));
                }
                isNavigating = true;
                //NavigationHistory history = historyGoForwardStack.Pop();
                TheWebBrowser.Navigate(new Uri("javascript:history.forward();"));
                //historyGoBackStack.Push(history);
                //CanGoBack = (historyGoBackStack.Count >= 2 ? true : false);
                //CanGoForward = (historyGoForwardStack.Count >= 1 ? true : false);
            }
            catch
            {
            }
        }

        public void Refresh()
        {
            try
            {
                if (!isLoadCompleted)
                {
                    //TheWebBrowser.Navigate(new Uri("javascript:;"));
                }
                isNavigating = true;
                TheWebBrowser.Navigate(new Uri("javascript:history.go();"));
            }
            catch
            {
            }
        }

        #endregion

        #region Private methods

        private void setScript()
        {
            // POST動作の発生をフックするためのスクリプトを実行
            string script =
                  "var pos = location.href.indexOf(location.pathname);"
                + "var root = location.href.substr(0, pos);"
                + "var path = location.href.substr(0, pos + location.pathname.length);"
                + "pos = path.lastIndexOf('/');"
                + "var base = path.substr(0, pos);"
                + "var forms = document.getElementsByTagName('form');"
                + "for (var i = 0; i < forms.length; i++)"
                + "{"
                + "    var form = forms[i];"
                + "    if (form.method == 'post') {"
                + "        form.onsubmit = function() {"
                + "            var postData = [];"
                + "            window.external.notify('test:' + this.id);"
                + "            for (var j = 0; j < this.elements.length; j++) {"
                + "                var element = this.elements[j];"
                + "                 window.external.notify('test:' + element.name + '=' + element.value);"
                + "                if (!element.name || !element.type) continue;"
                + "                if (element.name.length == 0 || element.type == 'file') continue;"
                + "                postData.push(encodeURIComponent(element.name) + '=' + encodeURIComponent(element.value));"
                + "            }"
                + "            var action = this.action;"
                + "            if (action && action.length > 0) {"
                + "                if (action.charAt[0] == '/') {"
                + "                    action = root + action;"
                + "                } else if (action.indexOf('://') == -1) {"
                + "                    action = base + '/' + action;"
                + "                }"
                + "            } else {"
                + "                action = path;"
                + "            }"
                + "            window.external.notify('post' + '\t' + this.method + '\t' + action + '\t' + this.enctype + '\t' + postData.join('&'));"
                + "            return false;"
                + "        }"
                + "    }"
                + "}";
            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    TheWebBrowser.Navigate(new Uri("javascript:;"));
                    TheWebBrowser.InvokeScript("eval", script);
                }
                catch
                {
                    try
                    {
                        TheWebBrowser.Navigate(new Uri("javascript:;"));
                        Thread.Sleep(100);
                        TheWebBrowser.InvokeScript("eval", script);
                    }
                    catch
                    {
                    }
                }
            });
        }

        #endregion

        #region Dependency properties

        /// <summary>
        /// ブラウザーが処理を実行中かどうかを取得します。
        /// </summary>
        public bool IsBusy
        {
            get { return (bool)GetValue(IsBusyProperty); }
            set { SetValue(IsBusyProperty, value); }
        }

        public static readonly DependencyProperty IsBusyProperty =
            DependencyProperty.Register("IsBusy", typeof(bool),
            typeof(WebBrowser), new PropertyMetadata((bool)false));

        /// <summary>
        /// ブラウザーが前のページへ戻ることができるかどうかを取得します。
        /// </summary>
        public bool CanGoBack
        {
            get { return (bool)GetValue(CanGoBackProperty); }
            set { SetValue(CanGoBackProperty, value); }
        }

        public static readonly DependencyProperty CanGoBackProperty =
            DependencyProperty.Register("CanGoBack", typeof(bool),
            typeof(WebBrowser), new PropertyMetadata((bool)false));

        /// <summary>
        /// ブラウザーが次のページへ進むことができるかどうかを取得します。
        /// </summary>
        public bool CanGoForward
        {
            get { return (bool)GetValue(CanGoForwardProperty); }
            set { SetValue(CanGoForwardProperty, value); }
        }

        public static readonly DependencyProperty CanGoForwardProperty =
            DependencyProperty.Register("CanGoForward", typeof(bool),
            typeof(WebBrowser), new PropertyMetadata((bool)false));

        /// <summary>
        /// ウェブページのURIを設定・取得します。
        /// </summary>
        public Uri Source
        {
            get { return (Uri)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value);  }
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
            "Source",
            typeof(Uri),
            typeof(WebBrowser),
            new PropertyMetadata(OnSourcePropertyChanged));

        private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser source = d as WebBrowser;
            if (source != null)
            {
                source.Navigate((Uri)e.NewValue);
            }
        }

        #endregion

        #region Event handlers

        public event EventHandler<Microsoft.Phone.Controls.NavigatingEventArgs> Navigating;
        public event EventHandler<System.Windows.Navigation.NavigationEventArgs> Navigated;
        public event EventHandler<System.Windows.Navigation.NavigationFailedEventArgs> NavigationFailed;
        public event EventHandler<System.Windows.Navigation.NavigationEventArgs> LoadCompleted;

        #endregion

        #region Internal event handlers

        private void TheWebBrowser_Navigating(object sender, Microsoft.Phone.Controls.NavigatingEventArgs e)
        {
            // 通常のナビゲーションをフック
            string url = e.Uri.ToString();
            if (!isNavigating && !url.StartsWith("javascript:"))
            {
                isNavigating = true;
                numNavigates = 0;
                e.Cancel = true;
                (sender as Microsoft.Phone.Controls.WebBrowser).Navigate(e.Uri);
                return;
            }

            // ブラウザーの戻る・進むに対するヒストリーの保持件数を調整
            if (url.StartsWith("javascript:"))
            {
                // 関数名と引数を取得
                string func = url.Substring(11);
                int leftBracketPos = func.IndexOf('(');
                int rightBracketPos = func.LastIndexOf(')');
                string funcName = "";
                string funcValue = "";
                if (leftBracketPos >= 0)
                {
                    funcName = func.Substring(0, leftBracketPos).Trim();
                    funcValue = func.Substring(leftBracketPos + 1, rightBracketPos - leftBracketPos - 1).Trim();
                }
                else
                {
                    funcName = func;
                }

                // 関数名による処理分岐
                if (funcName.Equals("history.back"))
                {
                    NavigationHistory history = historyGoBackStack.Pop();
                    historyGoForwardStack.Push(history);
                    CanGoBack = (historyGoBackStack.Count >= 2 ? true : false);
                    CanGoForward = (historyGoForwardStack.Count >= 1 ? true : false);
                }
                else if (funcName.Equals("history.forward"))
                {
                    NavigationHistory history = historyGoForwardStack.Pop();
                    historyGoBackStack.Push(history);
                    CanGoBack = (historyGoBackStack.Count >= 2 ? true : false);
                    CanGoForward = (historyGoForwardStack.Count >= 1 ? true : false);
                }
                else if (funcName.Equals("history.go"))
                {
                    int funcValueInt = 0;
                    if (!string.IsNullOrEmpty(funcValue))
                    {
                        int.TryParse(funcValue, out funcValueInt);
                    }
                    if (funcValueInt == -1)
                    {
                        NavigationHistory history = historyGoBackStack.Pop();
                        historyGoForwardStack.Push(history);
                        CanGoBack = (historyGoBackStack.Count >= 2 ? true : false);
                        CanGoForward = (historyGoForwardStack.Count >= 1 ? true : false);
                    }
                    else if (funcValueInt == 1)
                    {
                        NavigationHistory history = historyGoForwardStack.Pop();
                        historyGoBackStack.Push(history);
                        CanGoBack = (historyGoBackStack.Count >= 2 ? true : false);
                        CanGoForward = (historyGoForwardStack.Count >= 1 ? true : false);
                    }
                }
                return;
            }

            IsBusy = true;
            isLoadCompleted = false;
            numNavigates++;

            // ハンドラー
            if (Navigating != null)
            {
                Navigating(sender, e);
            }
        }

        private void TheWebBrowser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            isNavigating = false;

            // ヒストリーの保持件数を調整
            if (e.NavigationMode == System.Windows.Navigation.NavigationMode.New)
            {
                bool toAddHistory = false;
                if (historyGoBackStack.Count == 0)
                {
                    toAddHistory = true;
                }
                else
                {
                    var lastHistory = historyGoBackStack.Peek();
                    if (!e.Uri.Equals(lastHistory.Uri))
                    {
                        toAddHistory = true;
                    }
                }
                if (toAddHistory)
                {
                    historyGoBackStack.Push(new NavigationHistory { Uri = e.Uri, NavigationCount = numNavigates });
                    CanGoBack = (historyGoBackStack.Count >= 2 ? true : false);
                }
            }

            // ハンドラー
            if (Navigated != null)
            {
                Navigated(sender, e);
            }
        }

        private void TheWebBrowser_NavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            // トゥームストーン状態から復帰すると発生する現象を回避
            string url = e.Uri.ToString();
            if (url.StartsWith("javascript:"))
            {
                return;
            }

            isNavigating = false;
            IsBusy = false;
            isLoadCompleted = true;

            // ヒストリーの保持件数を調整
            historyGoBackStack.Push(new NavigationHistory { Uri = e.Uri, NavigationCount = numNavigates });
            CanGoBack = (historyGoBackStack.Count >= 2 ? true : false);

            // ハンドラー
            if (NavigationFailed != null)
            {
                NavigationFailed(sender, e);
            }
        }

        private void TheWebBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            IsBusy = false;
            isLoadCompleted = true;

            // ブラウザー用の拡張JavaScriptを実行
            Thread thread = new Thread(new ThreadStart(setScript));
            thread.Start();

            // ハンドラー
            if (LoadCompleted != null)
            {
                LoadCompleted(sender, e);
            }
        }

        private void TheWebBrowser_ScriptNotify(object sender, Microsoft.Phone.Controls.NotifyEventArgs e)
        {
            // ブラウザー用の拡張JavaScriptからの通知を処理
            string[] values = e.Value.Split('\t');
            if (values.Length > 0)
            {
                // 処理種別によりフック
                string type = values[0];
                if (type.Equals("post"))
                {
                    // post: POST動作の発生をフック
                    string method = values[1];
                    string action = values[2];
                    string encType = values[3];
                    byte[] postData = Encoding.UTF8.GetBytes(values[4]);
                    isNavigating = true;
                    numNavigates = 0;
                    (sender as Microsoft.Phone.Controls.WebBrowser).Navigate(new Uri(action), postData, "Content-Type: " + encType);
                }
            }
        }

        #endregion
    }
}
