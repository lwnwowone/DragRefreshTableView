using System;
using UIKit;
using Foundation;
using System.Collections.Generic;
using CoreGraphics;
using System.Threading.Tasks;

namespace TestRefreshTableView
{
	public class DragRefreshTableSource : AlancDragRefreshTableSource
	{
		public DragRefreshTableSource (List<string> dataList, Action refreshAction, Action addMoreAction) : base(dataList,refreshAction,addMoreAction)
		{
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			return 1;
		}

		public override nint RowsInSection (UITableView tableview, nint section)
		{
			return dataList.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			UITableViewCell cell = tableView.DequeueReusableCell (cellIdentifier);

			if (cell == null) 
				cell = new UITableViewCell(UITableViewCellStyle.Default,cellIdentifier);

			cell.TextLabel.Text = dataList[indexPath.Row];

			cell.BackgroundColor = UIColor.Clear;

			return cell;
		}
	}
}

