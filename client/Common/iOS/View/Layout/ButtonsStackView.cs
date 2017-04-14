using System;
using UIKit;
using Foundation;
using System.Collections.Generic;
using CoreGraphics;

namespace StudioMobile
{
	public interface IButtonsStackDelegate 
	{
		void ActionPerformed(ButtonsStackView cell, int buttonIdx);
	}

	public class ButtonsStackView : StackPanelView 
	{
		List<UIButton> buttons = new List<UIButton>();

		public ButtonsStackView ()
		{
		}

		public ButtonsStackView (NSCoder coder) : base (coder)
		{
		}

		public ButtonsStackView (NSObjectFlag t) : base (t)
		{
		}

		public ButtonsStackView (IntPtr handle) : base (handle)
		{
		}

		public ButtonsStackView (CGRect frame) : base (frame)
		{
		}

		public IButtonsStackDelegate Delegate { get; set; }

		public void AddButton(UIButton button)
		{
			if (button == null)
				throw new ArgumentNullException ("button");
			buttons.Add (button);
			AddSubview (button);
			button.TouchUpInside += Button_Click;
		}

		public override void WillRemoveSubview (UIView uiview)
		{
			base.WillRemoveSubview (uiview);
			buttons.Remove (uiview as UIButton);
		}

		void Button_Click(object sender, EventArgs args)
		{
			var button = sender as UIButton;
			if (Delegate != null && sender != null) {
				int idx = 0;
				foreach (var b in Subviews) {
					if (b.Handle == button.Handle) {
						Delegate.ActionPerformed (this, idx);
					}
					idx++;
				}
			}
		}

		public void RemoveButton(int idx) 
		{
			var button = Subviews [idx] as UIButton;
			button.RemoveFromSuperview ();
			button.TouchUpInside -= Button_Click;
		}

		public UIButton GetButton (int idx)
		{
			return buttons [idx];
		}
	}
}

