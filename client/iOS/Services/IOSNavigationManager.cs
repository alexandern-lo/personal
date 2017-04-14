using System;
using UIKit;
using StudioMobile;
using LiveOakApp.Models.Services;
using LiveOakApp.iOS.Controller;
using Microsoft.Identity.Client;

namespace LiveOakApp.iOS.Services
{
    public class IOSNavigationManager : NavigationManager
    {
        public static new IOSNavigationManager Instance = new IOSNavigationManager();

        public override IPlatformParameters CurrentPlatformParameters()
        {
            var topController = AppDelegate.Shared.Window.RootViewController;
            var navController = topController as UINavigationController;
            if (navController != null)
                topController = navController.ViewControllers.Last();
            while (topController.PresentedViewController != null)
                topController = topController.PresentedViewController;
            return new PlatformParameters(topController);
        }

        protected override RootState GetCurrentState()
        {
            var rootController = AppDelegate.Shared.Window.RootViewController;
            if (rootController == null)
                return RootState.Initial;

            if (rootController is SlideController)
                return RootState.Main;

            var navController = rootController as UINavigationController;
            if (navController != null)
                rootController = navController.ViewControllers.First();

            if (rootController is StartupController)
                return RootState.Startup;
            if (rootController is LoginController)
                return RootState.Login;
            if (rootController is TermsController)
                return RootState.Terms;
            if (rootController is SubscriptionExpiredController)
                return RootState.SubscriptionExpired;

            return RootState.Initial;
        }

        protected override void ChangeState(RootState oldState, RootState newState)
        {
            var animated = oldState != RootState.Initial;
            var animation = UIViewAnimationOptions.TransitionCrossDissolve;
            if (oldState == RootState.Main || newState == RootState.Main)
                animation = UIViewAnimationOptions.TransitionFlipFromTop;
            UIViewController targetController = null;
            switch (newState)
            {
                case RootState.Initial:
                    break;
                case RootState.Main:
                    targetController = MainMenuController.CreateSliderController(AppDelegate.Shared.Window.Bounds);
                    break;
                case RootState.Startup:
                    targetController = new UINavigationController(new StartupController());
                    break;
                case RootState.Login:
                    targetController = new UINavigationController(new LoginController());
                    break;
                case RootState.Terms:
                    targetController = new UINavigationController(new TermsController());
                    break;
                case RootState.SubscriptionExpired:
                    targetController = new UINavigationController(new SubscriptionExpiredController());
                    break;
            }
            if (targetController == null) return;
            ReplaceRootController(targetController, animated, animation);
        }

        void ReplaceRootController(UIViewController controller, bool animated, UIViewAnimationOptions transition = UIViewAnimationOptions.TransitionCrossDissolve)
        {
            Action action = () => { AppDelegate.Shared.Window.RootViewController = controller; };
            if (animated)
            {
                UIView.Transition(AppDelegate.Shared.Window, 0.5f, transition, action, null);
            }
            else
            {
                action();
            }
        }
    }
}
