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
        #region Public Implementation

        #region Properties
        /// <summary>
        /// How man pages are in the Async Navigation Stack?
        /// </summary>
        public static int StackCount => PageStack.Count;

        /// <summary>
        /// The current page on the Async Navigation Stack
        /// </summary>
        public static Page CurrentPage => CurrentMetaPage?.Page;

        /// <summary>
        /// A stopwatch that tracks the performance of PushAsync and PopAsync
        /// </summary>
        public static readonly Stopwatch Stopwatch = new Stopwatch();
        #endregion

        #region Methods
        /// <summary>
        /// Push a page onto the Root Frame (Windows.UI.Xaml.Window.Current.Content)
        /// </summary>
        /// <param name="page">a pre-instantiated page</param>
        /// <param name="transitionInfo">Controls how the transition animation runs during the navigation action.</param>
        /// <returns>async Task to be awaited</returns>
        public static async Task PushAsync(Page page, NavigationTransitionInfo transitionInfo = null)
        {
            if (Windows.UI.Xaml.Window.Current.Content is Frame frame)
                await PushAsync(frame, page, transitionInfo);
            else
                throw new Exception("Cannot find Application's root frame.");
        }


        /// <summary>
        /// Push a page onto a Frame
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="page">a pre-instantiated page</param>
        /// <param name="transitionInfo">Controls how the transition animation runs during the navigation action.</param>
        /// <returns>async Task to be awaited</returns>
        public static async Task PushAsync(this Frame frame, Page page, NavigationTransitionInfo transitionInfo = null)
        {
            if (page is null)
                throw new ArgumentNullException("PushAsync page cannot be null.");

            if (frame is null)
                throw new ArgumentNullException("PushAsync frame cannot be null.");

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
            Stopwatch.Stop();
        }

        /// <summary>
        /// Pop the page most recently pushed onto the AsyncNavigation stack (via AsyncNavigation.PushAsync)
        /// </summary>
        /// <returns></returns>
        public static async Task PopAsync()
        {
            if (Windows.UI.Xaml.Window.Current.Content is Frame frame)
                await PopAsync(frame);
            else
                throw new Exception("Cannot find Application's root frame.");
        }

        /// <summary>
        /// Pop the page most recently pushed onto the AsyncNavigation stack (via AsyncNavigation.PushAsync)
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public static async Task PopAsync(this Frame frame)
        {
            if (frame is null)
                throw new ArgumentNullException("PopAsync frame cannot be null.");

            if (CurrentNavigationTask != null && !CurrentNavigationTask.IsCompleted)
            {
                var tcs = new TaskCompletionSource<bool>();
                Task oldTask = CurrentNavigationTask;
                CurrentNavigationTask = tcs.Task;
                await oldTask;
            }

            CurrentNavigationTask = PopAsyncInner(frame);
            await CurrentNavigationTask;
            Stopwatch.Stop();
        }
        #endregion

        #endregion


        #region Internal Implementation

        #region Properties

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

        internal static void PopCurrentMetaPage()
            => PageStack.Pop();

        #endregion

        #region Methods
        internal static Page PageForGuid(Guid guid)
        {
            if (PageStack.FirstOrDefault(mp => mp.Guid == guid) is MetaPage metaPage)
                return metaPage?.Page;
            return null;
        }
        #endregion

        #endregion


        #region Private Implementation

        #region Properties
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

        static Task CurrentNavigationTask { get; set; }

        static MetaPage CurrentMetaPage
        {
            get
            {
                if (PageStack.Any())
                    return PageStack.Peek();
                return null;
            }
        }

        static Guid? CurrentPageGuid => CurrentMetaPage?.Guid;

        #endregion


        #region Methods
        static async Task<bool> PushAsyncInner(Frame frame, Page page, NavigationTransitionInfo transitionInfo)
        {
            if (PageStack.Any(pair => pair.Page == page))
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
        #endregion

        #endregion


        #region Delete Me!
        /*
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
        */
        #endregion

    }
}
