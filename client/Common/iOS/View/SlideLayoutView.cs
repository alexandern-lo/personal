using System;
using UIKit;
using System.Drawing;
using CoreGraphics;
using Foundation;
using System.Diagnostics;
using StudioMobile;
using SL4N;

namespace StudioMobile
{
	public enum SlideLayoutLocation
	{
		Left,
		Right,
		Center,
		None
	}

	public enum SlideTransitionStyle
	{
		Reveal, Paginate
	}

	public class DisplaySlideViewEventArgs
	{
		public bool Animated { get; internal set; }
		public UIView TargetView { get; internal set; }
		public SlideLayoutLocation TargetLocation { get; internal set; }
	}

	[Register("SlideLayoutView")]
	public class SlideLayoutView : UIView
	{
		UIView presentedView;

		public SlideLayoutView()
		{
			CreateView();
		}

		public SlideLayoutView(IntPtr handle) : base(handle)
		{
			CreateView();
		}

		void CreateView()
		{
			LeftWidth = RightWidth = (float)UIScreen.MainScreen.ApplicationFrame.Width * 0.9f;
			Style = SlideTransitionStyle.Reveal;

			tapGesture = new UITapGestureRecognizer()
			{
				NumberOfTapsRequired = 1,
				CancelsTouchesInView = true,
				DelaysTouchesBegan = true,
				DelaysTouchesEnded = true
			};
			tapGesture.AddTarget(() => DismissPresentedView(true));

			panGesture = new UIPanGestureRecognizer()
			{
				MaximumNumberOfTouches = 1,
				MinimumNumberOfTouches = 1,
				CancelsTouchesInView = true,
				DelaysTouchesBegan = true,
				DelaysTouchesEnded = true,
				ShouldReceiveTouch = (_1, _2) => true,
				ShouldBegin = PanShouldBegin
			};
			panGesture.AddTarget(() => OnPan());
			AddGestureRecognizer(panGesture);

			VelocityThreshold = 800f;
			TranslateThreshold = 50f;
			SlideSpeed = 0.2f;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				Cleanup.All(left, right, content);
			}
			base.Dispose(disposing);
		}

		private UIView left, right, content;

		[Outlet]
		public UIView Left
		{
			get { return left; }
			set
			{
				left = SetView(value, SlideLayoutLocation.Left);
			}
		}

		[Outlet]
		public UIView Right
		{
			get { return right; }
			set
			{
				right = SetView(value, SlideLayoutLocation.Right);
			}
		}

		[Outlet]
		public UIView Content
		{
			get { return content; }
			set
			{
				content = SetView(value, SlideLayoutLocation.Center);
			}
		}

		internal UIView SetView(UIView view, SlideLayoutLocation location)
		{
			if (view == null)
				throw new ArgumentNullException();
			var prevView = GetViewForLocation(location);
			if (prevView != null)
			{
				if (prevView == view)
				{
					DismissPresentedView(false);
				}
				prevView.RemoveFromSuperview();
			}

			if (view != null)
			{
				AddSubview(view);
			}

			switch (location)
			{
				case SlideLayoutLocation.Left:
					left = view;
					if (content != null)
					{
						InsertSubviewBelow(left, content);
					}
					break;
				case SlideLayoutLocation.Right:
					right = view;
					if (content != null)
					{
						InsertSubviewBelow(right, content);
					}
					break;
				case SlideLayoutLocation.Center:
					content = view;
					break;
			}

			SetNeedsLayout();
			return view;
		}

		public SlideTransitionStyle Style
		{
			get;
			set;
		}

		private float leftWidth;

		public float LeftWidth
		{
			get { return leftWidth; }
			set
			{
				if (value >= 0)
				{
					leftWidth = value;
					SetNeedsLayout();
				}
			}
		}

		private float rightWidth;

