﻿using System;
using System.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using BrainShare.Models;
using BrainShare.Common;
using BrainShare.Database;
using Windows.UI.Popups;


// The Split Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234234

namespace BrainShare.Views
{
    /// <summary>
    /// A page that displays a group title, a list of items within the group, and details for
    /// the currently selected item.
    /// </summary>
    public sealed partial class LibraryBookReader : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private StorageFile loadedFile;
        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return navigationHelper; }
        }
        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return defaultViewModel; }
        }
        public LibraryBookReader()
        {
            InitializeComponent();
            // Setup the navigation helper
            navigationHelper = new NavigationHelper(this);
            navigationHelper.LoadState += navigationHelper_LoadState;
            navigationHelper.SaveState += navigationHelper_SaveState;
            navigationHelper.GoBackCommand = new RelayCommand(() => GoBack(), () => CanGoBack());
            itemListView.SelectionChanged += ItemListView_SelectionChanged;
            // Start listening for Window size changes 
            // to change from showing two panes to showing a single pane
            Window.Current.SizeChanged += Window_SizeChanged;
            InvalidateVisualState();
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
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            var file = e.NavigationParameter as BookObservable;
            bool found = false;
            bool download = false;
            bool fullydownloaded = false;
            try
            {
                await CommonTask.DeleteTemporaryFiles();
            }
            catch
            {

            }
            if (CommonTask.IsInternetConnectionAvailable())
            {
                found = true;
                try
                {
                    loadedFile = await Constants.appFolder.GetFileAsync(file.file_url);
                    await CommonTask.LoadPdfFileAsync(loadedFile, DefaultViewModel, ActualWidth);
                }
                catch
                {
                    found = false;
                }
                if (found == false)
                {
                    var messageDialog = new MessageDialog(Message.File_Access_Message, Message.File_Access_Header);
                    messageDialog.Commands.Add(new UICommand(Message.Yes, (command) =>
                    {
                        download = true;
                    }));
                    messageDialog.Commands.Add(new UICommand(Message.No, (command) =>
                    {
                        download = false;
                    }));

                    messageDialog.DefaultCommandIndex = 1;
                    await messageDialog.ShowAsync();

                    if (download == true)
                    {
                        loadingRing.IsActive = true;
                        LoadingMsg.Visibility = Visibility.Visible;
                        try
                        {
                            await CommonTask.FileDownloader(file.file_url, file.book_title);
                            fullydownloaded = true;
                        }
                        catch (Exception ex)
                        {
                            string err = ex.ToString();
                            fullydownloaded = false;
                        }
                        if (fullydownloaded == true)
                        {
                            loadingRing.IsActive = false;
                            LoadingMsg.Visibility = Visibility.Collapsed;
                            using (var db = new SQLite.SQLiteConnection(Constants.dbPath))
                            {
                                var query = (db.Table<Book>().Where(c => c.Book_id == file.book_id)).Single();
                                string newPath = query.Book_title + Constants.PDF_extension;
                                Book fileDownloaded = new Book(query.Book_id, query.Book_title, query.Book_author, query.Book_description,
                                    query.updated_at, query.thumb_url, query.file_size, query.Library_id, query.Category_id, query.Category_name, newPath);
                                db.Update(fileDownloaded);
                                file.file_url = newPath;
                            }
                            loadedFile = await Constants.appFolder.GetFileAsync(file.file_url);
                            await CommonTask.LoadPdfFileAsync(loadedFile, DefaultViewModel, ActualWidth);
                        }
                        else if (fullydownloaded == false)
                        {
                            var message = new MessageDialog(Message.Download_Error, Message.Download_Header).ShowAsync();
                        }
                    }
                }
            }
            else
            {
                found = true;
                try
                {
                    loadedFile = await Constants.appFolder.GetFileAsync(file.file_url);
                    await CommonTask.LoadPdfFileAsync(loadedFile, DefaultViewModel, ActualWidth);
                }
                catch
                {
                    found = false;
                }
                if (found == false)
                {
                    var messageDialog = new MessageDialog(Message.Offline_File_Unavailable, Message.File_Access_Header).ShowAsync();
                }
            }
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="e">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {

            if (itemsViewSource.View != null)
            {
                var selectedItem = (PdfDataItem)itemsViewSource.View.CurrentItem;
                if (selectedItem != null) e.PageState["SelectedItem"] = selectedItem.UniqueId;
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

            InvalidateVisualState();
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
            if (UsingLogicalPageNavigation()) InvalidateVisualState();
        }
        private bool CanGoBack()
        {
            if (UsingLogicalPageNavigation() && itemListView.SelectedItem != null)
            {
                return true;
            }
            else
            {
                return navigationHelper.CanGoBack();
            }

        }
        private void GoBack()
        {

            if (UsingLogicalPageNavigation() && itemListView.SelectedItem != null)
            {
                // When logical page navigation is in effect and there's a selected item that
                // item's details are currently displayed.  Clearing the selection will return to
                // the item list.  From the user's point of view this is a logical backward
                // navigation.
                itemListView.SelectedItem = null;
            }
            else
            {
                navigationHelper.GoBack();
            }
        }
        private void InvalidateVisualState()
        {
            var visualState = DetermineVisualState();
            VisualStateManager.GoToState(this, visualState, false);
            navigationHelper.GoBackCommand.RaiseCanExecuteChanged();
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
            var logicalPageBack = UsingLogicalPageNavigation() && itemListView.SelectedItem != null;

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