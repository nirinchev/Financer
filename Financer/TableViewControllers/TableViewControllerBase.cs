using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.Foundation;
using System.Drawing;

namespace Financer
{
    public abstract class TableViewControllerBase<T1, T2> : UITableViewController
    {
        protected const string OldSegueIdentifier = "Old";

        private LazyInvoker lazySearchTimer;
        protected TableViewSourceBase<T1, T2> tableViewSource;

        protected abstract UISearchBar SearchBar { get; set; }

        protected abstract UIBarButtonItem AddItemButton { get; set; }

        private Dictionary<T1, T2[]> filteredItems;
        public Dictionary<T1, T2[]> FilteredItems { 
            get {
                return this.filteredItems;
            }
            protected set {
                if (value != this.filteredItems) {
                    this.filteredItems = value;
                    if (this.tableViewSource != null) {
                        this.tableViewSource.Update (value);
                        this.TableView.ReloadData ();
                    }
                }
            }
        }

        private Action<T2> selectionCallback;
        public Action<T2> SelectionCallback { 
            get {
                return this.selectionCallback;
            }
            set {
                if (value != this.selectionCallback) {
                    this.selectionCallback = value;
                    this.AddItemButton.Enabled = value == null;
                }
            }
        }

        private T2 selectedItem;
        protected T2 SelectedItem { 
            get {
                return this.selectedItem;
            }
            set {
                if (!value.Equals(this.selectedItem)) {
                    this.selectedItem = value;
                    if (this.SelectionCallback != null) {
                        this.SelectionCallback (value);
                    } else {
                        this.PerformSegue (OldSegueIdentifier, this);
                    }
                }
            }
        }


        public TableViewControllerBase ()
        {
            this.Initialize();
        }

        public TableViewControllerBase (IntPtr handle) : base (handle)
        {
            this.Initialize();
        }

        private void Initialize()
        {
            this.FilteredItems = new Dictionary<T1, T2[]> ();
            this.InitializeTableViewSource (this.FilteredItems);
            this.tableViewSource.Callback = (item) => {
                this.SelectedItem = item;
            };
            this.lazySearchTimer = new LazyInvoker (0.5, this.UpdateFilteredItems);
        }

        protected abstract void InitializeTableViewSource (Dictionary<T1, T2[]> items);

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            this.TableView.Source = this.tableViewSource;

            this.SearchBar.TextChanged += this.HandleSearchBarTextChanged;
        }

        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);
            this.TableView.SetContentOffset (new PointF (0, 44), true);
            this.UpdateFilteredItems ();
        }

        private void HandleSearchBarTextChanged (object sender, UISearchBarTextChangedEventArgs e)
        {
            lazySearchTimer.Run ();
        }

        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);
        }

        protected abstract void UpdateFilteredItems ();
    }
}