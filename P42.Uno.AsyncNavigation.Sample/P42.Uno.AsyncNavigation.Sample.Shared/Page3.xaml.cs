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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace P42.Uno.AsyncNavigation.Sample.Shared
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Page3 : Page
    {
        public Page3()
        {
            this.InitializeComponent();
        }

        async void _backwardsClick(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[" + Navigation.Stopwatch.ElapsedMilliseconds + "] Page3._backwardsClick ENTER  [" + Navigation.StackCount + "]");
            await P42.Uno.AsyncNavigation.Navigation.PopAsync();
            System.Diagnostics.Debug.WriteLine("[" + Navigation.Stopwatch.ElapsedMilliseconds + "] Page3._backwardsClick EXIT  [" + Navigation.StackCount + "]");
        }
    }
}
