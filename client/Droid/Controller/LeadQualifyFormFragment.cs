using Android.OS;
using Android.Views;
using LiveOakApp.Droid.Views;
using LiveOakApp.Models.ViewModels;
using StudioMobile;

namespace LiveOakApp.Droid.Controller
{
    public class LeadQualifyFormFragment : CustomFragment
    {
        QualifyTabView view;
        LeadDetailsViewModel model;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            model = ((LeadFragment)ParentFragment).ViewModel;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = new QualifyTabView(inflater.Context);

            Bindings.Adapter(view.QuestionsPager, view.GetQuestionsAdapter(model.Questions));

            Bindings.Adapter(view.StatePager, view.GetStatesAdapter(model.States));

            Bindings.Property(model.Questions, _ => _.Count)
                    .To(view.QuestionsCountProperty());

            Bindings.Property(model, _ => _.LeadNotes)
                    .To(view.NotesView.TextProperty());

            Bindings.Property(model, _ => _.CurrentState)
                    .To(view.StatePager.SelectedPagePositionProperty());

            return view;
        }
    }
}
