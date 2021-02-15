using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace P42.Uno.AsyncNavigation
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public partial class PagePresenter : Page
    {
        bool _waitingForLoad;

        public bool CanGoBack { get; private set; }


        Grid _grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = new GridLength(1, GridUnitType.Star)}
            },
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star)}
            }
        };

        internal Grid NavBar = new Grid
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Top,
            RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }
            }
        };

        internal StackPanel TitlePanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
        internal ContentPresenter TitleContentPresenter = new ContentPresenter
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(5),
            Foreground = new SolidColorBrush((Color)Application.Current.Resources["SystemAccentColor"])
        };
        internal ContentPresenter IconContentPresenter = new ContentPresenter
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };


        internal Button BackButton = new Button
        {
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Center,
            Visibility = Visibility.Collapsed,
            BorderThickness = new Thickness(0),
            Background = new SolidColorBrush(Colors.Transparent),
        };

        internal ContentPresenter BackButtonTextPresenter = new ContentPresenter();

        internal Page Page;

        public PagePresenter()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            _waitingForLoad = true;

            Background = new SolidColorBrush((Color)Application.Current.Resources["SystemAltHighColor"]);

            //RelativePanel.SetAlignHorizontalCenterWithPanel(TitleContentPresenter, true);
            //RelativePanel.SetAlignVerticalCenterWithPanel(TitleContentPresenter, true);

            if (Application.Current.Resources["TitleTextBlockStyle"] is Windows.UI.Text.FontStyle headerFontStyle)
                TitleContentPresenter.FontStyle = headerFontStyle;
            else
            {
                TitleContentPresenter.FontWeight = Windows.UI.Text.FontWeights.Bold;
                TitleContentPresenter.FontSize = 20;
            }

            TitlePanel.Children.Add(IconContentPresenter);
            TitlePanel.Children.Add(TitleContentPresenter);
            NavBar.Children.Add(TitlePanel);

            //RelativePanel.SetLeftOf(IconContentPresenter, TitleContentPresenter);
            //RelativePanel.SetAlignVerticalCenterWithPanel(IconContentPresenter, true);
            //NavBar.Children.Add(IconContentPresenter);

            BackButton.Content = new StackPanel
            {
                Spacing = 5,
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new SymbolIcon { Symbol = Symbol.Back },
                    BackButtonTextPresenter
                }
            };
            BackButton.Click += OnBackButtonClickedAsync;
            RelativePanel.SetAlignVerticalCenterWithPanel(BackButton, true);
            NavBar.Children.Add(BackButton);


            _grid.Children.Add(NavBar);

            Content = _grid;

            KeyboardAccelerator GoBack = new KeyboardAccelerator();
            GoBack.Key = VirtualKey.GoBack;
            GoBack.Invoked += BackInvoked;
            KeyboardAccelerator AltLeft = new KeyboardAccelerator();
            AltLeft.Key = VirtualKey.Left;
            AltLeft.Invoked += BackInvoked;
            this.KeyboardAccelerators.Add(GoBack);
            this.KeyboardAccelerators.Add(AltLeft);
            // ALT routes here
            AltLeft.Modifiers = VirtualKeyModifiers.Menu;

        }

        async void BackInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            if (this.FindAncestor<NavigationPage>() is NavigationPage navPage)
                await navPage.OnBackButtonClickedAsync(Page, null);
            args.Handled = true;
        }


        async void OnBackButtonClickedAsync(object sender, RoutedEventArgs e)
        {
            if (this.FindAncestor<NavigationPage>() is NavigationPage navPage)
                await navPage.OnBackButtonClickedAsync(Page, e);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.PagePresenter.OnUnloaded  ENTER  [" + Content + "]  tcs[" + this.GetLoadedTaskCompletedSource() + "]");
            if (this.GetUnloadTaskCompletionSource() is TaskCompletionSource<bool> tcs)
            {
                //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.PagePresenter.OnUnloaded  A1  [" + Content + "]  tcs[" + this.GetLoadedTaskCompletedSource() + "]");
                this.SetUnloadTaskCompletionSource(null);
                //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.PagePresenter.OnUnloaded  A2  [" + Content + "]  tcs[" + this.GetLoadedTaskCompletedSource() + "]");
                tcs.SetResult(true);
                //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.PagePresenter.OnUnloaded  A3  [" + Content + "]  tcs[" + this.GetLoadedTaskCompletedSource() + "]");
            }
            //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.PagePresenter.OnUnloaded  EXIT  [" + Content + "]  tcs[" + this.GetLoadedTaskCompletedSource() + "]");
        }

        public PagePresenter(Page page, bool canGoBack) : this()
        {
            Page = page;
            CanGoBack = canGoBack;

            if (page.GetBackButtonTitle() != default)
                BackButtonTextPresenter.Content = page.GetBackButtonTitle();

            if (page.GetIconColor() != default)
                IconContentPresenter.Foreground = new SolidColorBrush(page.GetIconColor());

            if (page.GetIcon() != default)
                IconContentPresenter.Content = page.GetIcon();


            BackButton.Visibility = CanGoBack && page.GetHasBackButton()
                ? Visibility.Visible
                : Visibility.Collapsed;
            if (page.GetTitle() != default)
                TitleContentPresenter.Content = page.GetTitle();

            NavBar.Visibility = page.GetHasNavigationBar()
                ? Visibility.Visible
                : Visibility.Collapsed;

            Grid.SetRow(page, 1);
            _grid.Children.Add(page);

        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.PagePresenter.OnLoaded  ENTER  [" + Content+"]  waiting["+_waitingForLoad+"]  tcs["+this.GetLoadedTaskCompletedSource()+"]");
            if (_waitingForLoad && this.GetLoadedTaskCompletedSource() is TaskCompletionSource<bool> tcs)
            {
                //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.PagePresenter.OnLoaded A1");
                this.SetLoadedTaskCompletedSource(null);
                //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.PagePresenter.OnLoaded A2");
                tcs.SetResult(true);
                //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.PagePresenter.OnLoaded A3");
            }
            //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.PagePresenter.OnLoaded  EXIT  [" + Content + "]");
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.PagePresenter.ArrangeOverride  ENTER  [" + Content + "]  tcs["+this.GetArrangedTaskCompletionSource()+"]");
            var result = finalSize;
            if (Parent != null)
                result =  base.ArrangeOverride(finalSize);

            if (this.GetArrangedTaskCompletionSource() is TaskCompletionSource<bool> tcs)
            {
                //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.PagePresenter.ArrangeOverride A1");
                this.SetArrangedTaskCompletionSource(null);
                //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.PagePresenter.ArrangeOverride A2");
                tcs.SetResult(true);
                //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.PagePresenter.ArrangeOverride A3");
            }
            //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.PagePresenter.ArrangeOverride  EXIT  [" + Content + "]");
            return result;
        }
    }
}
