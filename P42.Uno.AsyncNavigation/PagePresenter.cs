using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace P42.Uno.AsyncNavigation
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public partial class PagePresenter : Page
    {
        bool _waitingForLoad;

        public PagePresenter()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            _waitingForLoad = true;
            
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

        public PagePresenter(Page page) : this()
        {
            Content = page;
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