		public float RightWidth
		{
			get { return rightWidth; }
			set
			{
				if (value >= 0)
				{
					rightWidth = value;
					SetNeedsLayout();
				}
			}
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			switch (Style)
			{
				case SlideTransitionStyle.Paginate:
					LayoutForPaginate();
					break;
				case SlideTransitionStyle.Reveal:
					LayoutForReveal();
					break;
			}
		}

		private void LayoutForReveal()
		{
			if (Left != null)
			{
				Left.Bounds = new CGRect(0, 0, leftWidth, Bounds.Height);
				Left.Center = new CGPoint(leftWidth / 2, Bounds.Height / 2);
			}
			if (Right != null)
			{
				Right.Bounds = new CGRect(0, 0, rightWidth, Bounds.Height);
				Right.Center = new CGPoint(Bounds.Width - rightWidth / 2, Bounds.Height / 2);
			}
			if (Content != null)
			{
				Content.Bounds = Bounds;
				Content.Center = new CGPoint(Bounds.Width / 2, Bounds.Height / 2);
			}
		}

		private void LayoutForPaginate()
		{
			if (Left != null)
			{
				Left.Bounds = new CGRect(0, 0, leftWidth, Bounds.Height);
				Left.Center = new CGPoint(-leftWidth / 2, Bounds.Height / 2);
			}
			if (Right != null)
			{
				Right.Bounds = new CGRect(0, 0, rightWidth, Bounds.Height);
				Right.Center = new CGPoint(Bounds.Width + rightWidth / 2, Bounds.Height / 2);
			}
			if (Content != null)
			{
				Content.Bounds = Bounds;
				Content.Center = new CGPoint(Bounds.Width / 2, Bounds.Height / 2);
			}
		}

		private UITapGestureRecognizer tapGesture;

		public UITapGestureRecognizer TapGestureRecognizer { get { return tapGesture; } }

		private UIPanGestureRecognizer panGesture;

		public UIPanGestureRecognizer PanGestureRecognizer { get { return panGesture; } }

		private enum PanAction
		{
			Open,
			Close,
			None
		}

		private PanAction GetPanAction(SlideLayoutLocation target)
		{
			if (target == SlideLayoutLocation.None || target == SlideLayoutLocation.Center)
				return PanAction.None;

			if (presentedView != null)
			{
				return PanAction.Close;
			}
			else {
				return PanAction.Open;
			}
		}

		private SlideLayoutLocation GetPanTarget(CGPoint velocity)
		{
			SlideLayoutLocation target = SlideLayoutLocation.None;
			if (presentedView != null)
			{
				target = GetLocationForView(presentedView);
			}
			else {
				target = velocity.X > 0 ? SlideLayoutLocation.Left : SlideLayoutLocation.Right;
				if (!HasViewForLocation(target))
				{
					target = SlideLayoutLocation.None;
				}
			}
			return target;
		}

		private bool ShouldPerformPanActionWithTarget(CGPoint translate, PanAction action, SlideLayoutLocation target)
		{
			var velocity = panGesture.VelocityInView(this);
			var absDX = Math.Abs(translate.X);
			bool thresholdsMatch = velocity.X > VelocityThreshold || absDX > TranslateThreshold;
			bool directionMatch = false;
			switch (action)
			{
				case PanAction.Open:
					{
						switch (target)
						{
							case SlideLayoutLocation.Left:
								directionMatch = translate.X > 0;
								break;
							case SlideLayoutLocation.Right:
								directionMatch = translate.X < 0;
								break;
						}
						break;
					}
				case PanAction.Close:
					{
						switch (target)
						{
							case SlideLayoutLocation.Left:
								directionMatch = translate.X < 0;
								break;
							case SlideLayoutLocation.Right:
								directionMatch = translate.X > 0;
								break;
						}
						break;
					}
			}
			return thresholdsMatch && directionMatch;
		}

		private CGPoint GetPanOrigin(PanAction action, SlideLayoutLocation target)
		{
			if (action == PanAction.Close)
			{
				return new PointF(GetTargetTranslationForLocation(target), 0);
			}
			else {
				return Bounds.Location;
			}
		}

