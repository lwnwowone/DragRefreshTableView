using System;
using UIKit;
using CoreGraphics;

namespace TestRefreshTableView
{
	public enum HeaderAndFooterStatus
	{
		Display,
		Actived,
		Loading
	}

	public class ALDragRefreshTableView : UITableView
	{
		private DragRefreshTableHeaderView headerView = new DragRefreshTableHeaderView();
		private DragRefreshTableFooterView footerView = new DragRefreshTableFooterView();

		#region AutoResize

		public override CGRect Frame {
			get {
				return base.Frame;
			}
			set {
				RemoveTableHeader ();
				RemoveTableFooter ();
				base.Frame = value;
				if(null != footerView) footerView.Frame = new CGRect (0, 0, this.Bounds.Width, this.RowHeight);
				CreateTableHeader ();
				CreateTableFooter ();
			}
		}

		public override CGPoint ContentOffset {
			get {
				return base.ContentOffset;
			}
			set {
				base.ContentOffset = value;
				if (null != headerView && value.Y<=0) {
					CGRect tmp = headerView.Frame;
					tmp.Height = -value.Y;
					headerView.Frame = tmp;
				}
			}
		}

		public override nfloat RowHeight {
			get {
				return base.RowHeight;
			}
			set {
				RemoveTableHeader ();
				RemoveTableFooter ();
				base.RowHeight = value;
				if(null != footerView) footerView.Frame = new CGRect (0, 0, this.Bounds.Width, this.RowHeight);
				CreateTableHeader ();
				CreateTableFooter ();
			}
		}

		#endregion

		public ALDragRefreshTableView (DragRefreshTableSource theSource)
		{
			this.BackgroundColor = UIColor.Clear;
			SetSource (theSource);
		}

		private void SetSource (DragRefreshTableSource theSource)
		{
			theSource.ThisTableView = this;
			this.Source = theSource;

			theSource.Scrolling += (locationY) => {
				//Console.WriteLine ("locationY=" + locationY);
				nfloat footerViewActionDis = this.Frame.Height >= this.ContentSize.Height ? locationY : locationY - (this.ContentSize.Height - this.Frame.Height);
				if (footerViewActionDis > 0) {
					if (footerViewActionDis > theSource.ActiveDistance) {
						if (footerView.Status == HeaderAndFooterStatus.Display && theSource.IsDragging)
							footerView.Status = HeaderAndFooterStatus.Actived;
					} else {
						if (footerView.Status == HeaderAndFooterStatus.Actived)
							footerView.Status = HeaderAndFooterStatus.Display;
					}
				}
				if (locationY < 0) {
					if (locationY < -theSource.ActiveDistance) {
						if (headerView.Status == HeaderAndFooterStatus.Display && theSource.IsDragging)
							headerView.Status = HeaderAndFooterStatus.Actived;
					} else {
						if (headerView.Status == HeaderAndFooterStatus.Actived)
							headerView.Status = HeaderAndFooterStatus.Display;
					}
				}
			};

			theSource.Loading += (isloading, isHeaderLoding) => {
				if (isloading) {
					this.UserInteractionEnabled = false;
					if (isHeaderLoding) {
						headerView.Status = HeaderAndFooterStatus.Loading;
					} else {
						footerView.Status = HeaderAndFooterStatus.Loading;
					}
				} else {
					this.UserInteractionEnabled = true;
					if (isHeaderLoding) {
						headerView.DefaultText = "last refresh " + DateTime.Now.ToString ();
						headerView.Status = HeaderAndFooterStatus.Display;
					} else
						footerView.Status = HeaderAndFooterStatus.Display;
				}
			};
		}

		public void CreateTableHeader()
		{
			headerView.Frame = new CGRect (0, 0, this.Frame.Width, 0);
			if (null == this.BackgroundView) {
				this.BackgroundView = new UIView ();
			}
			this.BackgroundView.AddSubview (headerView);
		}

		public void RemoveTableHeader()
		{
			headerView.RemoveFromSuperview ();
		}

		public void CreateTableFooter()
		{
//			if (this.Frame.Height < this.ContentSize.Height) {
				this.TableFooterView = null;
				this.TableFooterView = footerView;
//			}
		}

		public void RemoveTableFooter()
		{
			this.TableFooterView = null;
		}

	}

	public class DragRefreshTableHeaderView : UIView
	{
		private string defaultText = "Drop down to refresh";
		public string DefaultText {
			get {
				return defaultText;
			}
			set {
				defaultText = value;
			}
		}

		HeaderAndFooterStatus status = HeaderAndFooterStatus.Display;
		public HeaderAndFooterStatus Status {
			get {
				return status;
			}
			set {
				status = value;
				if (HeaderAndFooterStatus.Display == status) {
					label.Hidden = false;
					activityIndicator.RemoveFromSuperview ();
					label.Text = defaultText;
				}
				else if (HeaderAndFooterStatus.Actived == status) {
					label.Hidden = false;
					activityIndicator.RemoveFromSuperview ();
					label.Text = "Release to refresh";
				}
				else if (HeaderAndFooterStatus.Loading == status) {
					label.Hidden = true;
					this.AddSubview (activityIndicator);
				}
			}
		}	

		public override CGRect Frame {
			get {
				return base.Frame;
			}
			set {
				base.Frame = value;

				label.Frame = this.Bounds;
				int maxThershold = 40;
				int minThershold = 20;
				if (label.Frame.Height < minThershold)
					label.Alpha = 0;
				else if (label.Frame.Height > maxThershold)
					label.Alpha = 1;
				else {
					float alpha = ((float)label.Frame.Height - minThershold) / (maxThershold - minThershold);
					label.Alpha = alpha;
				}

				activityIndicator.Frame = new CGRect((this.Frame.Width-20)/2,(this.Frame.Height-20)/2,20,20);
			}
		}

		UILabel label = new UILabel();
		UIActivityIndicatorView activityIndicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);

		public DragRefreshTableHeaderView()
		{
			this.BackgroundColor = UIColor.Clear;

			label.TextAlignment = UITextAlignment.Center;
			this.AddSubview (label);

			activityIndicator.StartAnimating ();

			Status = HeaderAndFooterStatus.Display;
		}
	}

	public class DragRefreshTableFooterView : UIView
	{
		HeaderAndFooterStatus status = HeaderAndFooterStatus.Display;
		public HeaderAndFooterStatus Status {
			get {
				return status;
			}
			set {
				status = value;
				if (HeaderAndFooterStatus.Display == status) {
					label.Hidden = false;
					activityIndicator.RemoveFromSuperview ();
					label.Text = "Pull up for more";
				}
				else if (HeaderAndFooterStatus.Actived == status) {
					label.Hidden = false;
					activityIndicator.RemoveFromSuperview ();
					label.Text = "Release for more";
				}
				else if (HeaderAndFooterStatus.Loading == status) {
					label.Hidden = true;
					this.AddSubview (activityIndicator);
				}
			}
		}	

		UILabel label = new UILabel();
		UIActivityIndicatorView activityIndicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);

		public DragRefreshTableFooterView()
		{
			this.BackgroundColor = UIColor.Clear;

			label.TextAlignment = UITextAlignment.Center;
			this.AddSubview (label);

			activityIndicator.StartAnimating ();

			Status = HeaderAndFooterStatus.Display;
		}

		public override void LayoutSubviews ()
		{
			label.Frame = this.Bounds;
			activityIndicator.Frame = new CGRect((this.Frame.Width-20)/2,(this.Frame.Height-20)/2,20,20);
		}
	}
}

