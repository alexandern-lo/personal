using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using LiveOakApp.iOS.TableSources;
using LiveOakApp.iOS.View.Content;
using LiveOakApp.iOS.View.TableFooters;
using LiveOakApp.Models;
using LiveOakApp.Models.Services;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.Controller.Content
{
    public class DashboardController : MenuContentController<DashboardView>
    {
        DashboardViewModel ViewModel = new DashboardViewModel();
        UIAlertController ProgressAlert = UIAlertController.Create(null, null, UIAlertControllerStyle.Alert);
        UIActivityIndicatorView ProgressIndicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
        UIAlertController CompletionAlert = UIAlertController.Create(null, null, UIAlertControllerStyle.Alert);

        public DashboardController(SlideController slideController) : base(slideController)
        {
            Title = L10n.Localize("MenuDashboard", "Dashboard");

            ChooseLeadsMenuCommand = new Command()
            {
                Action = ChooseLeadsMenuAction
            };
            ChooseEventsMenuCommand = new Command()
            {
                Action = ChooseEventsMenuAction
            };
            ChooseResourcesMenuCommand = new Command()
            {
                Action = ChooseResourcesMenuAction
            };
            ChooseRecentActivityMenuCommand = new Command()
            {
                Action = ChooseRecentActivityMenuAction
            };
            AddExpenseButtonClickCommand = new Command()
            {
                Action = AddExpenseButtonClickAction
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ViewModel.LoadDashboardCommand.Execute();
            View.SetupTableHeaderAndFooter(ViewModel.LeadsStatistics);
            var tableBinding = View.GetSectionsBinding(ViewModel.AllTableBindingData, ViewModel.GetMaxEventExpenses, GoalEditingButtonClickAction);
            Bindings.Add(tableBinding);
            Bindings.Property(ViewModel.LoadDashboardCommand, _ => _.IsRunning).UpdateTarget((source) =>
            {
                View.DashboardFetchRunning = source.Value;
                View.RefreshControl.Subviews[0].Subviews[0].Hidden = source.Value;
                UpdateErrorAndWarningViews();
                if (source.Value == false) View.SetupTableHeaderAndFooter(ViewModel.LeadsStatistics);
            });
            Bindings.Command(ChooseLeadsMenuCommand).To(View.LeadsTabBarButton.ClickTarget());
            Bindings.Command(ChooseEventsMenuCommand).To(View.EventsTabBarButton.ClickTarget());
            Bindings.Command(ChooseResourcesMenuCommand).To(View.ResourcesTabBarButton.ClickTarget());
            Bindings.Command(ChooseRecentActivityMenuCommand).To(View.RecentActivityTabBarButton.ClickTarget());
            Bindings.Command(AddExpenseButtonClickCommand).To(((DashboardTableFooterView)View.DashboardTableView.TableFooterView)
                                                              .AddExpenseButton.ClickTarget());
            Bindings.Property(ViewModel, _ => _.LeadsStatistics).UpdateTarget((source) => View.SetupTableHeaderAndFooter(ViewModel.LeadsStatistics));
            Bindings.Command(ViewModel.LoadDashboardCommand).To(View.ErrorView.ReloadButton.ClickTarget());
            Bindings.Property(ViewModel.LoadDashboardCommand, _ => _.Error)
                    .Convert(_ => _?.MessageForHuman())
                    .To(View.ErrorView.ErrorMessageLabel.TextProperty());
            Bindings.Property(ViewModel.LoadDashboardCommand, _ => _.Error)
                    .UpdateTarget((source) => UpdateErrorAndWarningViews());

            View.RefreshControl.AddTarget((sender, e) =>
            {
                View.RefreshControl.EndRefreshing();
                ViewModel.LoadDashboardCommand.Execute();
            }, UIControlEvent.ValueChanged);

            ProgressIndicator.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
            var alertBounds = ProgressAlert.View.Bounds;
            alertBounds.Y += 15;
            ProgressIndicator.Frame = alertBounds;
            ProgressAlert.View.AddSubview(ProgressIndicator);
            var completionAlertOk = UIAlertAction.Create(L10n.Localize("Ok", "Ok"), UIAlertActionStyle.Cancel, null);
            CompletionAlert.AddAction(completionAlertOk);
        }

        public void UpdateErrorAndWarningViews()
        {
            bool isError = ViewModel.LoadDashboardCommand.Error != null;
            bool fetchRunning = View.DashboardFetchRunning;

            View.ErrorView.Hidden = !isError || fetchRunning;

            if (isError)
            {
                View.ErrorView.ErrorMessageLabel.Text = ViewModel.LoadDashboardCommand.Error.MessageForHuman();
                View.ErrorView.SizeToFit();
            }
        }

        void GoalEditingButtonClickAction(DashboardEventViewModel dashboardEvent)
        {
            UIAlertController alert = UIAlertController.Create(L10n.Localize("ChooseEventGoalAlertTitle", "Choose event goal"), null, UIAlertControllerStyle.Alert);
            alert.AddTextField((UITextField textField) =>
            {
                textField.Text = dashboardEvent.LeadsGoal.ToString();
                textField.KeyboardType = UIKeyboardType.NumberPad;
            });

            UIAlertAction alertActionCancel = UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, null);
            UIAlertAction alertActionSave = UIAlertAction.Create(L10n.Localize("Save", "Save"), UIAlertActionStyle.Default, async (alertAction) =>
              {
                  uint newGoal;
                  if (UInt32.TryParse(alert.TextFields[0].Text, out newGoal) && newGoal < Int32.MaxValue)
                  {
                      if (dashboardEvent.LeadsGoal == newGoal) return;
                      ShowProgressAlert(L10n.Localize("EditGoalRequestAlertTitle", "Edit goal request.."));
                      try
                      {
                          await dashboardEvent.EditEventGoal((int)newGoal);
                      }
                      catch (Exception e)
                      {
                          DismissProgressAlertAndShowCompletionAlert(L10n.Localize("Error", "Error"), e.MessageForHuman());
                          return;
                      }
                      DismissProgressAlertAndShowCompletionAlert(L10n.Localize("Success", "Success"), null);
                      ViewModel.LoadDashboardCommand.Execute();
                  }
                  else {
                      alert.DismissViewController(true, () => { });
                      UIAlertController errorAlert = UIAlertController.Create(L10n.Localize("Error", "Error"), L10n.Localize("ChooseEventGoalErrorAlert", "Input valid goal number"), UIAlertControllerStyle.Alert);
                      UIAlertAction alertActionOk = UIAlertAction.Create(L10n.Localize("Ok", "Ok"), UIAlertActionStyle.Cancel, null);
                      errorAlert.AddAction(alertActionOk);
                      PresentViewController(errorAlert, true, null);
                  };
              });
            alert.AddAction(alertActionCancel);
            alert.AddAction(alertActionSave);
            PresentViewController(alert, true, null);
        }

        public Command AddExpenseButtonClickCommand { get; private set; }
        void AddExpenseButtonClickAction(object arg)
        {
            EventsController eventsController = null;
            eventsController = new EventsController((EventViewModel @event) =>
            {
                eventsController.NavigationController.PopViewController(true);

                UIAlertController alert = UIAlertController.Create(L10n.Localize("AddExpenseAlertTitle", "New expense"), null, UIAlertControllerStyle.Alert);
                alert.AddTextField((UITextField textField) =>
                {
                    textField.Placeholder = L10n.Localize("AddExpenseTextFieldPlaceholder", "Expense value");
                    textField.KeyboardType = UIKeyboardType.DecimalPad;
                });
                alert.AddTextField((UITextField textField) => textField.Placeholder = L10n.Localize("AddExpenseDescriptionTextFieldPlaceholder", "Description"));

                UIAlertAction alertActionCancel = UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, null);
                UIAlertAction alertActionAdd = UIAlertAction.Create(L10n.Localize("Add", "Add"), UIAlertActionStyle.Default, async (alertAction) =>
                 {
                     decimal newExpense;
                     if (Decimal.TryParse(alert.TextFields[0].Text, out newExpense) && newExpense >= 0)
                     {
                         ShowProgressAlert(L10n.Localize("AddExpenseRequestAlertTitle", "Add expense request.."));
                         try
                         {
                             await ViewModel.LeadsStatistics.AddExpense(@event, newExpense, alert.TextFields[1].Text);
                         }
                         catch (Exception e)
                         {
                             DismissProgressAlertAndShowCompletionAlert(L10n.Localize("Error", "Error"), e.MessageForHuman());
                             return;
                         }
                         DismissProgressAlertAndShowCompletionAlert(L10n.Localize("Success", "Success"), null);
                         await ViewModel.LoadDashboardCommand.ExecuteAsync();
                     }
                     else {
                         alert.DismissViewController(true, () => { });
                         UIAlertController errorAlert = UIAlertController.Create(L10n.Localize("Error", "Error"), L10n.Localize("AddExpenseErrorAlert", "Input valid expense value"), UIAlertControllerStyle.Alert);
                         UIAlertAction alertActionOk = UIAlertAction.Create(L10n.Localize("Ok", "Ok"), UIAlertActionStyle.Cancel, null);
                         errorAlert.AddAction(alertActionOk);
                         PresentViewController(errorAlert, true, null);
                     };
                 });
                alert.AddAction(alertActionCancel);
                alert.AddAction(alertActionAdd);
                PresentViewController(alert, true, null);
            });
            NavigationController.PushViewController(eventsController, true);
        }

        void ShowProgressAlert(string title)
        {
            ProgressAlert.Title = title + "\n";
            ProgressAlert.Message = "\n\n";
            ProgressIndicator.StartAnimating();
            PresentViewController(ProgressAlert, true, null);
        }

        void DismissProgressAlertAndShowCompletionAlert(string title, string message)
        {
            ProgressIndicator.StopAnimating();
            CompletionAlert.Message = message;
            CompletionAlert.Title = title;
            ProgressAlert.DismissViewController(true, () => PresentViewController(CompletionAlert, true, null));
        }


        public Command ChooseLeadsMenuCommand { get; private set; }
        void ChooseLeadsMenuAction(object arg)
        {
            ChooseMenuItem(MainMenuItemType.Leads);
        }

        public Command ChooseEventsMenuCommand { get; private set; }
        void ChooseEventsMenuAction(object arg)
        {
            ChooseMenuItem(MainMenuItemType.Events);
        }

        public Command ChooseResourcesMenuCommand { get; private set; }
        void ChooseResourcesMenuAction(object arg)
        {
            ChooseMenuItem(MainMenuItemType.MyResources);
        }

        public Command ChooseRecentActivityMenuCommand { get; private set; }
        void ChooseRecentActivityMenuAction(object arg)
        {
            ChooseMenuItem(MainMenuItemType.RecentActivity);
        }
    }
}

