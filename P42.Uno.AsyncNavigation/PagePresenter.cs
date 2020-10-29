using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

namespace P42.Uno.AsyncNavigation
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public partial class PagePresenter : Page
    {
        bool _waitingForLoad;

        public PagePresenter()
        {
            //Unloaded += PagePresenter_Unloaded;
            Loaded += PagePresenter_Loaded;
            _waitingForLoad = true;
        }

        public PagePresenter(Page page) : this()
        {
            System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] PagePresenter.ctor ENTER page:["+page.GetType()+"]");
            Content = page;
            System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] PagePresenter.ctor EXIT page:[" + page.GetType() + "]");
        }

        private void PagePresenter_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] PagePresenter.Loaded  ENTER content:[" + Content.GetType() + "]");
            if (_waitingForLoad && this.GetLoadedTaskCompletedSource() is TaskCompletionSource<bool> tcs)
            {
                this.SetLoadedTaskCompletedSource(null);
                tcs.SetResult(true);
            }
            System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] PagePresenter.Loaded  EXIT content:[" + Content.GetType() + "]");
        }

        /*
        private void PagePresenter_Unloaded(object sender, RoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("[" + Navigation.Stopwatch.ElapsedMilliseconds + "] PagePresenter.Unloaded");
            if (Navigation.CurrentPage == Content)
            {
                Navigation.PopCurrentMetaPage();
            }
            if (Content is Page page && page.GetUnloadTaskCompletionSource() is TaskCompletionSource<bool> tcs)
            {
                page.SetUnloadTaskCompletionSource(null);
                tcs.SetResult(true);
            }
            Content = null;
        }*/

        protected override Size ArrangeOverride(Size finalSize)
        {
            System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] PagePresenter.ArrangeOverride("+finalSize+")  ENTER content:["+Content.GetType()+"]");
            var result =  base.ArrangeOverride(finalSize);
            if (this.GetArrangedTaskCompletionSource() is TaskCompletionSource<bool> tcs)
            {
                this.SetArrangedTaskCompletionSource(null);
                tcs.SetResult(true);
            }
            System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] PagePresenter.ArrangeOverride(" + finalSize + ")  EXIT content:[" + Content.GetType() + "]");
            return result;
        }

        /*
        protected override void OnBringIntoViewRequested(BringIntoViewRequestedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("[" + Navigation.Stopwatch.ElapsedMilliseconds + "] PagePresenter.OnBringIntoViewRequested[" + e.TargetElement+"] ENTER");
            base.OnBringIntoViewRequested(e);
            //System.Diagnostics.Debug.WriteLine("[" + Navigation.Stopwatch.ElapsedMilliseconds + "] PagePresenter.OnBringIntoViewRequested[" + e.TargetElement+"] EXIT");
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("[" + Navigation.Stopwatch.ElapsedMilliseconds + "] PagePresenter.OnGotFocus ENTER");
            base.OnGotFocus(e);
            //System.Diagnostics.Debug.WriteLine("[" + Navigation.Stopwatch.ElapsedMilliseconds + "] PagePresenter[" + Parameter+"].OnGotFocus EXIT");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("[" + Navigation.Stopwatch.ElapsedMilliseconds + "][" + e.Parameter + "] PagePresenter.OnNavigatedTo ENTER");
            base.OnNavigatedTo(e);

            if (e.Parameter is null)
            {
                throw new InvalidOperationException($"Cannot navigate to {nameof(PagePresenter)} without "
                + $"providing a {nameof(Page)} identifier.");
            }

            // Find the page instance in the dictionary and then discard it so we don't prevent it from being collected
            var key = Parameter = (Guid)e.Parameter;
            var page = NavigationPage.PageForGuid(key);
            //Pages.Remove(key);

            Content = page;
            //System.Diagnostics.Debug.WriteLine("[" + Navigation.Stopwatch.ElapsedMilliseconds + "][" + e.Parameter + "] PagePresenter.OnNavigatedTo EXIT");
        }
                */

        /*
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[" + Navigation.Stopwatch.ElapsedMilliseconds + "]][" + e.Parameter + "] PagePresenter.OnNavigatedFrom ENTER");
            base.OnNavigatedFrom(e);
            System.Diagnostics.Debug.WriteLine("[" + Navigation.Stopwatch.ElapsedMilliseconds + "]][" + e.Parameter + "] PagePresenter.OnNavigatedFrom EXIT");
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("[" + Navigation.Stopwatch.ElapsedMilliseconds + "]][" + e.Parameter + "] PagePresenter.OnNavigatingFrom ENTER");
            base.OnNavigatingFrom(e);
            //System.Diagnostics.Debug.WriteLine("[" + Navigation.Stopwatch.ElapsedMilliseconds + "]][" + e.Parameter + "] PagePresenter.OnNavigatedFrom EXIT");
        }

        protected override void PopulatePropertyInfoOverride(string propertyName, AnimationPropertyInfo animationPropertyInfo)
        {
            //System.Diagnostics.Debug.WriteLine("[" + Navigation.Stopwatch.ElapsedMilliseconds + "] PagePresenter.PopulatePropertyInfoOverride[" + propertyName + "] ENTER ");
            base.PopulatePropertyInfoOverride(propertyName, animationPropertyInfo);
            //System.Diagnostics.Debug.WriteLine("[" + Navigation.Stopwatch.ElapsedMilliseconds + "] PagePresenter.PopulatePropertyInfoOverride[" + propertyName + "] EXIT");
        }
        */


    }
}
