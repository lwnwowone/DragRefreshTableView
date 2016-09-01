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

			//Origin data in tableview
			dataList = new List<string> () { "January", "February", "March", "April" };

			//datalist : tableview's datalist
			//Refresh : It's an action which will fire when drag down tableview event actived
			//Refresh : It's an action which will fire when pull up tableview event actived
			DragRefreshTableSource source = new DragRefreshTableSource (dataList,Refresh,MoreData);
			source.ActiveDistance = 60;//Set an active distance

			ALDragRefreshTableView tableView = new ALDragRefreshTableView(source);
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
				dataList.AddRange (new List<string> () { "Apple", "Google", "FaceBook", "MicroSoft" });
			});
		}
	}
}



