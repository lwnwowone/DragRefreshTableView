using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;

namespace TestRefreshTableView
{
	public class TestViewController : UIViewController
	{
		List<string> dataList;

		public override void ViewDidLoad ()
		{
			this.View.BackgroundColor = UIColor.White;

			CGRect tmp = this.View.Bounds;
			tmp.Y = 20;
			tmp.Height = tmp.Height - 20;

			dataList = new List<string> () { "January", "February", "March" };
			DragRefreshTableSource source = new DragRefreshTableSource (dataList,Refresh,MoreData);
			AlancDragRefreshTableView tableView = new AlancDragRefreshTableView(source);
			tableView.Frame = tmp;
			tableView.RowHeight = 50;
			this.Add(tableView);
		}

		private void Refresh ()
		{
			System.Threading.Thread.Sleep (3000);
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				Console.WriteLine ("Refresh completed.");
			});
		}
			                                                    
		private void MoreData ()
		{
			System.Threading.Thread.Sleep (3000);
			UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				dataList.AddRange (new List<string> () { "BAIDU", "GOOGLE", "FACEBOOK", "YAHOO" });
			});
		}
	}
}



