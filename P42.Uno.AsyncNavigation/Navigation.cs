using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace P42.Uno.AsyncNavigation
{
    public static class Navigation
    {

        #region ArrangeTaskCompletedSource Property
        internal static readonly DependencyProperty ArrangeTaskCompletedSourceProperty = DependencyProperty.RegisterAttached(
            "ArrangeTaskCompletedSource",
            typeof(TaskCompletionSource<bool>),
            typeof(Navigation),
            new PropertyMetadata(null)
        );
        internal static TaskCompletionSource<bool> GetArrangeTaskCompletedSource(this Page element)
            => (TaskCompletionSource<bool>)element.GetValue(ArrangeTaskCompletedSourceProperty);
        internal static void SetArrangeTaskCompletedSource(this Page element, TaskCompletionSource<bool> value)
            => element.SetValue(ArrangeTaskCompletedSourceProperty, value);
        #endregion ArrangeTaskCompletedSource Property


        #region UnloadTaskCompletionSource Property
        internal static readonly DependencyProperty UnloadTaskCompletionSourceProperty = DependencyProperty.RegisterAttached(
            "UnloadTaskCompletionSource",
            typeof(TaskCompletionSource<bool>),
            typeof(Navigation),
            new PropertyMetadata(null)
        );
        internal static TaskCompletionSource<bool> GetUnloadTaskCompletionSource(this Page element)
            => (TaskCompletionSource<bool>)element.GetValue(UnloadTaskCompletionSourceProperty);
        internal static void SetUnloadTaskCompletionSource(this Page element, TaskCompletionSource<bool> value)
            => element.SetValue(UnloadTaskCompletionSourceProperty, value);
        #endregion UnloadTaskCompletionSource Property



        class MetaPage
        {
            public Guid Guid { get; private set; }

            public Page Page { get; private set; }

            public MetaPage(Page page)
            {
                Page = page;
                Guid = Guid.NewGuid();
            }
        }

        static Stack<MetaPage> PageStack = new Stack<MetaPage>();

        public static int StackCount => PageStack.Count;

        static Task CurrentNavigationTask;

        static MetaPage CurrentMetaPage
        {
            get
            {
                if (PageStack.Any())
                    return PageStack.Peek();
                return null;
            }
        }

        internal static void PopCurrentMetaPage()
            => PageStack.Pop();

        static Guid? CurrentPageGuid => CurrentMetaPage?.Guid;

        public static Page CurrentPage => CurrentMetaPage?.Page;

        public static readonly Stopwatch Stopwatch = new Stopwatch();

        internal static Page PageForGuid(Guid guid)
        {
            if (PageStack.FirstOrDefault(mp => mp.Guid == guid) is MetaPage metaPage)
                return metaPage?.Page;
            return null;
                
        }

        static Navigation()
        {
            if (Windows.UI.Xaml.Window.Current.Content is Frame frame)
            {
                frame.Navigated += Frame_Navigated;
                frame.Navigating += Frame_Navigating;
                frame.NavigationFailed += Frame_NavigationFailed;
                frame.NavigationStopped += Frame_NavigationStopped;
            }
        }

        private static void Frame_NavigationStopped(object sender, NavigationEventArgs e)
        {

            System.Diagnostics.Debug.WriteLine("["+Navigation.Stopwatch.ElapsedMilliseconds+ "][" + e.Parameter + "] Navigation.Frame_NavigationStopped mode[" + e.NavigationMode + "] content[" + e.Content + "] uri[" + e.Uri + "]");
        }

        private static void Frame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[" + Navigation.Stopwatch.ElapsedMilliseconds + "][" + e.Exception + "] Navigation.Frame_NavigationFailed[" + e.Handled + "]");
        }

        private static void Frame_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[" + Navigation.Stopwatch.ElapsedMilliseconds + "][" + e.Parameter + "] Navigation.Frame_Navigating[" + e.NavigationMode + "][" + e.Cancel + "]");
        }

        private static void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[" + Navigation.Stopwatch.ElapsedMilliseconds + "][" + e.Parameter + "] Navigation.Frame_Navigated[" + e.NavigationMode+"]["+e.Content+"]["+e.Uri+"]");
        }

        public static async Task PushAsync(Page page)
        {
            if (Windows.UI.Xaml.Window.Current.Content is Frame frame)
                await frame.PushAsync(page);
            else
                throw new Exception("Cannot find Application's root frame.");
        }

        public static async Task PushAsync(this Frame frame, Page page, NavigationTransitionInfo transitionInfo = null)
        {
            frame.IsNavigationStackEnabled = true;
            if (CurrentNavigationTask != null && !CurrentNavigationTask.IsCompleted)
            {
                var tcs = new TaskCompletionSource<bool>();
                Task oldTask = CurrentNavigationTask;
                CurrentNavigationTask = tcs.Task;
                await oldTask;
            }

            CurrentNavigationTask = PushAsyncInner(frame, page, transitionInfo);
            await CurrentNavigationTask;
        }

        public static async Task PopAsync()
        {
            if (Windows.UI.Xaml.Window.Current.Content is Frame frame)
                await frame.PopAsync();
            else
                throw new Exception("Cannot find Application's root frame.");
        }

        public static async Task PopAsync(this Frame frame)
        {
            if (CurrentNavigationTask != null && !CurrentNavigationTask.IsCompleted)
            {
                var tcs = new TaskCompletionSource<bool>();
                Task oldTask = CurrentNavigationTask;
                CurrentNavigationTask = tcs.Task;
                await oldTask;
            }

            CurrentNavigationTask = PopAsyncInner(frame);
            await CurrentNavigationTask;
        }


        static async Task<bool> PushAsyncInner(Frame frame, Page page, NavigationTransitionInfo transitionInfo)
        {
            if (PageStack.Any(pair=>pair.Page == page))
                return false;

            Stopwatch.Reset();
            Stopwatch.Start();

            page.Measure(EstimatedFrameSize(frame));
            var metaPage = new MetaPage(page);

            var tcs = new TaskCompletionSource<bool>();
            page.SetArrangeTaskCompletedSource(tcs);

            PageStack.Push(metaPage);
            frame.Navigate(typeof(PagePresenter), metaPage.Guid, transitionInfo);
            return await tcs.Task;
        }

        static async Task<bool> PopAsyncInner(Frame frame)
        {
            if (frame.CanGoBack)
            {
                Stopwatch.Reset();
                Stopwatch.Start();

                if (CurrentPage is Page page)
                {
                    var tcs = new TaskCompletionSource<bool>();
                    page.SetUnloadTaskCompletionSource(tcs);
                    frame.GoBack();
                    return await tcs.Task;
                }
                frame.GoBack();
            }
            return false;
        }

        static Size EstimatedFrameSize(Frame frame)
        {
            var frameW = frame.Width;
            var frameH = frame.Height;

            if (frame.ActualWidth > 0)
                frameW = frame.ActualWidth;
            if (frame.ActualHeight > 0)
                frameH = frame.ActualHeight;

            if (frameW < 1)
                frameW = frame.DesiredSize.Width;
            if (frameH < 1)
                frameH = frame.DesiredSize.Height;

            if (frameW < 1)
                frameW = 900;
            if (frameH < 1)
                frameH = 1200;

            return new Size(frameW, frameH);
        }
        
    }
}
