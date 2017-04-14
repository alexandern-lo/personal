using System;
using StudioMobile;
using StudioMobile.FontAwesome;
using UIKit;

namespace LiveOakApp.iOS.View.Skin
{
	public class TextFieldSkinAttribute : DecoratorAttribute
	{
		public TextFieldSkinAttribute(params string[] name) : base(typeof(TextFieldSkin), name)
		{
		}
	}

	public static class TextFieldSkin
	{
		public static void SearchField(UITextField text)
		{
			text.BorderStyle = UITextBorderStyle.RoundedRect;
		}

		public static void TransparentTextField(UITextField textField)
		{
			textField.BackgroundColor = UIColor.Clear;
			textField.TextColor = UIColor.White;
			textField.Font = Fonts.NormalRegular;
		}

		public static void LoginTextField(UITextField textField)
		{
			TransparentTextField(textField);
			textField.SpellCheckingType = UITextSpellCheckingType.No;
			textField.AutocapitalizationType = UITextAutocapitalizationType.None;
			textField.AutocorrectionType = UITextAutocorrectionType.No;
			textField.KeyboardType = UIKeyboardType.EmailAddress;
			textField.ReturnKeyType = UIReturnKeyType.Next;
			textField.KeyboardAppearance = UIKeyboardAppearance.Dark;
		}

		public static void PasswordTextField(UITextField textField)
		{
			TransparentTextField(textField);
			textField.SecureTextEntry = true;
			textField.ReturnKeyType = UIReturnKeyType.Done;
			textField.KeyboardAppearance = UIKeyboardAppearance.Dark;
		}
	}
}

