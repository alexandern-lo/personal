using System.Threading.Tasks;
using Microsoft.Identity.Client;
using StudioMobile;

namespace LiveOakApp.Models.ViewModels
{
    public class LoginViewModel : DataContext
    {
        public const string AuthenticationCanceledErrorCode = "authentication_canceled";
        IPlatformParameters PlatformParameters;

#if __ANDROID__
        public LoginViewModel(Android.App.Activity presentFromActivity)
        {
            Init(new PlatformParameters(presentFromActivity));
        }
#endif

#if __IOS__
        public LoginViewModel(UIKit.UIViewController presentFromCtrl)
        {
            Init(new PlatformParameters(presentFromCtrl));
        }
#endif

        public void Init(IPlatformParameters platformParameters)
        {
            PlatformParameters = platformParameters;

            LoginCommand = new AsyncCommand
            {
                Action = LoginAction,
                CanExecute = CanExecuteLoginAction
            };

            ResetPasswordCommand = new AsyncCommand
            {
                Action = ResetPasswordAction,
                CanExecute = CanExecuteResetPasswordAction
            };
        }

        public bool IsLoggedIn
        {
            get { return ServiceLocator.Instance.AuthService.IsLoggedIn; }
        }

        public User User
        {
            get { return ServiceLocator.Instance.AuthService.CurrentUser; }
        }

        public bool TermsAccepted
        {
            get { return ServiceLocator.Instance.TermsOfUseService.IsAccepted; }
        }

        public bool CanUseSilentLogin
        {
            get { return ServiceLocator.Instance.AuthService.HasCachedTokens && !IsLoggedIn; }
        }

        #region Login

        public AsyncCommand LoginCommand { get; private set; }

        async Task LoginAction(object arg)
        {
            await ServiceLocator.Instance.AuthService.Login(PlatformParameters);
            await ServiceLocator.Instance.ProfileService.GetProfileNoThrow(null);
            RaisePropertyChanged(() => User);
            RaisePropertyChanged(() => IsLoggedIn);
            RaisePropertyChanged(() => TermsAccepted);
        }

        bool CanExecuteLoginAction(object arg)
        {
            return !LoginCommand.IsRunning && !IsLoggedIn;
        }

        public void LaunchLoginFlowAutomaticallyIfNeeded()
        {
            if (ShouldLaunchLoginFlowAutomatically())
            {
                LoginCommand.Execute();
            }
        }

        bool ShouldLaunchLoginFlowAutomatically()
        {
            if (IsLoggedIn)
            {
                return false;
            }
            var loginError = LoginCommand.Error;
            if (loginError == null)
            {
                return true;
            }
            var msalError = loginError as MsalException;
            if (msalError.IsMsalCancelledException())
            {
                return true;
            }
            return false;
        }

        #endregion

        #region Reset Password

        public AsyncCommand ResetPasswordCommand { get; private set; }

        async Task ResetPasswordAction(object arg)
        {
            await ServiceLocator.Instance.AuthService.ResetPassword(PlatformParameters);
            RaisePropertyChanged(() => User);
            RaisePropertyChanged(() => IsLoggedIn);
            RaisePropertyChanged(() => TermsAccepted);
        }

        bool CanExecuteResetPasswordAction(object arg)
        {
            return !ResetPasswordCommand.IsRunning && !IsLoggedIn;
        }

        #endregion
    }
}