		private PanAction panAction;
		private SlideLayoutLocation panTarget;
		private CGPoint panOrigin;

		private bool PanShouldBegin(UIGestureRecognizer recognizer)
		{
			var translate = panGesture.VelocityInView(this);
			var panTarget = GetPanTarget(translate);
			return panTarget != SlideLayoutLocation.None;
		}

		private void OnPan()
		{
			var velocity = panGesture.VelocityInView(this);
			var t = panGesture.TranslationInView(UIApplication.SharedApplication.KeyWindow);
			switch (panGesture.State)
			{
				case UIGestureRecognizerState.Began:
					{
						panTarget = GetPanTarget(velocity);
						panAction = GetPanAction(panTarget);
						panOrigin = GetPanOrigin(panAction, panTarget);
						if (panTarget != SlideLayoutLocation.None)
						{
							var view = GetViewForLocation(panTarget);
							var oppositeView = GetViewForLocation(GetOppositeLocation(panTarget));
							if (oppositeView != null)
							{
								InsertSubviewAbove(view, oppositeView);
							}
						}
						break;
					}
				case UIGestureRecognizerState.Changed:
					{
						var translation = panGesture.TranslationInView(this);
						double dx = 0;
						switch (panTarget)
						{
							case SlideLayoutLocation.Left:
								{
									dx = Math.Max(panOrigin.X + translation.X, 0);
									break;
								}
							case SlideLayoutLocation.Right:
								{
									dx = Math.Min(panOrigin.X + translation.X, 0);
									break;
								}
						}
						SlideView(dx);
						break;
					}
				case UIGestureRecognizerState.Cancelled:
				case UIGestureRecognizerState.Ended:
				case UIGestureRecognizerState.Failed:
					{
						var performAction = ShouldPerformPanActionWithTarget(velocity, panAction, panTarget);
						switch (panAction)
						{
							case PanAction.Open:
								if (performAction)
								{
									PresentView(panTarget);
								}
								else {
									DismissPresentedView(true);
								}
								break;
							case PanAction.Close:
								if (performAction)
								{
									DismissPresentedView(true);
								}
								else {
									PresentView(panTarget);
								}
								break;
						}
						break;
					}
				case UIGestureRecognizerState.Possible:
				default:
					break;
			}
		}

		private void SlideView(double dx)
		{
			if (dx > LeftWidth || dx < -RightWidth) {
				return;
			}
			var transform = CGAffineTransform.MakeTranslation((nfloat)dx, 0);
			switch (Style)
			{
				case SlideTransitionStyle.Reveal:
					Content.Transform = transform;
					break;
				case SlideTransitionStyle.Paginate:
					Content.Transform = transform;
					if (left != null)
						left.Transform = transform;
					if (right != null)
						right.Transform = transform;
					break;
			}
		}

		public UIView GetViewForLocation(SlideLayoutLocation location)
		{
			switch (location)
			{
				case SlideLayoutLocation.Left:
					return Left;
				case SlideLayoutLocation.Right:
					return Right;
				case SlideLayoutLocation.Center:
					return Content;
				case SlideLayoutLocation.None:
				default:
					return null;
			}
		}

		private SlideLayoutLocation GetLocationForView(UIView view)
		{
			if (view == Left)
				return SlideLayoutLocation.Left;
			if (view == Right)
				return SlideLayoutLocation.Right;
			if (view == Content)
				return SlideLayoutLocation.Center;
			return SlideLayoutLocation.None;
		}

		private SlideLayoutLocation GetOppositeLocation(SlideLayoutLocation location)
		{
			switch (location)
			{
				case SlideLayoutLocation.Left:
					return SlideLayoutLocation.Right;
				case SlideLayoutLocation.Right:
					return SlideLayoutLocation.Left;
				default:
					return SlideLayoutLocation.None;
			}
		}

