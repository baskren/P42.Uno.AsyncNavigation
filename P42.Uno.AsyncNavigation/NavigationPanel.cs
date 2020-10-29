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
    public sealed partial class NavigationPanel : Panel
    {
        //static NavigationPanel _current;
        //public static NavigationPanel Current => _current = _current ?? new NavigationPanel();

        readonly static Point Origin = new Point();
        internal Stack<PagePresenter> BackStack = new Stack<PagePresenter>();
        internal PagePresenter CurrentPagePresenter;
        internal Page CurrentPage => CurrentPagePresenter?.Content as Page;
        internal Stack<PagePresenter> ForewardStack = new Stack<PagePresenter>();

        internal bool CanGoBack => BackStack.Any();

        static Task<bool> CurrentNavigationTask { get; set; }

        bool IsAnimated = true;

        public NavigationPanel()
        {
            // is this redundant?
            SizeChanged += OnSizeChanged;
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
            if (BackStack.Any() && BackStack.Peek() is Page backPage)
                backPage.Arrange(new Rect(Origin, pageSize));
            if (CurrentPagePresenter is Page currentPage)
                currentPage.Arrange(new Rect(Origin, pageSize));
            if (ForewardStack.Any() && ForewardStack.Peek() is Page nextPage)
                nextPage.Arrange(new Rect(new Point(pageSize.Width, 0), pageSize));
        }

        public async Task<bool> PushAsync(Page page)
        {
            if (page is null)
                return false;

            System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] NavigationPanel.PushAsync ENTER  page:" + page);
            if (CurrentNavigationTask != null && !CurrentNavigationTask.IsCompleted)
                await CurrentNavigationTask;

            CurrentNavigationTask = PushAsyncInner(page);
            var result = await CurrentNavigationTask;
            System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] NavigationPanel.PushAsync EXIT  page:" + page);
            return result;
        }

        public async Task<bool> PopAsync()
        {
            System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] NavigationPanel.PushAsync ENTER  page:" + CurrentPage);
            if (CurrentNavigationTask != null && !CurrentNavigationTask.IsCompleted)
                await CurrentNavigationTask;

            CurrentNavigationTask = PopAsyncInner();
            var result =  await CurrentNavigationTask;
            System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] NavigationPanel.PushAsync EXIT  page:" + CurrentPage);
            return result;
        }


        async Task<bool> PushAsyncInner(Page page)
        {
            System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] NavigationPanel.PushAsyncInner ENTER  page:" + page);
            foreach (var child in ForewardStack)
                Children.Remove(child);
            ForewardStack.Clear();

            var presenter = new PagePresenter(page);

            if (CurrentPagePresenter is null)
                CurrentPagePresenter = presenter;
            else
                ForewardStack.Push(presenter);

            var tcs = new TaskCompletionSource<bool>();
            presenter.SetLoadedTaskCompletedSource(tcs);

            Children.Add(presenter);

            System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] NavigationPanel.PushAsyncInner A  page:" + page);
            await tcs.Task;
            System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] NavigationPanel.PushAsyncInner A'  page:" + page);

            
            if (ForewardStack.Any())
            {
                BackStack.Push(CurrentPagePresenter);
                CurrentPagePresenter = ForewardStack.Pop();
                //ArrangePages(new Size(ActualWidth, ActualHeight));
                System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] NavigationPanel.PushAsyncInner B  page:" + page);
                if (IsAnimated)
                {
                    var x = ActualWidth;
                    while (x > 0)
                    {
                        await Task.Delay(20);
                        x -= 100;
                        if (x < 0)
                            x = 0;
                        CurrentPagePresenter.Arrange(new Rect(new Point(x, 0), new Size(ActualWidth, ActualHeight)));
                    }
                }
                else
                {
                    tcs = new TaskCompletionSource<bool>();
                    presenter.SetArrangedTaskCompletionSource(tcs);
                    InvalidateArrange();

                    await tcs.Task;
                }
                System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] NavigationPanel.PushAsyncInner B'  page:" + page);
            }


            System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] NavigationPanel.PushAsyncInner EXIT  page:" + page);
            return true;
        }

        public async Task<bool> PopAsyncInner()
        {
            if (!BackStack.Any())
                return false;

            System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] NavigationPanel.PopAsyncInner ENTER  page:" + CurrentPage);
            if (CurrentPagePresenter is PagePresenter presenter)
            {
                ForewardStack.Push(CurrentPagePresenter);
                CurrentPagePresenter = BackStack.Pop();

                if (IsAnimated)
                {
                    System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] NavigationPanel.PushAsyncInner A  page:" + presenter.Content);
                    var x = 0.0;
                    while (x < ActualWidth)
                    {
                        await Task.Delay(20);
                        x += 100;
                        if (x > ActualWidth)
                            x = ActualWidth;
                        presenter.Arrange(new Rect(new Point(x, 0), new Size(ActualWidth, ActualHeight)));
                    }
                    System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] NavigationPanel.PushAsyncInner A'  page:" + presenter.Content);
                }

                var tcs = new TaskCompletionSource<bool>();
                presenter.SetArrangedTaskCompletionSource(tcs);

                Children.Remove(presenter);

                System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] NavigationPanel.PopAsyncInner B  page:" + presenter.Content);
                var result = await tcs.Task;
                System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] NavigationPanel.PopAsyncInner B'  page:" + presenter.Content);

                return result;
            }

            System.Diagnostics.Debug.WriteLine("[" + NavigationPage.Stopwatch.ElapsedMilliseconds + "] NavigationPanel.PopAsyncInner ENTER  page:" + CurrentPage);
            return false;
        }
    }
}
