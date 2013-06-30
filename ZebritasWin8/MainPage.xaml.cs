using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ZebritasWin8.Pages.Menus;
using ZebritasWin8.Pages.Problems;

namespace ZebritasWin8
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            animationEnded = true;
            stbMapFLip.AutoReverse = true;
            stbMapFLip.Completed += stbMapFLip_Completed;
        }

        void stbMapFLip_Completed(object sender, object e)
        {
            
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }
        public bool animationEnded { get; set; }
        private void rtnSettings_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(animationEnded)
            {
                stbMapFLip.Begin();
            }
            else
            {
                stbMapFLip.Resume();
            }

        }

        private void rtnAbout_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }

        private void rtnProblems_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(CategoryPage));
        }

        private void map_Tapped(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(TroublesPage));
        }
    }
}