		private float GetTargetTranslationForLocation(SlideLayoutLocation location)
		{
			switch (location)
			{
				case SlideLayoutLocation.Left:
					return LeftWidth;
				case SlideLayoutLocation.Right:
					return -RightWidth;
				case SlideLayoutLocation.Center:
				case SlideLayoutLocation.None:
				default:
					return 0;
			}
		}

		private bool HasViewForLocation(SlideLayoutLocation location)
		{
			return GetViewForLocation(location) != null;
		}

		public float VelocityThreshold { get; set; }

		public float TranslateThreshold { get; set; }

		public event EventHandler<DisplaySlideViewEventArgs> WillPresentView;
		public event EventHandler<DisplaySlideViewEventArgs> DidPresentView;

		public void PresentView(SlideLayoutLocation location, bool animated = true)
		{
			if (location == SlideLayoutLocation.Center)
				throw new ArgumentException("location");
			if (!HasViewForLocation(location))
				return;
			var contentView = this.Content;
			var targetView = GetViewForLocation(location);
			var oppositeView = GetViewForLocation(GetOppositeLocation(location));
			var nowPresentedView = presentedView;

			var eventArgs = new DisplaySlideViewEventArgs()
			{
				TargetView = targetView,
				TargetLocation = location,
				Animated = animated
			};
			//if controller is not yet presented then we are about to show it
			if (targetView != nowPresentedView)
			{
				if (WillPresentView != null)
				{
					WillPresentView(this, eventArgs);
				}
			}
			//if opposite menu was presented then dismiss it
			if (oppositeView == nowPresentedView && nowPresentedView != null)
			{
				DismissPresentedView(animated);
			}
			//if both menus are specified then ensure the menu we are about to show is visible
			if (oppositeView != null)
			{
				InsertSubviewAbove(targetView, oppositeView);
			}

			presentedView = targetView;

			Action slide = () =>
			{
				var dx = GetTargetTranslationForLocation(location);
				SlideView(dx);
			};
			Action done = () =>
			{
				if (targetView != nowPresentedView)
				{
					if (contentView != null)
					{
						contentView.AddGestureRecognizer(tapGesture);
						foreach (var subview in contentView.Subviews)
						{
							subview.UserInteractionEnabled = false;
						}
					}
					if (DidPresentView != null)
					{
						DidPresentView(this, eventArgs);
					}
				}
			};

			if (animated)
			{
				UIView.Animate(
					SlideSpeed,
					0,
					UIViewAnimationOptions.CurveEaseInOut,
					slide,
					done);
			}
			else {
				slide();
				done();
			}
		}

		public event EventHandler<DisplaySlideViewEventArgs> WillDismissView;
		public event EventHandler<DisplaySlideViewEventArgs> DidDismissView;

		public void DismissPresentedView(bool animated = true)
		{
			//if no controller is presented then just revert content controller to original state
			var nowPresentedView = presentedView;
			var contentController = this.Content;

			presentedView = null;

			var eventArgs = new DisplaySlideViewEventArgs()
			{
				TargetView = nowPresentedView,
				TargetLocation = GetLocationForView(presentedView),
				Animated = animated
			};

			if (nowPresentedView != null)
			{
				if (WillDismissView != null)
				{
					WillDismissView(this, eventArgs);
				}
			}

			Action slideBack = () =>
			{
				SlideView(0);
			};
			Action done = () =>
			{
				if (contentController != null)
				{
					contentController.RemoveGestureRecognizer(tapGesture);
					foreach (var subview in contentController.Subviews)
					{
						subview.UserInteractionEnabled = true;
					}
				}
				if (nowPresentedView != null)
				{
					if (DidDismissView != null)
					{
						DidDismissView(this, eventArgs);
					}
				}
			};
			if (animated)
			{
				UIView.Animate(
					SlideSpeed,
					0,
					UIViewAnimationOptions.CurveEaseInOut,
					slideBack,
					done);
			}
			else {
				slideBack();
				done();
			}
		}

		public float SlideSpeed { get; set; }
	}

}
