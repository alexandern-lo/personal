using System;
using System.Collections.Generic;
using CoreGraphics;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View
{
	public enum CellViewGravity { 
		Center,
		Top,
		Right,
		Bottom,
		Left,
		TopLeft,
		TopRight,
		BottomRight,
		BottomLeft
	}

	public enum CellViewWidthType {
		LeftSegment,
		RightSegment,
		CenterSegment,
		Separator,
		LeftSegmentMultiplier,
		RightSegmentMultiplier,
		CenterSegmentMultiplier,
		SeparatorMultiplier
	}

	public class CellView : HighlightButton
	{
		public CellView(IntPtr handle) : base (handle)
		{
			Initialize();
		}

		public CellView()
		{
			Initialize();
		}

		void Initialize()
		{
			// Default settings
			EnableLeftIcon = true;
			EnableRightIcon = false;
			EnableSeparator = false;

			LeftIconSize = new CGSize(28.0f, 28.0f);
			RightIconSize = new CGSize(20.0f, 20.0f);

			LeftImageGravity = CellViewGravity.Center;
			RightImageGravity = CellViewGravity.Center;
			TitleLabelGravity = CellViewGravity.Center;
			SeparatorGravity = CellViewGravity.BottomRight;

			LeftImageMargin = new UIEdgeInsets(0, 0, 0, 0);
			RightImageMargin = new UIEdgeInsets(0, 0, 0, 0);
			TitleLabelMargin = new UIEdgeInsets(0, 0, 0, 0);
			SeparatorMargin = new UIEdgeInsets(0, 0, 0, 0);

			SeparatorView.BackgroundColor = UIColor.Black.ColorWithAlpha(0.1f);
			SetTitleColor(UIColor.Black, UIControlState.Normal);
			LeftIconImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			RightIconImageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			TitleLabel.LineBreakMode = UILineBreakMode.TailTruncation;
		}

		[View]
		public UIImageView LeftIconImageView { get; private set; }

		[View]
		public UIImageView RightIconImageView { get; private set; }

		[View]
		public UIView SeparatorView { get; private set; }

		public CGSize LeftIconSize { get; set; }
		public CGSize RightIconSize { get; set; }

		public bool EnableLeftIcon { 
			get { return !LeftIconImageView.Hidden; }
			set { LeftIconImageView.Hidden = !value; }
		}
		public bool EnableRightIcon {
			get { return !RightIconImageView.Hidden; }
			set { RightIconImageView.Hidden = !value; }
		}
		public bool EnableSeparator {
			get { return !SeparatorView.Hidden; }
			set { SeparatorView.Hidden = !value; }
		}

		public CellViewGravity LeftImageGravity { get; set; }
		public CellViewGravity RightImageGravity { get; set; }
		public CellViewGravity TitleLabelGravity { get; set; }
		public CellViewGravity SeparatorGravity { get; set; }

		public UIEdgeInsets LeftImageMargin { get; set; }
		public UIEdgeInsets RightImageMargin { get; set; }
		public UIEdgeInsets TitleLabelMargin { get; set; }
		public UIEdgeInsets SeparatorMargin { get; set; }

		public void AddFixedWidth(CellViewWidthType widthType, nfloat width)
		{
			var fixedWidth = new FixedWidth()
			{
				WidthType = widthType,
				WidthValue = width
			};
			FixedWidths.Add(fixedWidth);
		}

		public nfloat MinWidthForHeight(nfloat height)
		{
			CGSize size = TitleLabel.SizeThatFits(new CGSize(nfloat.MaxValue, height));
			var minLeftSegmentWidth = EnableLeftIcon ? LeftImageMargin.Left + LeftImageMargin.Right + LeftIconSize.Width : 0f;
			var minRightSegmentWidth = EnableRightIcon ? RightImageMargin.Left + RightImageMargin.Right + RightIconSize.Width : 0f;
			return minLeftSegmentWidth + minRightSegmentWidth + size.Width;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			nfloat pW = Bounds.Width;
			nfloat pH = Bounds.Height;

			nfloat LeftSegmentWidth = LeftIconSize.Width + LeftImageMargin.Left + LeftImageMargin.Right;
			nfloat RightSegmentWidth = RightIconSize.Width + RightImageMargin.Left + RightImageMargin.Right;
			nfloat CenterSegmentWidth = 0.0f;
			nfloat SeparatorWidth = pW;
			bool fixedLeft = false;
			bool fixedRight = false;
			bool fixedCenter = false;

			if (!EnableLeftIcon)
			{
				LeftSegmentWidth = 0.0f;
				fixedLeft = true;
			}

			if (!EnableRightIcon)
			{
				RightSegmentWidth = 0.0f;
				fixedRight = true;
			}

			foreach (FixedWidth fixedWidth in FixedWidths)
			{
				switch (fixedWidth.WidthType) {
					case CellViewWidthType.LeftSegment: LeftSegmentWidth = fixedWidth.WidthValue; fixedLeft = true; break;
					case CellViewWidthType.RightSegment: RightSegmentWidth = fixedWidth.WidthValue; fixedRight = true; break;
					case CellViewWidthType.CenterSegment: CenterSegmentWidth = fixedWidth.WidthValue; fixedCenter = true; break;
					case CellViewWidthType.Separator: SeparatorWidth = fixedWidth.WidthValue; break;
					case CellViewWidthType.LeftSegmentMultiplier: LeftSegmentWidth = fixedWidth.WidthValue * pW; fixedLeft = true; break;
					case CellViewWidthType.RightSegmentMultiplier: RightSegmentWidth = fixedWidth.WidthValue * pW; fixedRight = true; break;
					case CellViewWidthType.CenterSegmentMultiplier: CenterSegmentWidth = fixedWidth.WidthValue * pW; fixedCenter = true; break;
					case CellViewWidthType.SeparatorMultiplier: SeparatorWidth = fixedWidth.WidthValue * pW; break; 
				}
			}

			LeftIconImageView.Frame = new CGRect(new CGPoint(0, 0), LeftIconSize);
			RightIconImageView.Frame = new CGRect(new CGPoint(0, 0), RightIconSize);
			SeparatorView.Frame = new CGRect(0, 0, SeparatorWidth, 1f);
			TitleLabel.SizeToFit();

			if (fixedCenter)
			{
				if (fixedRight)
				{
					LeftSegmentWidth = pW - CenterSegmentWidth - RightSegmentWidth;
				}
				else {
					if (fixedLeft)
					{
						RightSegmentWidth = pW - LeftSegmentWidth - CenterSegmentWidth;
					}
					else {
						LeftSegmentWidth = RightSegmentWidth = (pW - CenterSegmentWidth) / 2;
					}
				}
			}
			else {
				CenterSegmentWidth = pW - LeftSegmentWidth - RightSegmentWidth;
			}

			var LeftSectionFrame = CustomizeFrameForInsets(new CGRect(0, 0, LeftSegmentWidth, pH), LeftImageMargin);
			var RightSectionFrame = CustomizeFrameForInsets(new CGRect(LeftSegmentWidth + CenterSegmentWidth, 0, RightSegmentWidth, pH), RightImageMargin);
			var CenterSectionFrame = CustomizeFrameForInsets(new CGRect(LeftSegmentWidth, 0, CenterSegmentWidth, pH), TitleLabelMargin);
			var SeparatorFrame = CustomizeFrameForInsets(new CGRect(0, 0, pW, pH), SeparatorMargin);

			TitleLabel.SizeThatFits(new CGSize(CenterSectionFrame.Width, CenterSectionFrame.Height));
			TitleLabel.Frame = new CGRect(0,0, CenterSectionFrame.Width, TitleLabel.Frame.Height);

			TitleLabel.TextAlignment = UITextAlignment.Left;
			if (TitleLabelGravity == CellViewGravity.Center || TitleLabelGravity == CellViewGravity.Top || TitleLabelGravity == CellViewGravity.Bottom)
			{
				TitleLabel.TextAlignment = UITextAlignment.Center;
			}
			if (TitleLabelGravity == CellViewGravity.Right || TitleLabelGravity == CellViewGravity.TopRight || TitleLabelGravity == CellViewGravity.BottomRight)
			{
				TitleLabel.TextAlignment = UITextAlignment.Right;
			}

			PositionViewInFrame(LeftIconImageView, LeftSectionFrame, LeftImageGravity);
			PositionViewInFrame(RightIconImageView, RightSectionFrame, RightImageGravity);
			PositionViewInFrame(TitleLabel, CenterSectionFrame, TitleLabelGravity);
			PositionViewInFrame(SeparatorView, SeparatorFrame, SeparatorGravity);
		}

		CGRect CustomizeFrameForInsets(CGRect frame, UIEdgeInsets insets)
		{ 
			frame.X += insets.Left;
			frame.Y += insets.Top;
			frame.Width -= (insets.Left + insets.Right);
			frame.Height -= (insets.Top + insets.Bottom);
			return frame;
		}

		void PositionViewInFrame(UIView view, CGRect frame, CellViewGravity gravity)
		{
			var pW = frame.Width;
			var pH = frame.Height;
			var w = view.Frame.Width;
			var h = view.Frame.Height;
			nfloat xPosition = 0.0f;
			nfloat yPosition = 0.0f;

			if (gravity == CellViewGravity.Center || gravity == CellViewGravity.Top || gravity == CellViewGravity.Bottom)
			{
				xPosition = (pW - w) / 2.0f;
			}

			if (gravity == CellViewGravity.Right || gravity == CellViewGravity.TopRight || gravity == CellViewGravity.BottomRight)
			{
				xPosition = pW - w;
			}

			if (gravity == CellViewGravity.Center || gravity == CellViewGravity.Left || gravity == CellViewGravity.Right)
			{
				yPosition = (pH - h) / 2.0f;
			}

			if (gravity == CellViewGravity.Bottom || gravity == CellViewGravity.BottomLeft || gravity == CellViewGravity.BottomRight)
			{
				yPosition = pH - h;
			}

			view.Frame = new CGRect(frame.X + xPosition, frame.Y + yPosition, w, h);
		}

		List<FixedWidth> FixedWidths = new List<FixedWidth>();
		class FixedWidth
		{
			public CellViewWidthType WidthType { get; set; }
			public nfloat WidthValue { get; set; }
		}
	}
}
