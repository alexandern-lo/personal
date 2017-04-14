using System.Threading.Tasks;
using System.ComponentModel;
using StudioMobile;
using LiveOakApp.Models.Services;

namespace LiveOakApp.Models.ViewModels
{
    public class CachableCommandViewModel<T> : AsyncCommandBase where T : new()
    {
        CachableRequest<T> Request { get; set; }

        public T Result { get { return Request.Result; } }

        public bool IsRunningWithoutData { get { return IsRunning && !Request.DataIsLoadedToCache; } }

        public CachableCommandViewModel(CachableRequest<T> request)
        {
            Request = request;
            PropertyChanged += OnIsRunningChanged;
        }

        void OnIsRunningChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsRunning")
            {
                RaisePropertyChanged(() => IsRunningWithoutData);
            }
        }

        protected override async Task DoExecuteAsync(object parameter)
        {
            var loadedFromCache = await Request.LoadFromCache();
            if (loadedFromCache)
            {
                RaisePropertyChanged(() => Result);
                RaisePropertyChanged(() => IsRunningWithoutData);
            }
            var loadedFromNetwork = await Request.LoadFromNetwork(Token);
            if (loadedFromNetwork)
            {
                RaisePropertyChanged(() => Result);
            }
        }

        protected override bool CheckCanExecute(object parameter)
        {
            return !IsRunning;
        }
    }
}
