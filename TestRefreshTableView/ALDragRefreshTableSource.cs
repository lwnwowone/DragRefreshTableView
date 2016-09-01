using System;
using UIKit;
using Foundation;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestRefreshTableView
{
	public abstract class ALDragRefreshTableSource : UITableViewSource
	{
		private delegate void TaskHandler ();

		public delegate void ScrollingHandler (nfloat locationY);
		public event ScrollingHandler Scrolling;

		public delegate void LoadingHandler (bool isLoading, bool isHeaderLoding);
		public event LoadingHandler Loading;

		private bool isDragging = false;
		public bool IsDragging {
			get {
				return isDragging;
			}
		}

		private bool isHeaderLoding = true;
		private bool isLoading = false;
		public bool IsLoading {
			get {
				return isLoading;
			}
			set {
				isLoading = value;
				if (null != Loading) Loading (isLoading, isHeaderLoding);
			}
		}

		protected nfloat activeDistance = 80;
		public nfloat ActiveDistance {
			get {
				return activeDistance;
			}
			set{ 
				activeDistance = value;
			}
		}

		protected UITableView thisTableView = null;
		public UITableView ThisTableView { 
			set {
				thisTableView = value;
			}
		}

		protected string cellIdentifier = "MyTableViewCell";
		protected List<string> dataList;
		protected Action refreshAction;
		protected Action addMoreAction;

		public ALDragRefreshTableSource (List<string> _dataList,Action _refreshAction,Action _addMoreAction)
		{
			this.dataList = _dataList;
			this.refreshAction = _refreshAction;
			this.addMoreAction = _addMoreAction;
			//dataList = new List<string> () { "January", "February", "March" };
		}

		#region about pull up and drop down

		public void SetDragDistance (nfloat distance)
		{
			activeDistance = distance;
		}

		public override void Scrolled (UIScrollView scrollView)
		{
			if (null != Scrolling) Scrolling (scrollView.ContentOffset.Y);
		}

		public override void DraggingStarted (UIScrollView scrollView)
		{
			isDragging = true;
		}

		public override void DraggingEnded (UIScrollView scrollView, bool willDecelerate)
		{
			isDragging = false;
			//			nfloat offset = scrollView.ContentOffset.Y - (scrollView.ContentSize.Height - scrollView.Frame.Size.Height);
			nfloat footerViewActionDis = thisTableView.Frame.Height >= thisTableView.ContentSize.Height ? scrollView.ContentOffset.Y : scrollView.ContentOffset.Y - (thisTableView.ContentSize.Height - thisTableView.Frame.Height);
			if (!isLoading && footerViewActionDis > activeDistance) LoadMoreData ();
			if (scrollView.ContentOffset.Y < -activeDistance) {
				UIView.Animate (0.3, delegate {
					scrollView.ContentInset = new UIEdgeInsets (thisTableView.RowHeight, 0, 0, thisTableView.RowHeight);
				});
				Refresh ();
			}
		}

		private void LoadMoreData ()
		{
			if (!isLoading) {
				isHeaderLoding = false;
				IsLoading = true;
				//				tableView.RemoveTableHeader();
				AddMoreData (delegate {
					UIApplication.SharedApplication.InvokeOnMainThread (delegate {
						thisTableView.ReloadData ();
						isHeaderLoding = false;
						IsLoading = false;
						//						tableView.CreateTableHeader();
					});
				});
			}
		}

		private void Refresh ()
		{
			if (!isLoading) {
				isHeaderLoding = true;
				IsLoading = true;
				RefreshData (delegate {
					UIApplication.SharedApplication.InvokeOnMainThread (delegate {
						UIView.Animate (0.3, delegate {
							this.thisTableView.ContentInset = new UIEdgeInsets (0, 0, 0, 0);
						});
						thisTableView.ReloadData ();
						isHeaderLoding = true;
						IsLoading = false;
					});
				});
			}
		}

		private void AddMoreData (TaskHandler callback)
		{
			Task task = new Task (() => {
				addMoreAction ();
				//System.Threading.Thread.Sleep (3000);
				//UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				//	dataList.AddRange (new List<string> () { "BAIDU", "GOOGLE", "FACEBOOK", "YAHOO" });
				//});
			});
			task.Start ();
			task.ContinueWith (delegate {
				callback.Invoke ();
			});
		}

		private void RefreshData (TaskHandler callback)
		{
			Task task = new Task (() => {
				refreshAction ();
				//System.Threading.Thread.Sleep (3000);
				//UIApplication.SharedApplication.InvokeOnMainThread (delegate {
				//	Console.WriteLine ("Refresh completed.");
				//});
			});
			task.Start ();
			task.ContinueWith (delegate {
				callback.Invoke ();
			});
		}

		#endregion
	}
}

