using System;
using System.Text;
using Foundation;
using UIKit;
using MessageUI;
using StudioMobile;
using LiveOakApp.iOS.View.Content;
using LiveOakApp.Models;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using LiveOakApp.iOS.View.Cells;

namespace LiveOakApp.iOS.Controller.Content
{
    public class MyResourcesController : MenuContentController<MyResourcesView>, IUITableViewDelegate
    {
        ResourcesViewModel ViewModel = new ResourcesViewModel();
        PlainUITableViewBinding<ResourceViewModel> ResourcesBinding { get; set; }
        bool IsChosenEmailSendMode { get; set; }
        string EmailForChosenEmailSendMode { get; set; }

        public MyResourcesController(string email) // constructor for send resources to previously chosen email
        {
            Title = L10n.Localize("MenuMyResources", "My resources");
            EmailForChosenEmailSendMode = email;
            IsChosenEmailSendMode = true;
            SendSelectedResourcesCommand = new Command()
            {
                Action = SendSelectedResourcesAction
            };
            Initialize();
        }

        public MyResourcesController(SlideController slideController) : base(slideController)
        {
            Title = L10n.Localize("MenuMyResources", "My resources");
            IsChosenEmailSendMode = false;
            SendSelectedResourcesCommand = new Command()
            {
                Action = SendButtonClick
            };
            Initialize();
        }

        void Initialize()
        {
            OpenResourceCommand = new Command()
            {
                Action = OpenResourceAction
            };
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            if (IsChosenEmailSendMode)
            {
                NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (sender, e) => DismissViewController(true, null));
            }
            ResourcesBinding = View.GetSectionsBinding(ViewModel.Resources);
            Bindings.Add(ResourcesBinding);
            Bindings.Command(OpenResourceCommand).To(ResourcesBinding.ItemSelectedTarget());
            Bindings.Command(SendSelectedResourcesCommand).To(View.SendButton.ClickTarget());
            Bindings.Property(ViewModel.LoadResourcesCommand, _ => _.IsRunning).UpdateTarget((s) => View.ResourcesFetchRunning = s.Value);
            Bindings.Command(ViewModel.LoadResourcesCommand).To(View.ErrorView.ReloadButton.ClickTarget());

            Bindings.Property(ViewModel.LoadResourcesCommand, _ => _.Error)
                    .Convert(_ => _?.MessageForHuman())
                    .To(View.ErrorView.ErrorMessageLabel.TextProperty());
            Bindings.Property(ViewModel.LoadResourcesCommand, _ => _.IsRunning)
                    .UpdateTarget((source) => UpdateErrorAndWarningViews());
            Bindings.Property(ViewModel.LoadResourcesCommand, _ => _.Error)
                    .UpdateTarget((source) => UpdateErrorAndWarningViews());
            Bindings.Property(ViewModel.Resources, _ => _.Count)
                    .UpdateTarget((source) =>
                    {
                        UpdateErrorAndWarningViews();
                        View.DisableSendButton();
                    });
            ViewModel.LoadResourcesCommand.Execute();
            View.CheckCellSelectionAndSendButtonEnabling();

            View.RefreshControl.AddTarget((sender, e) =>
            {
                View.RefreshControl.EndRefreshing();
                ViewModel.LoadResourcesCommand.Execute();
            }, UIControlEvent.ValueChanged);
        }

        public void UpdateErrorAndWarningViews()
        {
            bool isError = ViewModel.LoadResourcesCommand.Error != null;
            bool isEmptyList = ViewModel.Resources.Count == 0;
            bool fetchRunning = View.ResourcesFetchRunning;

            View.ErrorView.Hidden = !isError || fetchRunning;
            View.MessageView.Hidden = isError || !isEmptyList || fetchRunning;

            if (isError)
            {
                View.ErrorView.ErrorMessageLabel.Text = ViewModel.LoadResourcesCommand.Error.MessageForHuman();
                View.ErrorView.SizeToFit();
            }
            else if (isEmptyList)
            {
                View.MessageView.MessageLabel.Text = L10n.Localize("NoResourcesLabel", "No resources");
                View.MessageView.SizeToFit();
            }
        }

