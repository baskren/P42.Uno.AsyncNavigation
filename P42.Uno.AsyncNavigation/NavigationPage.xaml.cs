using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace P42.Uno.AsyncNavigation
{
    /// <summary>
    /// Host Page for Asynchronous Page Navigation
    /// </summary>
    public partial class NavigationPage : Page
    {
        #region Public Implementation


        #region Attached Properties 

        #region HasNavigationBar Property
        public static readonly DependencyProperty HasNavigationBarProperty = DependencyProperty.RegisterAttached(
            "HasNavigationBar",
            typeof(bool),
            typeof(NavigationExtensions),
            new PropertyMetadata(true, OnHasNavigationBarChanged)
        );
        static void OnHasNavigationBarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Page page && page.FindAncestor<PagePresenter>() is PagePresenter presenter)
            {
                presenter.NavBar.Visibility = page.GetHasBackButton()
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        /// <summary>
        /// Returns presence of Navigation Bar
        /// </summary>
        /// <param name="page"></param>
        /// <returns><see langword="true"/> if page has a Navigation Bar</returns>
        public static bool GetHasNavigationBar(Page page)
            => (bool)page.GetValue(HasNavigationBarProperty);
        /// <summary>
        /// Sets presence of Navigation Bar
        /// </summary>
        /// <param name="page"></param>
        /// <param name="value"></param>
        public static void SetHasNavigationBar(Page page, bool value)
            => page.SetValue(HasNavigationBarProperty, value);
        #endregion HasNavigationBar Property

        #region Title Property
        public static readonly DependencyProperty TitleProperty = DependencyProperty.RegisterAttached(
            "Title",
            typeof(object),
            typeof(NavigationExtensions),
            new PropertyMetadata(default(object), OnTitleChanged)
        );
        static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Page page && page.FindAncestor<PagePresenter>() is PagePresenter presenter)
            {
                presenter.TitleContentPresenter.Content = page.GetTitle();
            }
        }
        /// <summary>
        /// Get title displayed in page's navigation bar
        /// </summary>
        /// <param name="page"></param>
        /// <returns>title object</returns>
        public static object GetTitle(Page page)
            => (object)page.GetValue(TitleProperty);
        /// <summary>
        /// Sets title displayed in page's navigation bar
        /// </summary>
        /// <param name="page"></param>
        /// <param name="value"></param>
        public static void SetTitle(Page page, object value)
            => page.SetValue(TitleProperty, value);
        #endregion Title Property

        #region HasBackButton Property
        public static readonly DependencyProperty HasBackButtonProperty = DependencyProperty.RegisterAttached(
            "HasBackButton",
            typeof(bool),
            typeof(NavigationExtensions),
            new PropertyMetadata(true, OnHasBackButtonChanged)
        );
        static void OnHasBackButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Page page && page.FindAncestor<PagePresenter>() is PagePresenter presenter)
            {
                presenter.BackButton.Visibility = page.GetHasBackButton()
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }
        /// <summary>
        /// Gets the presence of the back button for the page
        /// </summary>
        /// <param name="page"></param>
        /// <returns>bool</returns>
        public static bool GetHasBackButton(Page page)
            => (bool)page.GetValue(HasBackButtonProperty);
        /// <summary>
        /// Sets the presence of the back button for the page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="value"></param>
        public static void SetHasBackButton(Page page, bool value)
            => page.SetValue(HasBackButtonProperty, value);
        #endregion HasBackButton Property

        #region BackButtonTitle Property
        public static readonly DependencyProperty BackButtonTitleProperty = DependencyProperty.RegisterAttached(
            "BackButtonTitle",
            typeof(string),
            typeof(NavigationExtensions),
            new PropertyMetadata(default(string), OnBackButtonTitleChanged)
        );
        static void OnBackButtonTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Page page && page.FindAncestor<PagePresenter>() is PagePresenter presenter)
            {
                presenter.BackButtonTextPresenter.Content = page.GetBackButtonTitle();
            }
        }
        /// <summary>
        /// Gets the back button's title
        /// </summary>
        /// <param name="page"></param>
        /// <returns>string</returns>
        public static string GetBackButtonTitle(Page page)
            => (string)page.GetValue(BackButtonTitleProperty);
        /// <summary>
        /// Sets the back button's title
        /// </summary>
        /// <param name="page"></param>
        /// <param name="value"></param>
        public static void SetBackButtonTitle(Page page, string value)
            => page.SetValue(BackButtonTitleProperty, value);
        #endregion BackButtonTitle Property

        #region IconColor Property
        public static readonly DependencyProperty IconColorProperty = DependencyProperty.RegisterAttached(
            "IconColor",
            typeof(Color),
            typeof(NavigationExtensions),
            new PropertyMetadata(default(Color), OnIconColorChanged)
        );
        static void OnIconColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Page page && page.FindAncestor<PagePresenter>() is PagePresenter presenter)
            {
                presenter.IconContentPresenter.Foreground = new SolidColorBrush(page.GetIconColor());
            }
        }
        /// <summary>
        /// Get's the page's icon color
        /// </summary>
        /// <param name="page"></param>
        /// <returns>Color</returns>
        public static Color GetIconColor(Page page)
            => (Color)page.GetValue(IconColorProperty);
        /// <summary>
        /// Sets the page's icon color
        /// </summary>
        /// <param name="page"></param>
        /// <param name="value"></param>
        public static void SetIconColor(Page page, Color value)
            => page.SetValue(IconColorProperty, value);
        #endregion IconColor Property

        #region Icon Property
        public static readonly DependencyProperty IconProperty = DependencyProperty.RegisterAttached(
            "Icon",
            typeof(IconElement),
            typeof(NavigationExtensions),
            new PropertyMetadata(default(IconElement), OnIconChanged)
        );
        static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Page page && page.FindAncestor<PagePresenter>() is PagePresenter presenter)
            {
                presenter.IconContentPresenter.Content = page.GetIcon();
            }
        }
        /// <summary>
        /// Gets the page's Icon
        /// </summary>
        /// <param name="page"></param>
        /// <returns>IconElement</returns>
        public static IconElement GetIcon(Page page)
            => (IconElement)page.GetValue(IconProperty);
        /// <summary>
        /// Sets the page's Icon
        /// </summary>
        /// <param name="page"></param>
        /// <param name="value"></param>
        public static void SetIcon(Page page, IconElement value)
            => page.SetValue(IconProperty, value);
        #endregion Icon Property



        #endregion

        #region Properties
        /// <summary>
        /// The current page on the Async Navigation Stack
        /// </summary>
        public Page CurrentPage => _navPanel.CurrentPage;

        /// <summary>
        /// A stopwatch that tracks the performance of PushAsync and PopAsync
        /// </summary>
        public static readonly Stopwatch Stopwatch = new Stopwatch();

        public int StackCount => CurrentPage is null ? 0 : _navPanel.BackStack.Count() + 1;
        #endregion


        #region Construction / Initialization
        public NavigationPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Page page)
                PushAsync(page);
            else if (e.Parameter is Type type && typeof(Page).IsAssignableFrom(type))
            {
                var instance = Activator.CreateInstance(type) as Page;
                PushAsync(instance);
            }
        }

        #endregion


        #region Methods
        /// <summary>
        /// Push a page onto the Root Frame (Windows.UI.Xaml.Window.Current.Content)
        /// </summary>
        /// <param name="page">a pre-instantiated page</param>
        /// <param name="pageAnimationOptions">Controls how the transition animation runs during the navigation action.</param>
        /// <returns>async Task to be awaited</returns>
        public async Task<bool> PushAsync(Page page, PageAnimationOptions pageAnimationOptions = null)
        {
            if (page is null)
                throw new ArgumentNullException("P42.Uno.AsyncNavigation.PushAsync page cannot be null.");

            if (_navPanel.Children.Any(c => c is Page p && p.Content == page))
                return false;

            //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.NavigationPage.PushAsync ENTER  page:[" + page + "]");

            Stopwatch.Reset();
            Stopwatch.Start();

            var result = await _navPanel.PushAsync(page, pageAnimationOptions);

            Stopwatch.Stop();

            //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.NavigationPage.PushAsync EXIT  page:[" + page + "]");
            return result;
        }

        /// <summary>
        /// Pop the page most recently pushed onto the AsyncNavigation stack (via AsyncNavigation.PushAsync)
        /// </summary>
        /// <param name="pageAnimationOptions">Controls how the transition animation runs during the navigation action.</param>
        /// <returns></returns>
        public async Task<bool> PopAsync(PageAnimationOptions pageAnimationOptions = null)
        {
            if (_navPanel.CanGoBack)
            {
                Stopwatch.Reset();
                Stopwatch.Start();

                //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.NavigationPage.PopAsync ENTER  page:[" + CurrentPage + "]");
                if (_navPanel.CurrentPagePresenter is PagePresenter page)
                {
                    await _navPanel.PopAsync(pageAnimationOptions);
                    //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.NavigationPage.PopAsync EXIT  page:[" + CurrentPage + "]");
                    return true;
                }
            }
            //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.NavigationPage.PopAsync EXIT  page:[" + CurrentPage + "]");
            return false;
        }
        #endregion

        /// <summary>
        /// Method called when NavBar's Back Button is pressed.  Can be intercepted to override page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public virtual async Task OnBackButtonClickedAsync(object sender, RoutedEventArgs e)
        {
            if (sender is Page page)
                await page.PopAsync();
        }



        #endregion


    }
}
