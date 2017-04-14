using System;
using Android.OS;
using Android.Views;
using LiveOakApp.Droid.Views;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using MikePhil.Charting.Data;
using StudioMobile;
using Android.Support.V7.App;
using Android.Widget;
using LiveOakApp.Models;
using Java.Net;
using Javax.Net.Ssl;
using System.Net;
using System.Globalization;

namespace LiveOakApp.Droid.Controller
{
    public class DashboardFragment : CustomFragment
    {

        DashboardViewModel model;
        DashboardView view;

        public static DashboardFragment Create()
        {
            var fragment = new DashboardFragment();
            fragment.Title = L10n.Localize("MenuDashboard", "Dashboard");
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            model = new DashboardViewModel();
            ChooseLeadsMenuCommand = new Command()
            {
                Action = ChooseLeadsMenuAction
            };
            ChooseEventsMenuCommand = new Command()
            {
                Action = ChooseEventsMenuAction
            };
            ChooseRecentActivityMenuCommand = new Command()
            {
                Action = ChooseRecentActivityMenuAction
            };
            ChooseResourcesMenuCommand = new Command()
            {
                Action = ChooseResourcesMenuAction
            };
            ShowEventSelectorCommand = new Command()
            {
                Action = ShowEventSelectorAction
            };
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = new DashboardView(inflater.Context);

            view.EditGoalAction = EditGoalAction;
            Bindings.Adapter(view.LeadsTakenList, view.GetEventLeadsTakenAdapter(model.EventsGoals));
            Bindings.Adapter(view.ResourcesList, view.GetResourcesAdapter(model.Resources));
            Bindings.Adapter(view.CostPerLeadList, view.GetEventLeadCostAdapter(model.Events, (float)model.GetMaxEventExpenses()));

            Bindings.Property(model.Events, _ => _.Count)
                    .UpdateTarget((count) => 
            {
                if (count.Value > 0)
                    view.ShowEventsLists();
                else
                    view.HideEventsLists();
            });
            Bindings.Property(model.Resources, _ => _.Count)
                    .UpdateTarget((count) =>
            {
                if (count.Value > 0)
                    view.ShowResourcesList();
                else
                    view.HideResourcesList();
            });

            Bindings.Property(model.LeadsStatistics, _ => _.AllTimeCount)
                    .Convert((number) => number.ToString())
                    .To(view.LeadsTotal.TextProperty());
            Bindings.Property(model.LeadsStatistics, _ => _.LastPeriodCount)
                    .Convert((number) => number.ToString())
                    .To(view.LeadsMonthly.TextProperty());
            Bindings.Property(model.LeadsStatistics, _ => _.LastPeriodGoal)
                    .Convert((number) => new PieEntry(number))
                    .UpdateTarget((entry) =>
            {
                view.LeadStatsDataSet.Values[1] = entry.Value;
                view.LeadStatsChart.NotifyDataSetChanged();
            });
            Bindings.Property(model.LeadsStatistics, _ => _.LastPeriodCount)
                    .Convert((number) => new PieEntry(number == 0 && model.LeadsStatistics.LastPeriodGoal == 0 ? 100 : number))
                    .UpdateTarget((entry) =>
            {
                view.LeadStatsDataSet.Values[0] = entry.Value;
                view.LeadStatsChart.NotifyDataSetChanged();
            });
            Bindings.Property(model.LeadsStatistics, _ => _.LastPeriodCountPercents)
                    .Convert((percentage) => percentage + "%")
                    .To(view.LeadsPeriodPercent.TextProperty());
            Bindings.Property(model.LeadsStatistics, _ => _.LastPeriodGoalPercents)
                    .Convert((percentage) => percentage + "%")
                    .To(view.LeadsGoalPercent.TextProperty());

            Bindings.Property(model.LeadsStatistics.ThisYearExpenses, _ => _.Amount)
                    .Convert((amount) => model.LeadsStatistics.ThisYearExpenses.GetCurrencySymbol() + amount.ToShortNumber())
                    .To(view.YearlyExpenses.TextProperty());
            Bindings.Property(model.LeadsStatistics.ThisYearCostPerLead, _ => _.Amount) 
                    .Convert((amount) => model.LeadsStatistics.ThisYearCostPerLead.GetCurrencySymbol() + amount.ToShortNumber())
                    .To(view.AverageLeadCost.TextProperty());
            
            Bindings.Command(ShowEventSelectorCommand)
                    .To(view.AddExpenseButton.ClickTarget());
            Bindings.Command(ChooseLeadsMenuCommand)
                    .To(view.LeadsTab.ClickTarget());
            Bindings.Command(ChooseEventsMenuCommand)
                    .To(view.EventsTab.ClickTarget());
            Bindings.Command(ChooseResourcesMenuCommand)
                    .To(view.ResourcesTab.ClickTarget());
            Bindings.Command(ChooseRecentActivityMenuCommand)
                    .To(view.RecentActivityTab.ClickTarget());

            Bindings.Property(model.LoadDashboardCommand, _ => _.Error)
                    .Convert((ex) => ex.MessageForHuman())
                    .To(view.ErrorView.MessageProperty)
                    .AfterTargetUpdate((a, b) =>
            {
                if (model.LoadDashboardCommand.Error == null)
                    view.HideError();
                else
                    view.ShowError();
            });

            Bindings.Property(model.LoadDashboardCommand, _ => _.IsRunning)
                    .UpdateTarget((running) =>
            {
                view.DashboardRefresher.Enabled = !running.Value;
                view.IsShowingProgressBar = running.Value;
            });

            Bindings.Command(model.LoadDashboardCommand)
                    .To(view.ErrorView.ActionButtonClickTarget);

            view.DashboardRefresher.Refresh += async (sender, e) =>
            {
                view.DashboardRefresher.Refreshing = false;
                try
                {
                    await model.LoadDashboardCommand.ExecuteAsync();
                }
                catch (Exception ex)
                {
                    LOG.Error(ex.MessageForHuman());
                }
            };

            model.LoadDashboardCommand.Execute();

            return view;
        }

