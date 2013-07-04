using ZebritasWin8.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using ZebrasLib.Classes;
using System.Threading.Tasks;
using ZebrasLib.Places;

// The Split Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234234

namespace ZebritasWin8.Pages.Places
{
    /// <summary>
    /// A page that displays a group title, a list of items within the group, and details for
    /// the currently selected item.
    /// </summary>
    public sealed partial class SubCategoriesPage : Page
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
            get { return this.navigationHelper; }
        }

        public SubCategoriesPage()
        {
            this.InitializeComponent();

            // Setup the navigation helper
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            // Setup the logical page navigation components that allow
            // the page to only show one pane at a time.
            this.navigationHelper.GoBackCommand = new ZebritasWin8.Common.RelayCommand(() => this.GoBack(), () => this.CanGoBack());
            this.lsvSubCategories.SelectionChanged += itemListView_SelectionChanged;

            // Start listening for Window size changes 
            // to change from showing two panes to showing a single pane
            Window.Current.SizeChanged += Window_SizeChanged;
            this.InvalidateVisualState();
            watcher = new Geolocator();
            watcher.MovementThreshold = 200;
            latitude = 150;
            longitude = 150;
            comingBack = false;
            this.Loaded += SubCategoriesPage_Loaded;
            //prgPlaces.Visibility = System.Windows.Visibility.Visible;
        }

        void  SubCategoriesPage_Loaded(object sender, RoutedEventArgs e)
        {
            GetGps();
        }

        private async void GetGps()
        {
            Geoposition position = await watcher.GetGeopositionAsync();
            latitude = position.Coordinate.Latitude;
            longitude = position.Coordinate.Longitude;
            //prgPlaces.Visibility = System.Windows.Visibility.Visible;
            lstAllPlaces = await DownloadPlacesFromTheInternet(latitude, longitude);
            //prgPlaces.Visibility = System.Windows.Visibility.Collapsed;
            if (lstAllPlaces != null)
            {
                lstAllPlaces = PlacesMethods.getDistancesForEachPlace(latitude, longitude, lstAllPlaces);
                PopulateLists(lstAllPlaces);
            }
            //watcher.Stop();
        }

        private Geolocator watcher;
        List<Place> lstAllPlaces;
        double latitude;
        double longitude;
        bool comingBack;
        string categoryCode;
        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            pageTitle.DataContext = staticClasses.selectedCategory.name;
            categoryCode = staticClasses.selectedCategory.code;
            //prgPlaces.Visibility = System.Windows.Visibility.Collapsed;
            if(!comingBack)
            {
                //LoadAppBar();
                //watcher.Start();
                comingBack = true;
            }
        }

        //#region AppBar
        //private void LoadAppBar()
        //{
        //    ApplicationBar = new ApplicationBar();

        //    ApplicationBarIconButton btnUpdatePlaces = new ApplicationBarIconButton();
        //    btnUpdatePlaces.IconUri = new Uri("/Assets/AppBar/download.png", UriKind.Relative);
        //    btnUpdatePlaces.Text = AppResources.AppBarDownload;
        //    btnUpdatePlaces.Click += btnUpdatePlaces_Click;
        //    ApplicationBar.Buttons.Add(btnUpdatePlaces);
        //}

        //private async void btnUpdatePlaces_Click(object sender, EventArgs e)
        //{
        //    prgPlaces.IsIndeterminate = false;
        //    prgPlaces.IsIndeterminate = true;
        //    prgPlaces.Visibility = System.Windows.Visibility.Visible;
        //    lstAllPlaces = await DownloadPlacesFromTheInternet(latitude,longitude);
        //    if (lstAllPlaces != null)
        //    {
        //        lstAllPlaces = PlacesMethods.getDistancesForEachPlace(latitude, longitude, lstAllPlaces);
        //        PopulateLists(lstAllPlaces);
        //        UpdateDataBase(lstAllPlaces);
        //    }
        //    prgPlaces.Visibility = System.Windows.Visibility.Collapsed;
        //}
        //#endregion

        //#region GPS
        //private void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
        //{
        //    switch (e.Status)
        //    {
        //        case GeoPositionStatus.Disabled:
        //            MessageBox.Show(AppResources.TxtGPSDisabled);
        //            NavigationService.GoBack();
        //            break;

        //        case GeoPositionStatus.NoData:
        //            MessageBox.Show(AppResources.TxtGPSNoData);
        //            NavigationService.GoBack();
        //            break;

        //        default:
        //            break;
        //    }
        //}

        //private async void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        //{
        //    latitude = e.Position.Location.Latitude;
        //    longitude = e.Position.Location.Longitude;
        //    prgPlaces.Visibility = System.Windows.Visibility.Visible;
        //    lstAllPlaces = await DownloadOrGetPlacesFromDataBase();
        //    prgPlaces.Visibility = System.Windows.Visibility.Collapsed;
        //    if (lstAllPlaces != null)
        //    {
        //        lstAllPlaces = PlacesMethods.getDistancesForEachPlace(latitude,longitude, lstAllPlaces);
        //        PopulateLists(lstAllPlaces);
        //    }
        //    watcher.Stop();
        //}
        //#endregion

        //private async Task<List<Place>>DownloadOrGetPlacesFromDataBase()
        //{
        //    List<Place> lstToReturn = new List<Place>();
        //    if (App.AutoDownloadsPlaces)
        //    {
        //        prgPlaces.Visibility = System.Windows.Visibility.Visible;
        //        lstToReturn = await DownloadPlacesFromTheInternet(latitude, longitude);
        //        prgPlaces.Visibility = System.Windows.Visibility.Collapsed;
        //    }
        //    if (lstToReturn != null)
        //        return lstToReturn;
        //    lstToReturn = DBPhone.PlacesMethods.GetItems(DBPhone.CategoriesMethods.GetSubCategoriesCodes(categoryCode));

        //    if (lstToReturn != null)
        //        return lstToReturn;
        //    else {
        //        SetVisibilities(AppResources.TxtNoPlaces, Visibility.Visible, Visibility.Collapsed);
        //        return null;
        //    }
                
        //}

        private async Task<List<Place>> DownloadPlacesFromTheInternet(double latitude, double longitude)
        {
            List<Place> lstFromTheInternet = await PlacesMethods.getAllPlacesByCategory(categoryCode, 59.9188200940917, 30.2882713079452);
            if (lstFromTheInternet != null)
            {
                if (lstFromTheInternet.Count > 0)
                {
                    foreach (Place P in lstFromTheInternet)
                    {
                        P.parentCategoryCode = categoryCode;
                    }

                    //UpdateDataBase(lstFromTheInternet);
                    return lstFromTheInternet;
                }
            }
            return null;
        }

        //private void SetVisibilities(string message, Visibility forTheMessages, Visibility forTheLists)
        //{
        //    txtNoPlacesFound.Text = message;
        //    txtNoNearPlacesFound.Text = message;
        //    txtNoPopularPlacesFound.Text = message;
        //    txtNoPlacesFound.Visibility = forTheMessages;
        //    txtNoNearPlacesFound.Visibility = forTheMessages;
        //    txtNoPopularPlacesFound.Visibility = forTheMessages;
        //    lstbAllPlaces.Visibility = forTheLists;
        //}

        //private void UpdateDataBase(List<Place> toAddList)
        //{   
        //    DBPhone.PlacesMethods.RemoveItems(categoryCode);
        //    DBPhone.PlacesMethods.AddItems(toAddList);
        //}

        private void PopulateLists(List<Place> lstAllPlaces)
        {
            //List<Place> lstTempPlaces;
            lsvSubCategories.ItemsSource = getDajaCategories(lstAllPlaces);
            lsvSubCategories.Visibility = Visibility.Visible;
            //txtNoPlacesFound.Visibility = Visibility.Collapsed;

            //lstTempPlaces = PlacesMethods.getPlacesOrderedByPopularity(lstAllPlaces);
            //if (lstTempPlaces.Count > 0)
            //{
            //    lstbPopularPlaces.ItemsSource = getDajaCategories(lstTempPlaces);
            //    txtNoPlacesFound.Visibility = Visibility.Collapsed;
            //    lstbPopularPlaces.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    txtNoPopularPlacesFound.Text = AppResources.TxtNoPopularPlaces;
            //    txtNoPopularPlacesFound.Visibility = Visibility.Visible;
            //    lstbPopularPlaces.Visibility = Visibility.Collapsed;
            //} 

            //lstTempPlaces = PlacesMethods.getPlacesNear(lstAllPlaces,App.nearDistance);
            //if (lstTempPlaces.Count > 0)
            //{
            //    lstbNearPlaces.ItemsSource = getDajaCategories(lstTempPlaces);
            //    lstbNearPlaces.Visibility = Visibility.Visible;
            //    txtNoNearPlacesFound.Visibility = Visibility.Collapsed;
            //}
            //else
            //{
            //    txtNoNearPlacesFound.Text = AppResources.TxtNoNearPlaces;
            //    lstbNearPlaces.Visibility = Visibility.Collapsed;
            //    txtNoNearPlacesFound.Visibility = Visibility.Visible;
            //} 
        }

        //private void placeSelected(object sender, GestureEventArgs e)
        //{
        //    staticClasses.selectedPlace = (((sender as StackPanel).Tag) as Place);
        //    //NavigationService.Navigate(new Uri("/Pages/Places/SelectedPlacePage.xaml?comingFrom=Selected", UriKind.Relative));

        //}

        private List<bindingCategory> getDajaCategories(List<Place> lstPlaces)
        {
            List<bindingCategory> lstCategoriesDaja = new List<bindingCategory>();
            List<Category> lstSubCategories = staticClasses.selectedCategory.subCategories;
            bindingCategory categoryDaja;

            foreach (Category subCategory in lstSubCategories)
            {
                categoryDaja = new bindingCategory();
                categoryDaja.category = subCategory;
                var query = from variable
                            in lstPlaces
                            where variable.categoryCode.Equals(subCategory.code)
                            select variable;
                    
                categoryDaja.lstPlaces = query.ToList();
                categoryDaja.category.icon = "/Assets/Icons/Categories/" + categoryDaja.category.icon;
                lstCategoriesDaja.Add(categoryDaja);
            }
            return lstCategoriesDaja;
        }











        void itemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.UsingLogicalPageNavigation())
            {
                this.navigationHelper.GoBackCommand.RaiseCanExecuteChanged();
            }
        }

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
            // TODO: Assign a bindable group to Me.DefaultViewModel("Group")
            // TODO: Assign a collection of bindable items to Me.DefaultViewModel("Items")

            if (e.PageState == null)
            {
                // When this is a new page, select the first item automatically unless logical page
                // navigation is being used (see the logical page navigation #region below.)
                if (!this.UsingLogicalPageNavigation() && this.itemsViewSource.View != null)
                {
                    this.itemsViewSource.View.MoveCurrentToFirst();
                }
            }
            else
            {
                // Restore the previously saved state associated with this page
                if (e.PageState.ContainsKey("SelectedItem") && this.itemsViewSource.View != null)
                {
                    // TODO: Invoke Me.itemsViewSource.View.MoveCurrentTo() with the selected
                    //       item as specified by the value of pageState("SelectedItem")

                }
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            if (this.itemsViewSource.View != null)
            {
                // TODO: Derive a serializable navigation parameter and assign it to
                //       pageState("SelectedItem")

            }
        }

        #region Logical page navigation

        // The split page isdesigned so that when the Window does have enough space to show
        // both the list and the dteails, only one pane will be shown at at time.
        //
        // This is all implemented with a single physical page that can represent two logical
        // pages.  The code below achieves this goal without making the user aware of the
        // distinction.

        private const int MinimumWidthForSupportingTwoPanes = 768;

        /// <summary>
        /// Invoked to determine whether the page should act as one logical page or two.
        /// </summary>
        /// <returns>True if the window should show act as one logical page, false
        /// otherwise.</returns>
        private bool UsingLogicalPageNavigation()
        {
            return Window.Current.Bounds.Width < MinimumWidthForSupportingTwoPanes;
        }

        /// <summary>
        /// Invoked with the Window changes size
        /// </summary>
        /// <param name="sender">The current Window</param>
        /// <param name="e">Event data that describes the new size of the Window</param>
        private void Window_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            this.InvalidateVisualState();
        }

        /// <summary>
        /// Invoked when an item within the list is selected.
        /// </summary>
        /// <param name="sender">The GridView displaying the selected item.</param>
        /// <param name="e">Event data that describes how the selection was changed.</param>
        private void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Invalidate the view state when logical page navigation is in effect, as a change
            // in selection may cause a corresponding change in the current logical page.  When
            // an item is selected this has the effect of changing from displaying the item list
            // to showing the selected item's details.  When the selection is cleared this has the
            // opposite effect.
            if (this.UsingLogicalPageNavigation()) this.InvalidateVisualState();
        }

        private bool CanGoBack()
        {
            if (this.UsingLogicalPageNavigation() && this.lsvSubCategories.SelectedItem != null)
            {
                return true;
            }
            else
            {
                return this.navigationHelper.CanGoBack();
            }
        }
        private void GoBack()
        {
            if (this.UsingLogicalPageNavigation() && this.lsvSubCategories.SelectedItem != null)
            {
                // When logical page navigation is in effect and there's a selected item that
                // item's details are currently displayed.  Clearing the selection will return to
                // the item list.  From the user's point of view this is a logical backward
                // navigation.
                this.lsvSubCategories.SelectedItem = null;
            }
            else
            {
                this.navigationHelper.GoBack();
            }
        }

        private void InvalidateVisualState()
        {
            var visualState = DetermineVisualState();
            VisualStateManager.GoToState(this, visualState, false);
            this.navigationHelper.GoBackCommand.RaiseCanExecuteChanged();
        }

        /// <summary>
        /// Invoked to determine the name of the visual state that corresponds to an application
        /// view state.
        /// </summary>
        /// <returns>The name of the desired visual state.  This is the same as the name of the
        /// view state except when there is a selected item in portrait and snapped views where
        /// this additional logical page is represented by adding a suffix of _Detail.</returns>
        private string DetermineVisualState()
        {
            if (!UsingLogicalPageNavigation())
                return "PrimaryView";

            // Update the back button's enabled state when the view state changes
            var logicalPageBack = this.UsingLogicalPageNavigation() && this.lsvSubCategories.SelectedItem != null;

            return logicalPageBack ? "SinglePane_Detail" : "SinglePane";
        }

        #endregion

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}
