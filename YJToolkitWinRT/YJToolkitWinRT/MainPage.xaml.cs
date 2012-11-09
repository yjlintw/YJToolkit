using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using YJToolkit.YJToolkitCSharp.Networking;
using YJToolkit.YJToolkitCSharp.Convert;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace YJToolkitWinRT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            init();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void init()
        {
            Uri picUri = new Uri("http://upload.wikimedia.org/wikipedia/commons/d/d6/M8876_640_480.jpg");
            Stream stream = await FileConvert.UriToStream(picUri);

            Uri uri = new Uri("http://192.168.0.61/P284/gateway.php");
            var request = new HttpPostRequest(uri);
            request.Data.Add("method", "login");
            request.Data.Add("type", "email");
            request.Data.Add("info", "a@bb.cc");

            request.Data.Add("rect_eye_l", String.Format("{0},{1}", 0, 0));
            request.Data.Add("rect_eye_r", String.Format("{0},{1}", 0, 0));
            request.Data.Add("rect_nose", String.Format("{0},{1}", 0, 0));
            request.Data.Add("rect_mouth", String.Format("{0},{1}", 0, 0));
            request.Files.Add(new HttpPostFile("file", "123", stream, true));

            try
            {
                Debug.WriteLine("post");
                HttpResponse response = await Http.PostAsync(request);
                Debug.WriteLine("wait");
                Debug.WriteLine(response.Response);
            }
            catch (System.Net.WebException ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