        #region Dialogs

        AlertDialog ShowProgressDialog(string title)
        {
            var dialog = new AlertDialog.Builder(Context)
                                        .SetTitle(title)
                                        .SetView(new ProgressBar(Context))
                                        .SetCancelable(false)
                                        .Create();
            dialog.Show();
            return dialog;
        }

        DashboardEventViewModel targetDashboardEvent;
        void ShowEditGoalDialog()
        {
            var editGoalView = new EditText(Context);
            editGoalView.InputType = Android.Text.InputTypes.ClassNumber;
            new AlertDialog.Builder(Context)
                           .SetTitle(L10n.Localize("ChooseEventGoalAlertTitle", "Choose event goal"))
                           .SetView(editGoalView)
                           .SetPositiveButton(L10n.Localize("Save", "Save"), async (sender, e) =>
            {
                ((AlertDialog)sender).Dismiss();
                uint newGoal;
                if (UInt32.TryParse(editGoalView.Text, out newGoal))
                {
                    
                    var progressDialog = ShowProgressDialog(L10n.Localize("EditGoalRequestAlertTitle", "Edit goal request..."));
                    try 
                    {
                        await targetDashboardEvent.EditEventGoal((int)newGoal);
                    } catch (Exception ex) 
                    {
                        if (!((ex is UnknownHostException) || (ex is SSLException) || (ex is WebException)))
                            throw;
                        progressDialog.Dismiss();
                        ShowConnectionErrorDialog();
                    }
                    progressDialog.Dismiss();
                }
                else
                {
                    ShowGoalErrorDialog();
                }
            })
                           .SetNegativeButton(L10n.Localize("Cancel", "Cancel"), (sender, e) =>
            {
                ((AlertDialog)sender).Dismiss();
            })
                           .Create()
                           .Show();
        }

