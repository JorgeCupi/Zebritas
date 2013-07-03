using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using ZebritasWin8.Common;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ZebrasLib.Events;
using ZebrasLib;
using ZebrasLib.Classes;
using Windows.Devices.Geolocation;
using Bing.Maps;
using ZebritasWin8.UserControls;
// The Grouped Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234231

namespace ZebritasWin8.Pages.Problems
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class TroublesPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get
            {
                return this.navigationHelper;
            }
        }

        public TroublesPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.Loaded += TroublesPage_Loaded;
            watcher = new Geolocator();
            watcher.MovementThreshold = 200;
            lstTroubles.SelectionChanged += lstTroubles_SelectionChanged;
            lstTroublesByFriends.SelectionChanged += lstTroublesByFriends_SelectionChanged;
            
        }

        public List<Problem> lstEvents { get; set; }
        public List<Problem> lstEventsByFriends { get; set; }
        //ApplicationBarIconButton btnReport;
        Geolocator watcher;
        //private GeoCoordinateWatcher watcher;
        private double latitude;
        private double longitude;

        //public TroublesPage()
        //{
        //    InitializeComponent();
            
            
        //}

        private async void GetGps()
        {
            Geoposition coordinate = await watcher.GetGeopositionAsync();
            latitude = coordinate.Coordinate.Latitude;
            longitude = coordinate.Coordinate.Longitude;
            //prgEvents.Visibility = System.Windows.Visibility.Visible;
            lstEvents = await ProblemsMethods.GetProblems(latitude, longitude, Main.GetValueFromTimeZone());
            if (lstEvents != null)
            {
                //lstEvents = await OurFacebook.FacebookMethods.GetFbInfoForTheseReporters(lstEvents, App.facebookAccessToken);
                LoadPushPins();
                //GetFriendsProblems();
            }
            //else
            //    SetTheWorldOnFire(AppResources.TxtInternetConnectionProblem);
            //prgEvents.Visibility = System.Windows.Visibility.Collapsed;
        }

        void lstTroublesByFriends_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
   
        }

        void lstTroubles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void TroublesPage_Loaded(object sender, RoutedEventArgs e)
        {
            GetGps();
        }

        //FRIENDS FACEBOOK
        //private async void GetFriendsProblems()
        //{
        //    List<facebookUser> facebookFriends = await OurFacebook.FacebookMethods.downloadFriendsList(App.facebookAccessToken);
        //    List<string> friendsCodes = new List<string>();
        //    foreach (facebookUser user in facebookFriends)
        //        friendsCodes.Add(user.id);

        //    lstEventsByFriends = await ProblemsMethods.GetProblems(friendsCodes);
        //    lstEventsByFriends = await OurFacebook.FacebookMethods.GetFbInfoForTheseReporters(lstEventsByFriends, App.facebookAccessToken);
        //    lstTroublesByFriends.ItemsSource = lstEventsByFriends;
        //}

        #region AppBar
        //private void LoadAppBar()
        //{
        //    ApplicationBar = new ApplicationBar();
        //    btnReport = new ApplicationBarIconButton();
        //    btnReport.IconUri = new Uri("/Assets/AppBar/upload.png", UriKind.Relative);
        //    btnReport.Text = AppResources.TxtReport;
        //    btnReport.Click += btnReport_Click;
        //    ApplicationBar.Buttons.Add(btnReport);
        //}

        //void btnReport_Click(object sender, EventArgs e)
        //{
        //    //NavigationService.Navigate(new Uri("/Pages/Trouble/ReportPage.xaml", UriKind.Relative));
        //}
        #endregion

        //void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        //{
        //    switch (e.Status)
        //    {
        //        case GeoPositionStatus.Disabled:
        //            SetTheWorldOnFire(AppResources.TxtGPSDisabled);
        //            break;
        //        case GeoPositionStatus.NoData:
        //            SetTheWorldOnFire(AppResources.TxtGPSNoData);
        //            break;
        //        default:
        //            break;
        //    }
        //}

        //private async void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        //{
        //    latitude = e.Position.Location.Latitude;
        //    longitude = e.Position.Location.Longitude;
        //    prgEvents.Visibility = System.Windows.Visibility.Visible;
        //    lstEvents = await ProblemsMethods.GetProblems(latitude, longitude);
        //    if (lstEvents != null)
        //    {
        //        lstEvents = await OurFacebook.FacebookMethods.GetFbInfoForTheseReporters(lstEvents, App.facebookAccessToken);
        //        LoadPushPins();
        //        GetFriendsProblems();
        //    }
        //    else
        //        SetTheWorldOnFire(AppResources.TxtInternetConnectionProblem);
        //    prgEvents.Visibility = System.Windows.Visibility.Collapsed;
        //    watcher.Stop();

        //}

        //private void SetTheWorldOnFire(string status)
        //{
        //    panProblems.Visibility = Visibility.Collapsed;
        //    txtNoInternet.Visibility = Visibility.Visible;
        //    txtNoInternet.Text = status;
        //    btnReport.IsEnabled = false;
        //}

        private void LoadPushPins()
        {
            MapLayer layers = new MapLayer();
            //MapOverlay overlay;

            foreach (var item in lstEvents)
            {
                //overlay = new MapOverlay();
                uscPushPin pushPin = new uscPushPin(item);
                pushPin.txbCategory.Text = item.type + "";
                //overlay.Content = pushPin;
                //overlay.GeoCoordinate = new GeoCoordinate(item.latitude, item.longitude);
                //layers.Add(overlay);}
                layers.Children.Add(pushPin);
            }
            mapTroubles.Center = new Location(latitude, longitude);
            mapTroubles.Children.Add(layers);
            lstTroubles.ItemsSource = lstEvents;
        }

        //private void lstTroubles_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    NavigateToReportersPage(lstTroubles);
        //}

        //void lstTroublesByFriends_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    NavigateToReportersPage(lstTroublesByFriends);
        //}

        //private void NavigateToReportersPage(System.Windows.Controls.ListBox lstTroubles)
        //{
        //    Problem problem = lstTroubles.SelectedItem as Problem;
        //    if (problem != null)
        //    {
        //        staticClasses.selectedEvent = problem;
        //        NavigationService.Navigate(new Uri("/Pages/Trouble/EventReportersPage.xaml", UriKind.Relative));
        //        lstTroubles.SelectedItem = null;
        //        lstTroubles.SelectedIndex = -1;
        //    }
        //}

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Assign a collection of bindable groups to this.DefaultViewModel["Groups"]
        }
    }
}
