using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace P42.Uno.AsyncNavigation
{
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



        public static async Task<bool> PushAsync(this FrameworkElement currentElement, Page page, NavigationTransitionInfo transitionInfo = null)
        {
            if (NavigationPage(currentElement) is NavigationPage navPage)
                return await navPage.PushAsync(page, transitionInfo);
            else
                throw new ArgumentException("Current page must either a NavigationPage or a child of a NavigationPage.");
        }

        public static async Task<bool> PopAsync(this FrameworkElement currentElement)
        {
            if (NavigationPage(currentElement) is NavigationPage navPage)
                return await navPage.PopAsync();
            else
                throw new ArgumentException("Current page must either a NavigationPage or a child of a NavigationPage.");
        }

        public static NavigationPage NavigationPage(this FrameworkElement element)
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
    }
}
