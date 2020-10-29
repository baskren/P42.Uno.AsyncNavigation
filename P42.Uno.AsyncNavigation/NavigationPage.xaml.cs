﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    /// Host Page for Asynchronous Page Navigation
    /// </summary>
    public partial class NavigationPage : Page
    {
        #region Public Implementation

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
        /// <param name="transitionInfo">Controls how the transition animation runs during the navigation action.</param>
        /// <returns>async Task to be awaited</returns>
        public async Task<bool> PushAsync(Page page, NavigationTransitionInfo transitionInfo = null)
        {
            System.Diagnostics.Debug.WriteLine("NavigationPage.PushAsync ENTER  page:["+page+"]");
            if (page is null)
                throw new ArgumentNullException("PushAsync page cannot be null.");

            if (_navPanel.Children.Any(c => c is Page p && p.Content == page))
                return false;

            Stopwatch.Reset();
            Stopwatch.Start();

            var result = await _navPanel.PushAsync(page);

            Stopwatch.Stop();

            System.Diagnostics.Debug.WriteLine("NavigationPage.PushAsync EXIT  page:[" + page + "]");
            return result;
        }

        /// <summary>
        /// Pop the page most recently pushed onto the AsyncNavigation stack (via AsyncNavigation.PushAsync)
        /// </summary>
        /// <returns></returns>
        public async Task<bool> PopAsync()
        {
            System.Diagnostics.Debug.WriteLine("NavigationPage.PopAsync ENTER  page:[" + CurrentPage + "]");
            if (_navPanel.CanGoBack)
            {
                Stopwatch.Reset();
                Stopwatch.Start();

                if (_navPanel.CurrentPagePresenter is PagePresenter page)
                {
                    await _navPanel.PopAsync();
                    System.Diagnostics.Debug.WriteLine("NavigationPage.PopAsync EXIT  page:[" + CurrentPage + "]");
                    return true;
                }
            }
            System.Diagnostics.Debug.WriteLine("NavigationPage.PopAsync EXIT  page:[" + CurrentPage + "]");
            return false;
        }
        #endregion


        #endregion


        /*
        #region Internal Implementation

        internal static Page PageForGuid(Guid guid)
        {
            if (PageStack.FirstOrDefault(mp => mp.Guid == guid) is MetaPage metaPage)
                return metaPage?.Page;
            return null;
        }

        #endregion
        */

        #region Private Implementation

        #region Properties

        #endregion


        #region Methods


        #endregion

        #endregion


    }
}
