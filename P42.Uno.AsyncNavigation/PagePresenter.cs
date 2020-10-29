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
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace P42.Uno.AsyncNavigation
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public partial class PagePresenter : Page
    {
        bool _waitingForLoad;

        public PagePresenter()
        {
            Loaded += PagePresenter_Loaded;
            _waitingForLoad = true;
            
        }

        public PagePresenter(Page page) : this()
        {
            Content = page;
        }

        private void PagePresenter_Loaded(object sender, RoutedEventArgs e)
        {
            if (_waitingForLoad && this.GetLoadedTaskCompletedSource() is TaskCompletionSource<bool> tcs)
            {
                this.SetLoadedTaskCompletedSource(null);
                tcs.SetResult(true);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var result =  base.ArrangeOverride(finalSize);
            if (this.GetArrangedTaskCompletionSource() is TaskCompletionSource<bool> tcs)
            {
                this.SetArrangedTaskCompletionSource(null);
                tcs.SetResult(true);
            }
            return result;
        }
    }
}
