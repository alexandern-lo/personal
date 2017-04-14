using System;
using System.Threading.Tasks;
using ServiceStack;
using StudioMobile;
using LiveOakApp.Models.Services;
using LiveOakApp.Models.Data.NetworkDTO;

namespace LiveOakApp.Models.ViewModels
{
    public class TermsOfUseViewModel : DataContext
    {
        readonly TermsOfUseService service;

        public TermsOfUseViewModel()
        {
            service = ServiceLocator.Instance.TermsOfUseService;
            AcceptCommand = new AsyncCommand
            {
                Action = AcceptAction,
                CanExecute = CanExecuteAcceptAction
            };
            DeclineCommand = new AsyncCommand
            {
                Action = DeclineAction,
                CanExecute = CanExecuteDeclineAction
            };
            LoadTermsCommand = new CachableCommandViewModel<TermsOfUseDTO>(service.TermsRequest);

            Bindings.Property(LoadTermsCommand, _ => _.Result).UpdateTarget((a) =>
            {
                LOG.Debug("result updated: {0}", a.Value);
                RaisePropertyChanged(() => Terms);
                RaisePropertyChanged(() => IsAccepted);
                RaisePropertyChanged(() => HasTermsErrorOccured);
                AcceptCommand.RaiseCanExecuteChanged();
            });
            Bindings.Property(AcceptCommand, _ => _.IsRunning).UpdateTarget((a) =>
            {
                DeclineCommand.RaiseCanExecuteChanged();
            });
            Bindings.Property(DeclineCommand, _ => _.IsRunning).UpdateTarget((a) =>
            {
                AcceptCommand.RaiseCanExecuteChanged();
            });
            Bindings.Bind();
        }

        public bool IsAccepted
        {
            get { return service.IsAccepted; }
        }

        public bool HasTermsErrorOccured
        {
            get { return Terms.Equals(EmptyTerms) || LoadTermsCommand.Error != null; }
        }

        public bool HasAcceptErrorOccured 
        {
            get { return AcceptCommand.Error != null; }
        }

        const string EmptyTerms = "";

        public string Terms
        {
            get { return service.Terms?.Text ?? EmptyTerms; }
        }

        public CachableCommandViewModel<TermsOfUseDTO> LoadTermsCommand { get; private set; }

        public AsyncCommand AcceptCommand { get; private set; }

        async Task AcceptAction(object args)
        {
            await service.Accept(service.Terms, AcceptCommand.Token);
            RaisePropertyChanged(() => IsAccepted);
            RaisePropertyChanged(() => HasAcceptErrorOccured);
        }

        bool CanExecuteAcceptAction(object arg)
        {
            // TODO: disallow accepting if terms not scrolled to the bottom
            return !DeclineCommand.IsRunning && !AcceptCommand.IsRunning && !Terms.IsEmpty();
        }

        public AsyncCommand DeclineCommand { get; private set; }

        async Task DeclineAction(object args)
        {
            await LoadTermsCommand.CancelCommand.ExecuteAsync(null);
            await service.Decline();
        }

        bool CanExecuteDeclineAction(object arg)
        {
            return !DeclineCommand.IsRunning && !AcceptCommand.IsRunning;
        }
    }
}
