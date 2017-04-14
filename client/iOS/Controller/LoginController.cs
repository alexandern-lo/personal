using StudioMobile;
using LiveOakApp.Models;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.iOS.View;
using LiveOakApp.iOS.Services;
using Microsoft.Identity.Client;
using Foundation;
using CoreText;
using UIKit;
using LiveOakApp.Models.Services;
using LiveOakApp.iOS.View.Skin;

namespace LiveOakApp.iOS.Controller
{
    public class LoginController : CustomController<LoginView>
    {
        LoginViewModel ViewModel;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            NavigationController.SetNavigationBarHidden(true, false);

            ViewModel = new LoginViewModel(this);

            Bindings.Command(ViewModel.LoginCommand)
                .To(View.LoginButton.ClickTarget())
                .WhenFinished((t, c) =>
                {
                    NavigateNext();
                    ViewModel.LaunchLoginFlowAutomaticallyIfNeeded();
                });

            Bindings.Property(ViewModel.LoginCommand, _ => _.IsRunning)
                .UpdateTarget((s) => { View.LoginRunning = s.Value; });

            Bindings.Property(ViewModel.LoginCommand, _ => _.Error).UpdateTarget((source) =>
            {
                if (source.Value is MsalException && ((MsalException)source.Value).IsMsalUserDontExistException())
                {
                    var messageString = source.Value.MessageForHuman();
                    var paragraph = new NSMutableParagraphStyle();
                    var linkText = "Avend web portal";
                    paragraph.Alignment = UITextAlignment.Center;
                    var attributeString = new NSMutableAttributedString(messageString, Fonts.LargeRegular, UIColor.Red, UIColor.Clear, UIColor.Red, paragraph);
                    attributeString.AddAttribute(UIStringAttributeKey.Link, new NSUrl(ApplicationConfig.WebLoginUrl), 
                                                 new NSRange(messageString.IndexOf(linkText, System.StringComparison.CurrentCulture), linkText.Length));
                    View.ErrorLabel.AttributedText = attributeString;
                    return;
                }
                View.ErrorLabel.Text = source.Value.MessageForHuman();
            });

            Bindings.Command(ViewModel.ResetPasswordCommand)
                .To(View.ResetPasswordButton.ClickTarget())
                .WhenFinished((t, c) => NavigateNext());

            Bindings.Property(ViewModel.ResetPasswordCommand, _ => _.IsRunning)
                .UpdateTarget((s) => { View.ResetPasswordRunning = s.Value; });

            Bindings.Property(ViewModel.ResetPasswordCommand, _ => _.Error)
                .Convert(_ => _?.MessageForHuman())
                .To(View.ResetPasswordErrorLabel.TextProperty());

            NavigateNext();
            ViewModel.LaunchLoginFlowAutomaticallyIfNeeded();
        }

        void NavigateNext()
        {
            IOSNavigationManager.Instance.NavigateToRequiredStateIfNeeded();
        }
    }
}
