using System;
using System.Drawing;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace Financer
{
    public class PeopleSource : TableViewSourceBase
    {
        public PeopleSource (Dictionary<string, object[]> items) : base(items)
        {
        }

        public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell (PeopleCell.Key) as PeopleCell;
            if (cell == null) {
                cell = new PeopleCell ();
            }

            var person = this.items.ItemForIndexPath(indexPath);
            cell.UpdateCell (person as Person);

            return cell;
        }
    }
}