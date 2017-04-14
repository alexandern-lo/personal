using UIKit;
using Foundation;

namespace StudioMobile
{
	public static class NSMutableStringExtensions
	{
		public static void AppendIcon(this NSMutableAttributedString str, FontIcon icon, UIColor textColor)
		{
			using (var attachment = icon.ToTextAttachment (textColor))
			using (var iconText = NSAttributedString.CreateFrom (attachment)) 
			{
				str.Append (iconText);
			}
		}

		public static void AppendText(this NSMutableAttributedString str, string text)
		{						
			using (var textStr = new NSAttributedString(text)) 
			{
				str.Append (textStr);
			}
		}
	}
	
}
