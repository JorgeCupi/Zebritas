﻿using ZebritasWin8.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using ZebrasLib.Classes;
using ZebritasWin8.Pages.Places;
using ZebrasLib.Places;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ZebritasWin8.Pages.Menus
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class CategoryPage : Page
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
        private bool comingBack;
        private Geolocator watcher;
        List<Category> returned;
        public CategoryPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            this.Loaded += CategoriesPage_Loaded;
            comingBack = false;
            grvCategories.SelectionChanged += lstCategoryList_SelectionChanged;
            //lstSearchResults.SelectionChanged += lstSearchResults_SelectionChanged;
            //txtSearch.ActionIconTapped += txtSearch_ActionIconTapped;
            watcher = new Geolocator();
            watcher.MovementThreshold = 200;
        }

        //void lstSearchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    Place place = lstSearchResults.SelectedItem as Place;
        //    if (place != null)
        //    {
        //        staticClasses.selectedPlace = place;
        //        NavigationService.Navigate(new Uri("/Pages/Places/SelectedPlacePage.xaml?comingFrom=Search", UriKind.Relative));
        //    }
        //}

        //void watcher_StatusChanged(object sender, GeoPositionStatusChangedEventArgs e)
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

        private void CategoriesPage_Loaded(object sender, RoutedEventArgs e)
        {
            GetGps();
            if (!comingBack)
            {
                //grvCategories.ItemsSource = DBPhone.CategoriesMethods.GetItems();
                comingBack = true;
            }
        }

        private async void GetGps()
        {
            Geoposition position = await watcher.GetGeopositionAsync();
            double latitude = position.Coordinate.Latitude;
            double longitude = position.Coordinate.Longitude;
            //prgSearchProgress.Visibility = System.Windows.Visibility.Visible;
            try
            {
                returned = await PlacesMethods.getCategories();
                if(returned!=null)
                {
                    grvCategories.ItemsSource = returned;
                }
            }
             catch (Exception)
            { /*Don't worry, be happy.*/}
            finally
            {
                //prgSearchProgress.Visibility = System.Windows.Visibility.Collapsed;
                //watcher.Stop();
            }
        }

        private void lstCategoryList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {            
            Category selectedcategory = grvCategories.SelectedItem as Category;
            if (selectedcategory != null)
            {
                if (selectedcategory.name != "")
                {
                    staticClasses.selectedCategory = selectedcategory;
                    this.Frame.Navigate(typeof(SubCategoriesPage));
                    //NavigationService.Navigate(new Uri("/Pages/Places/PlacesPage.xaml", UriKind.Relative));
                    grvCategories.SelectedIndex = -1;
                }
            }
        }

        //#region Search
        //private void txtSearch_ActionIconTapped(object sender, EventArgs e)
        //{
        //    lstSearchResults.ItemsSource = new List<Place>(); 
        //    lstSearchResults.Focus();
        //    if (txtSearch.Text.Length > 0)
        //    {
        //        //Don't touch. SERIOUSLY don't touch.
        //        //It's the only GeoCoordinateWatcher that won't work on the whole App.
        //        try
        //        {
        //            watcher.Start();
        //        }
        //        catch (Exception ex)
        //        { /*Beg so this don't crash again (don't even know why)*/}
        //    }
            
        //    else MessageBox.Show(AppResources.TxtSearchFailed);
        //}

        //private async void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        //{
        //    double latitude = e.Position.Location.Latitude;
        //    double longitude = e.Position.Location.Longitude;
        //    prgSearchProgress.Visibility = System.Windows.Visibility.Visible;
        //    try
        //    {
        //        List<Place> lstReturned = await PlacesMethods.getPlacesByQuery(txtSearch.Text, latitude, longitude);
        //        if (lstReturned != null)
        //                lstSearchResults.ItemsSource = lstReturned;
        //        else MessageBox.Show(AppResources.TxtInternetConnectionProblem);
        //    }
        //    catch (Exception)
        //    { /*Don't worry, be happy.*/}
        //    finally
        //    {
        //        prgSearchProgress.Visibility = System.Windows.Visibility.Collapsed;
        //        watcher.Stop();
        //    }
        //}
        //#endregion

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
    }
}