        EventViewModel targetEvent;
        void ShowAddExpenseDialog()
        {
            var editDescriptionView = new EditText(Context);
            editDescriptionView.Hint = L10n.Localize("AddExpenseDescriptionTextFieldPlaceholder", "Description");
            var editExpenseView = new EditText(Context);
            editExpenseView.Hint = L10n.Localize("AddExpenseTextFieldPlaceholder", "Expense value");
            editExpenseView.InputType = Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberFlagDecimal;
            var verticalLayout = new LinearLayout(Context);
            verticalLayout.Orientation = Orientation.Vertical;
            verticalLayout.AddView(editExpenseView);
            verticalLayout.AddView(editDescriptionView);
            new AlertDialog.Builder(Context)
                           .SetTitle(L10n.Localize("AddExpenseAlertTitle", "New expense"))
                           .SetView(verticalLayout)
                           .SetPositiveButton(L10n.Localize("Add", "Add"), async (sender, e) =>
            {
                ((AlertDialog)sender).Dismiss();
                decimal newExpense;
                if (Decimal.TryParse(editExpenseView.Text, NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out newExpense) && newExpense >= 0)
                {
                    var progressDialog = ShowProgressDialog(L10n.Localize("AddExpenseRequestAlertTitle", "Add expense request..."));
                    try
                    {
                        await model.LeadsStatistics.AddExpense(targetEvent, newExpense, editDescriptionView.Text);
                    }
                    catch (Exception ex)
                    {
                        if (!((ex is UnknownHostException) || (ex is SSLException) || (ex is WebException)))
                            throw;
                        progressDialog.Dismiss();

                    }
                    progressDialog.Dismiss();
                }
                else
                {
                    ShowExpenseErrorDialog();
                }
            })
                           .SetNegativeButton(L10n.Localize("Cancel", "Cancel"), (sender, e) =>
            {
                ((AlertDialog)sender).Dismiss();
            })
                           .Create()
                           .Show();
        }
        void ShowGoalErrorDialog()
        {
            new AlertDialog.Builder(Context)
                           .SetTitle(L10n.Localize("ChooseEventGoalErrorAlert", "Input valid goal number"))
                           .SetPositiveButton(L10n.Localize("Ok", "OK"), (sender, e) => ((AlertDialog)sender).Dismiss())
                           .Create()
                           .Show();
        }

        void ShowExpenseErrorDialog()
        {
            new AlertDialog.Builder(Context)
                           .SetTitle(L10n.Localize("AddExpenseErrorAlert", "Input valid expense value"))
                           .SetPositiveButton(L10n.Localize("Ok", "OK"), (sender, e) => ((AlertDialog)sender).Dismiss())
                           .Create()
                           .Show();
        }
        void ShowConnectionErrorDialog()
        {
            new AlertDialog.Builder(Context)
                           .SetTitle(L10n.Localize("InternetErrorMessage", "Internet connection appears to be offline"))
                           .SetPositiveButton(L10n.Localize("Ok", "OK"), (sender, e) => ((AlertDialog)sender).Dismiss())
                           .Create()
                           .Show();
        }

        #endregion

        #region Actions

        public Command ShowEventSelectorCommand { get; private set; }
        void ShowEventSelectorAction(object arg)
        {
            UiUtil.hideKeyboard(view);
            ShowEventSelectorFragment();
        }
        void ShowEventSelectorFragment()
        {
            var fragment = EventsFragment.CreateForResult(new Command
            {
                Action = (@event) =>
                {
                    targetEvent = @event as EventViewModel;
                    ShowAddExpenseDialog();
                }
            });

            FragmentManager.BeginTransaction()
                          .Replace(Resource.Id.fragment_container, fragment)
                          .AddToBackStack("events-choser-screen")
                          .Commit();
        }

        void EditGoalAction(DashboardEventViewModel eventViewModel)
        {
            targetDashboardEvent = eventViewModel;
            ShowEditGoalDialog();
        }

        public Command ChooseLeadsMenuCommand { get; private set; }
        void ChooseLeadsMenuAction(object arg)
        {
            (Activity as MainActivity).ChangeFragment(LeadsFragment.Create());
        }

        public Command ChooseEventsMenuCommand { get; private set; }
        void ChooseEventsMenuAction(object arg)
        {
            (Activity as MainActivity).ChangeFragment(EventsFragment.Create());
        }

        public Command ChooseRecentActivityMenuCommand { get; private set; }
        void ChooseRecentActivityMenuAction(object arg)
        {
            (Activity as MainActivity).ChangeFragment(RecentFragment.Create());
        }

        public Command ChooseResourcesMenuCommand { get; private set; }
        void ChooseResourcesMenuAction(object arg)
        {
            (Activity as MainActivity).ChangeFragment(MyResourcesFragment.Create());
        }

        #endregion
    }
}

