using P42.Uno.AsyncNavigation.Sample.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Uno.Extensions.Specialized;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace P42.Uno.AsyncNavigation.Sample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        async void _forwardsClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[" + Navigation.Stopwatch.ElapsedMilliseconds + "] MainPage._forwardsClick ENTER ["+Navigation.StackCount + "]");

            var page = new Page1();
            await P42.Uno.AsyncNavigation.Navigation.PushAsync(page);
            System.Diagnostics.Debug.WriteLine("["+Navigation.Stopwatch.ElapsedMilliseconds+ "] MainPage._forwardsClick EXIT  [" + Navigation.StackCount + "]");
        }
    }
}
