using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace P42.Uno.AsyncNavigation
{
    [System.ComponentModel.Bindable(System.ComponentModel.BindableSupport.Yes)]
    public static class NavigationExtensions
    {
        #region LoadedTaskCompletionSource Property
        internal static readonly DependencyProperty LoadedTaskCompletedSourceProperty = DependencyProperty.RegisterAttached(
            "LoadedTaskCompletionSource",
            typeof(TaskCompletionSource<bool>),
            typeof(NavigationExtensions),
            new PropertyMetadata(null)
        );
        internal static TaskCompletionSource<bool> GetLoadedTaskCompletedSource(this PagePresenter element)
            => (TaskCompletionSource<bool>)element.GetValue(LoadedTaskCompletedSourceProperty);
        internal static void SetLoadedTaskCompletedSource(this PagePresenter element, TaskCompletionSource<bool> value)
            => element.SetValue(LoadedTaskCompletedSourceProperty, value);
        #endregion LoadedTaskCompletionSource Property

        #region UnloadTaskCompletionSource Property
        internal static readonly DependencyProperty UnloadTaskCompletionSourceProperty = DependencyProperty.RegisterAttached(
            "UnloadTaskCompletionSource",
            typeof(TaskCompletionSource<bool>),
            typeof(NavigationExtensions),
            new PropertyMetadata(null)
        );
        internal static TaskCompletionSource<bool> GetUnloadTaskCompletionSource(this PagePresenter element)
            => (TaskCompletionSource<bool>)element.GetValue(UnloadTaskCompletionSourceProperty);
        internal static void SetUnloadTaskCompletionSource(this PagePresenter element, TaskCompletionSource<bool> value)
            => element.SetValue(UnloadTaskCompletionSourceProperty, value);
        #endregion UnloadTaskCompletionSource Property

        #region ArrangedTaskCompletionSource Property
        internal static readonly DependencyProperty ArrangedTaskCompletionSourceProperty = DependencyProperty.RegisterAttached(
            "ArrangedTaskCompletionSource",
            typeof(TaskCompletionSource<bool>),
            typeof(NavigationExtensions),
            new PropertyMetadata(null)
        );
        internal static TaskCompletionSource<bool> GetArrangedTaskCompletionSource(this PagePresenter element)
            => (TaskCompletionSource<bool>)element.GetValue(ArrangedTaskCompletionSourceProperty);
        internal static void SetArrangedTaskCompletionSource(this PagePresenter element, TaskCompletionSource<bool> value)
            => element.SetValue(ArrangedTaskCompletionSourceProperty, value);
        #endregion ArrangedTaskCompletionSource Property

        #region EntranceAnimationOptions Property
        public static readonly DependencyProperty EntranceAnimationOptionsProperty = DependencyProperty.RegisterAttached(
            "EntranceAnimationOptions",
            typeof(PageAnimationOptions),
            typeof(NavigationExtensions),
            new PropertyMetadata(default(PageAnimationOptions))
        );
        public static PageAnimationOptions GetEntranceAnimationOptions(this PagePresenter element)
            => (PageAnimationOptions)element.GetValue(EntranceAnimationOptionsProperty);
        public static void SetEntranceAnimationOptions(this PagePresenter element, PageAnimationOptions value)
            => element.SetValue(EntranceAnimationOptionsProperty, value);
        #endregion EntranceAnimationOptions Property

        #region NavigationPanel Property
        internal static readonly DependencyProperty NavigationPanelProperty = DependencyProperty.RegisterAttached(
            nameof(NavigationPanel),
            typeof(NavigationPanel),
            typeof(NavigationExtensions),
            new PropertyMetadata(default(NavigationPanel), OnNavigationPanelChanged)
        );
        static void OnNavigationPanelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Page page)
            {
                //throw new NotImplementedException();
            }
        }
        internal static NavigationPanel GetNavigationPanel(this Page page)
            => (NavigationPanel)page.GetValue(NavigationPanelProperty);
        internal static void SetNavigationPanel(this Page page, NavigationPanel value)
            => page.SetValue(NavigationPanelProperty, value);
        #endregion NavigationPanel Property

        #region HasNavigationBar Property

        /// <summary>
        /// Returns presence of Navigation Bar
        /// </summary>
        /// <param name="page"></param>
        /// <returns><see langword="true"/> if page has a Navigation Bar</returns>
        public static bool GetHasNavigationBar(this Page page)
            => (bool)page.GetValue(NavigationPage.HasNavigationBarProperty);
        /// <summary>
        /// Sets presence of Navigation Bar
        /// </summary>
        /// <param name="page"></param>
        /// <param name="value"></param>
        public static void SetHasNavigationBar(this Page page, bool value)
            => page.SetValue(NavigationPage.HasNavigationBarProperty, value);
        #endregion HasNavigationBar Property

        #region Title Property
        /// <summary>
        /// Get title displayed in page's navigation bar
        /// </summary>
        /// <param name="page"></param>
        /// <returns>title object</returns>
        public static object GetTitle(this Page page)
            => (object)page.GetValue(NavigationPage.TitleProperty);
        /// <summary>
        /// Sets title displayed in page's navigation bar
        /// </summary>
        /// <param name="page"></param>
        /// <param name="value"></param>
        public static void SetTitle(this Page page, object value)
            => page.SetValue(NavigationPage.TitleProperty, value);
        #endregion Title Property

        #region HasBackButton Property
        /// <summary>
        /// Gets the presence of the back button for the page
        /// </summary>
        /// <param name="page"></param>
        /// <returns>bool</returns>
        public static bool GetHasBackButton(this Page page)
            => (bool)page.GetValue(NavigationPage.HasBackButtonProperty);
        /// <summary>
        /// Sets the presence of the back button for the page
        /// </summary>
        /// <param name="page"></param>
        /// <param name="value"></param>
        public static void SetHasBackButton(this Page page, bool value)
            => page.SetValue(NavigationPage.HasBackButtonProperty, value);
        #endregion HasBackButton Property

        #region BackButtonTitle Property
        /// <summary>
        /// Gets the back button's title
        /// </summary>
        /// <param name="page"></param>
        /// <returns>string</returns>
        public static string GetBackButtonTitle(this Page page)
            => (string)page.GetValue(NavigationPage.BackButtonTitleProperty);
        /// <summary>
        /// Sets the back button's title
        /// </summary>
        /// <param name="page"></param>
        /// <param name="value"></param>
        public static void SetBackButtonTitle(this Page page, string value)
            => page.SetValue(NavigationPage.BackButtonTitleProperty, value);
        #endregion BackButtonTitle Property

        #region IconColor Property
        /// <summary>
        /// Get's the page's icon color
        /// </summary>
        /// <param name="page"></param>
        /// <returns>Color</returns>
        public static Color GetIconColor(this Page page)
            => (Color)page.GetValue(NavigationPage.IconColorProperty);
        /// <summary>
        /// Sets the page's icon color
        /// </summary>
        /// <param name="page"></param>
        /// <param name="value"></param>
        public static void SetIconColor(this Page page, Color value)
            => page.SetValue(NavigationPage.IconColorProperty, value);
        #endregion IconColor Property

        #region Icon Property
        /// <summary>
        /// Gets the page's Icon
        /// </summary>
        /// <param name="page"></param>
        /// <returns>IconElement</returns>
        public static IconElement GetIcon(this Page page)
            => (IconElement)page.GetValue(NavigationPage.IconProperty);
        /// <summary>
        /// Sets the page's Icon
        /// </summary>
        /// <param name="page"></param>
        /// <param name="value"></param>
        public static void SetIcon(this Page page, IconElement value)
            => page.SetValue(NavigationPage.IconProperty, value);
        #endregion Icon Property



        public static async Task<bool> PushAsync(this FrameworkElement currentElement, Page page, PageAnimationOptions pageAnimationOptions = null)
        {
            await Task.Delay(5);
            if (GetNavigationPage(currentElement) is NavigationPage navPage)
                return await navPage.PushAsync(page, pageAnimationOptions);
            else
                throw new ArgumentException("Current page must either a NavigationPage or a child of a NavigationPage.");
        }

        public static async Task<bool> PopAsync(this FrameworkElement currentElement, PageAnimationOptions pageAnimationOptions = null)
        {
            if (GetNavigationPage(currentElement) is NavigationPage navPage)
                return await navPage.PopAsync(pageAnimationOptions);
            else
                throw new ArgumentException("Current page must either a NavigationPage or a child of a NavigationPage.");
        }

        public static NavigationPage GetNavigationPage(this FrameworkElement element)
        {
            if (element is NavigationPage x)
                return x;
            var parent = element.Parent as FrameworkElement;
            while (parent != null)
            {
                if (parent is NavigationPage navPage)
                    return navPage;
                parent = parent.Parent as FrameworkElement;
            }
            return null;
        }

        internal static T FindAncestor<T>(this FrameworkElement element) where T:FrameworkElement
        {
            while (element != null)
            {
                element = element.Parent as FrameworkElement;
                if (element is T)
                    return element as T;
            }
            return null;
        }
    }
}