        Command OpenResourceCommand { get; set; }
        void OpenResourceAction(object parameter)
        {
            if (String.IsNullOrWhiteSpace(((ResourceViewModel)parameter).Url)) return;
            var resourceUri = UriExtensions.TryParseWebsiteUri(((ResourceViewModel)parameter).Url);
            var resourceUrl = UriExtensions.ToNSUrl(resourceUri);
            UIApplication.SharedApplication.OpenUrl(resourceUrl);
        }

        public Command SendSelectedResourcesCommand { get; private set; }
        void SendSelectedResourcesAction(object parameter)
        {
            if (View.ResourcesTableView.IndexPathsForSelectedRows?.Length > 0)
            {
                var messageToSend = GetSelectedResourcesUrl();
                presentEmailComposeViewController(new string[] { EmailForChosenEmailSendMode }, L10n.Localize("EmailSubject", "Avend resources"), messageToSend);
            }
        }

        void SendButtonClick(object parameter)
        {
            if (View.ResourcesTableView.IndexPathsForSelectedRows?.Length > 0)
            {
                var alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);
                alert.AddAction(UIAlertAction.Create(L10n.Localize("LeadsAlertAction", "Select lead"), UIAlertActionStyle.Default, (obj) => sendToLeadsActionClick()));
                alert.AddAction(UIAlertAction.Create(L10n.Localize("EmailAlertAction", "Direct E-mail"), UIAlertActionStyle.Default, (obj) => sendToEmailActionClick()));
                alert.AddAction(UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, null));
                PresentViewController(alert, true, null);
            }
        }

        void sendToLeadsActionClick()
        {
            string messageToSend = GetSelectedResourcesUrl();
            var leadsViewController = new LeadsController((LeadViewModel lead) =>
            {
                DismissViewController(true, null);
                var emails = lead.Emails;
                if (emails.Count > 1)
                {
                    var alert = UIAlertController.Create(null, null, UIAlertControllerStyle.ActionSheet);
                    foreach (LeadEmailViewModel email in emails)
                    {
                        alert.AddAction(UIAlertAction.Create(email.EmailDescription, UIAlertActionStyle.Default, (obj) =>
                        {
                            presentEmailComposeViewController(new string[] { email.Email }, L10n.Localize("EmailSubject", "Avend resources"), messageToSend);
                        }));
                    }
                    alert.AddAction(UIAlertAction.Create(L10n.Localize("Cancel", "Cancel"), UIAlertActionStyle.Cancel, null));
                    PresentViewController(alert, true, null);
                }
                else
                {
                    string[] recipients = emails.Count > 0 ? new string[] { emails.First().Email } : null;
                    presentEmailComposeViewController(recipients, L10n.Localize("EmailSubject", "Avend resources"), messageToSend);
                }
            }, () =>
            {
                DismissViewController(true, null);
            });
            var navController = new UINavigationController(leadsViewController);
            PresentViewController(navController, true, null);
        }

        void sendToEmailActionClick()
        {
            var messageToSend = GetSelectedResourcesUrl();
            presentEmailComposeViewController(null, L10n.Localize("EmailSubject", "Avend resources"), messageToSend);
        }

        string GetSelectedResourcesUrl()
        {
            // TODO: move this to ViewModel and add resources names
            // ViewModel.EmailMessageWithResouces(resources);
            var messageToSend = new StringBuilder();
            foreach (NSIndexPath resourceIndexPath in View.ResourcesTableView.IndexPathsForSelectedRows)
            {
                messageToSend.AppendLine(ResourcesBinding.DataSource[resourceIndexPath.Row].Url);
            }
            return messageToSend.ToString();
        }

        void presentEmailComposeViewController(string[] recipients, string subject, string body)
        {
            if (MFMailComposeViewController.CanSendMail)
            {
                var emailController = new MFMailComposeViewController();
                emailController.SetToRecipients(recipients);
                //emailController.SetSubject(L10n.Localize("EmailSubject", "Avend resources"));
                emailController.SetSubject(subject);
                emailController.SetMessageBody(body, false);
                emailController.Finished += (sender, e) =>
                {
                    emailController.DismissViewController(true, null);
                };
                PresentViewController(emailController, true, null);
            }
            else {
                var alert = UIAlertController.Create(null, L10n.Localize("EmailErrorAlert", "Failed to send email. Are there no emails configured?"), UIAlertControllerStyle.Alert);
                alert.AddAction(UIAlertAction.Create(L10n.Localize("Ok", "Ok"), UIAlertActionStyle.Cancel, null));
                PresentViewController(alert, true, null);
            }
        }
    }
}
