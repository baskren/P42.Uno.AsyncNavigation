using P42.Uno.AsyncNavigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SampleX
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page2 : Page
    {
        public Page2()
        {
            this.InitializeComponent();
            this.SetTitle(nameof(Page2));
            this.SetBackButtonTitle("BACK");
            this.SetIcon(new SymbolIcon { Symbol = Symbol.Send });
            this.SetIconColor(Windows.UI.Colors.Green);

        }

        async void _forwardsClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] Page2._forwardsClick ENTER  [" + this.GetNavigationPage().StackCount + "]");
            var page = new Page3();
            //await P42.Uno.AsyncNavigation.Navigation.PushAsync(page);
            await this.PushAsync(page);
            System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] Page2._forwardsClick EXIT  [" + this.GetNavigationPage().StackCount + "]");
        }

        async void _backwardsClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] Page2._backwardsClick ENTER  [" + this.GetNavigationPage().StackCount + "]");
            //await P42.Uno.AsyncNavigation.Navigation.PopAsync();
            await this.PopAsync();
            System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] Page2._backwardsClick EXIT  [" + this.GetNavigationPage()?.StackCount + "]");
        }

    }
}
