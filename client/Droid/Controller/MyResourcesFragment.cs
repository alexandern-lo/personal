using System.Collections.Generic;
using System.Text;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using LiveOakApp.Droid.Views;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;
using ServiceStack;
using System.Linq;
using LiveOakApp.Models;
using System;

namespace LiveOakApp.Droid.Controller
{
    public class MyResourcesFragment : CustomFragment
    {
        static string EMAIL_KEY = "EMAIL_KEY";
        public static MyResourcesFragment CreateForEmail(string email)
        {
            var fragment = new MyResourcesFragment();
            var args = new Bundle();
            args.PutString(EMAIL_KEY, email);
            fragment.Arguments = args;
            return fragment;
        }

        public static MyResourcesFragment Create()
        {
            var fragment = new MyResourcesFragment();
            var args = Bundle.Empty;
            fragment.Arguments = args;
            return fragment;
        }

        ResourcesViewModel model;
        ResourcesView resourcesView;

        public Command OpenResourceCommand { get; private set; }

        public MyResourcesFragment()
        {
            Title = L10n.Localize("MenuMyResources", "My resources");
            model = new ResourcesViewModel();

            model.LoadResourcesCommand.Execute();
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (Arguments?.GetString(EMAIL_KEY) == null)
                SendSelectedResourcesCommand = new Command
                {
                    Action = SendButtonClick
                };
            else
                SendSelectedResourcesCommand = new Command
                {
                    Action = SendSelectedResourcesAction
                };

            OpenResourceCommand = new Command
            {
                Action = OpenResourceAction
            };
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return new ResourcesView(container.Context);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            resourcesView = (ResourcesView)view;

            var adapter = resourcesView.ResourcesAdapter(model.Resources);
            Bindings.Adapter(resourcesView.ResourcesList, adapter);

            Bindings.Property(model.LoadResourcesCommand, _ => _.IsRunning)
                    .UpdateTarget((running) =>
            {
                resourcesView.ResourcesListRefresher.Enabled = !running.Value;
                resourcesView.IsShowingProgressBar = running.Value;
            });

            Bindings.Command(SendSelectedResourcesCommand).To(resourcesView.SendButton.ClickTarget());

            resourcesView.ResourceChangedAction = CheckSendResourcesEnabled;

            //don't work for unknown reason
            Bindings.Command(OpenResourceCommand)
                    .To(resourcesView.ResourcesList.ItemClickTarget())
                    .ParameterConverter((pos) => adapter[(int)pos]);

            resourcesView.ResourcesListRefresher.Refresh += async (sender, e) =>
            {
                resourcesView.ResourcesListRefresher.Refreshing = false;
                try
                {
                    await model.LoadResourcesCommand.ExecuteAsync();
                } catch (System.Exception ex) 
                {
                    LOG.Error(ex.MessageForHuman());
                }
            };
        }

        public override void OnResume()
        {
            base.OnResume(); 
            CheckSendResourcesEnabled();
        }

        public Command SendSelectedResourcesCommand { get; private set; }

        string GetSelectedResourcesUrl()
        {
            // TODO: move this to ViewModel and add resources names
            // ViewModel.EmailMessageWithResouces(resources);
            var messageToSend = new StringBuilder();
            foreach (ResourceViewModel resource in GetSelectedResources())
                messageToSend.AppendLine(model.GetResourceSharingUrl(resource));
            return messageToSend.ToString();
        }

        List<Guid> GetSelectedResourcesUids() 
        {
            return GetSelectedResources().ConvertAll((input) => input.Uid);
        }

        // TODO refactor
        List<ResourceViewModel> GetSelectedResources()
        {
            List<ResourceViewModel> selected = new List<ResourceViewModel>();
            foreach (ResourceViewModel resource in model.Resources)
            {
                if (resource.Selected)
                    selected.Add(resource);
            }
            return selected;
        }


        void SendButtonClick(object parameter)
        {
            if (GetSelectedResources().Count > 0)
            {
                string[] actions = new string[]
                {
                    L10n.Localize("LeadsAlertAction", "Select lead"),
                    L10n.Localize("EmailAlertAction", "Direct E-mail")
                };
                AlertDialog.Builder builder = new AlertDialog.Builder(Context);
                builder.SetItems(actions, (sender, e) =>
                {
                    switch (e.Which)
                    {
                        case 0:
                            SendToLeadsActionClick();
                            break;
                        case 1:
                            SendToEmailActionClick();
                            break;
                    }

                });
                builder.Create().Show();
            }
        }

        void SendSelectedResourcesAction(object parameter)
        {
            model.TrackResourceSentCommand.Execute(GetSelectedResources());
            doShare(Context, new string[] { Arguments.GetString(EMAIL_KEY) }, GetSelectedResourcesUrl());
        }

        void SendToLeadsActionClick()
        {
            FragmentManager.BeginTransaction()
                           .Replace(Resource.Id.fragment_container, 
                                    LeadsFragment.CreateForLeadEmails(GetSelectedResourcesUids(), GetSelectedResourcesUrl()))
                           .AddToBackStack("lead-choser-screen")
                           .Commit();
        }

        void SendToEmailActionClick()
        {
            model.TrackResourceSentCommand.Execute(GetSelectedResources());
            doShare(Context, new string[] { }, GetSelectedResourcesUrl());
        }

        void CheckSendResourcesEnabled()
        {
            Android.Util.Log.Error("11", "Checking send button " + model.Resources.Any(_ => _.Selected));
            resourcesView.SendButton.Enabled = model.Resources.Any(_ => _.Selected);
        }

        void OpenResourceAction(object arg)
        {
            var resource = arg as ResourceViewModel;
            if (resource.Url.IsNullOrEmpty()) return;
            var intent = new Intent(Intent.ActionView);
            intent.SetData(Android.Net.Uri.Parse(resource.Url));
            Context.StartActivity(intent);
        }

        public static void doShare(Context context, string[] recipients, string body)
        {
            try
            {
                var intent = new Intent(Intent.ActionSend);
                intent.SetType("message/rfc822");
                intent.PutExtra(Intent.ExtraSubject, L10n.Localize("EmailSubject", "Avend resources"));
                intent.PutExtra(Intent.ExtraText, body);
                intent.PutExtra(Intent.ExtraEmail, recipients);
                context.StartActivity(intent);
            }
            catch (ActivityNotFoundException)
            {
                try
                {
                    var intent = new Intent(Intent.ActionSend);
                    intent.PutExtra(Intent.ExtraSubject, L10n.Localize("EmailSubject", "Avend resources"));
                    intent.PutExtra(Intent.ExtraText, body);
                    intent.PutExtra(Intent.ExtraEmail, recipients);
                    context.StartActivity(intent);
                }
                catch (ActivityNotFoundException)
                {
                    //should never happen
                    Toast.MakeText(context, "Can't share", ToastLength.Short).Show();
                }
            }
        }
    }
}

