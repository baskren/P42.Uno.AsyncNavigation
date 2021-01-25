using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Timers;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class NavigationPanel : Panel
    {
        readonly static Point Origin = new Point();
        internal Stack<PagePresenter> BackStack = new Stack<PagePresenter>();
        internal PagePresenter CurrentPagePresenter;
        internal Page CurrentPage => CurrentPagePresenter?.Content as Page;
        internal Stack<PagePresenter> ForewardStack = new Stack<PagePresenter>();
        bool enteringNewPage;
        bool exitingOldPage;

        internal bool CanGoBack => BackStack.Any();

        static Task<bool> CurrentNavigationTask { get; set; }

        public NavigationPanel()
        {
            // is this redundant?
            //SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Clip = new RectangleGeometry { Rect = new Rect(Origin, e.NewSize) };
            ArrangePages(e.NewSize);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (CurrentPagePresenter is Page currentPage)
            {
                currentPage.Measure(availableSize);
                return currentPage.DesiredSize;
            }
            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            ArrangePages(finalSize);
            return base.ArrangeOverride(finalSize);
        }

        void ArrangePages(Size pageSize)
        {
#if __MACOS__
            if (!exitingOldPage)
#elif !NETFX_CORE
            if (!exitingOldPage && !enteringNewPage)
#endif
            {
                if (BackStack.Any() && BackStack.Peek() is Page backPage)
                    backPage.Arrange(new Rect(Origin, pageSize));
                if (CurrentPagePresenter is Page currentPage)
                {
#if __MACOS__
                    if (enteringNewPage)
                        currentPage.Arrange(new Rect(new Point(pageSize.Width, pageSize.Height), pageSize));
                    else
#endif
                        currentPage.Arrange(new Rect(Origin, pageSize));
                }
                if (ForewardStack.Any() && ForewardStack.Peek() is Page nextPage)
                    nextPage.Arrange(new Rect(new Point(pageSize.Width, 0), pageSize));
            }
        }

        public async Task<bool> PushAsync(Page page, PageAnimationOptions pageAnimationOptions = null)
        {
            if (page is null)
                return false;

            //System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] P42.Uno.AsyncNavigation.NavigationPanel.PushAsync ENTER  page:" + page);
            if (CurrentNavigationTask != null && !CurrentNavigationTask.IsCompleted)
                await CurrentNavigationTask;

            CurrentNavigationTask = PushAsyncInner(page, pageAnimationOptions);
            var result = await CurrentNavigationTask;
            //System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] P42.Uno.AsyncNavigation.NavigationPanel.PushAsync EXIT  page:" + page);
            return result;
        }

        public async Task<bool> PopAsync(PageAnimationOptions pageAnimationOptions = null)
        {
            //System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] P42.Uno.AsyncNavigation.NavigationPanel.PopAsync ENTER  page:" + CurrentPage);
            if (CurrentNavigationTask != null && !CurrentNavigationTask.IsCompleted)
                await CurrentNavigationTask;

            CurrentNavigationTask = PopAsyncInner(pageAnimationOptions);
            var result =  await CurrentNavigationTask;
            //System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] P42.Uno.AsyncNavigation.NavigationPanel.PopAsync EXIT  page:" + CurrentPage);
            return result;
        }


        async Task<bool> PushAsyncInner(Page page, PageAnimationOptions pageAnimationOptions)
        {
            //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.NavigationPanel.PushAsyncInner ENTER [" + page.Content+"]");

            foreach (var child in ForewardStack)
                Children.Remove(child);
            ForewardStack.Clear();

            var presenter = new PagePresenter(page, CurrentPagePresenter != null);

            pageAnimationOptions = pageAnimationOptions ?? new PageAnimationOptions();
            presenter.SetEntranceAnimationOptions(pageAnimationOptions);

            if (CurrentPagePresenter is null)
                CurrentPagePresenter = presenter;
            else
                ForewardStack.Push(presenter);

            var tcs = new TaskCompletionSource<bool>();
            presenter.SetLoadedTaskCompletedSource(tcs);

            enteringNewPage = true;
            //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.NavigationPanel.PushAsyncInner A1 [" + page.Content + "]");
            Children.Add(presenter);
            //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.NavigationPanel.PushAsyncInner A2 [" + page.Content + "]");

            await tcs.Task;
            //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.NavigationPanel.PushAsyncInner A3 [" + page.Content + "]");

            if (ForewardStack.Any())
            {
                BackStack.Push(CurrentPagePresenter);
                CurrentPagePresenter = ForewardStack.Pop();
                if (pageAnimationOptions.AnimationDirection > AnimationDirection.None || pageAnimationOptions.ShouldFade)
                {
                    var size = new Size(ActualWidth, ActualHeight);
                    var animator = new BaseActionAnimator(
                                            pageAnimationOptions.Duration,
                                            pageAnimationOptions.ToEntranceAction(CurrentPagePresenter,size),
                                            pageAnimationOptions.EasingFunction
                                            );
                    await animator.RunAsync();
                }
                else
                {
                    tcs = new TaskCompletionSource<bool>();
                    presenter.SetArrangedTaskCompletionSource(tcs);
                    InvalidateArrange();

                    await tcs.Task;
                }
            }
            enteringNewPage = false;
            //System.Diagnostics.Debug.WriteLine("P42.Uno.AsyncNavigation.NavigationPanel.PushAsyncInner EXIT [" + page.Content + "]");
            return true;
        }

        public async Task<bool> PopAsyncInner(PageAnimationOptions pageAnimationOptions)
        {
            if (!BackStack.Any())
                return false;

            if (CurrentPagePresenter is PagePresenter presenter)
            {
                pageAnimationOptions = pageAnimationOptions ?? presenter.GetEntranceAnimationOptions().FlipDirection();
                ForewardStack.Push(CurrentPagePresenter);
                CurrentPagePresenter = BackStack.Pop();

                exitingOldPage = true;
                if (pageAnimationOptions.AnimationDirection > AnimationDirection.None || pageAnimationOptions.ShouldFade)
                {
                    var size = new Size(ActualWidth, ActualHeight);
                    var animator = new BaseActionAnimator(
                                            pageAnimationOptions.Duration,
                                            pageAnimationOptions.ToExitAction(presenter, size),
                                            pageAnimationOptions.EasingFunction
                                            );
                    await animator.RunAsync();
                }
                exitingOldPage = false;

                var tcs = new TaskCompletionSource<bool>();
                presenter.SetUnloadTaskCompletionSource(tcs);

                Children.Remove(presenter);

                var result = await tcs.Task;
                return result;
            }
            return false;
        }
    }
}
