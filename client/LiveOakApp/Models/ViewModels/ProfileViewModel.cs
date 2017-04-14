using StudioMobile;

namespace LiveOakApp.Models.ViewModels
{
    public class ProfileViewModel : DataContext
    {
        public string UserFullName
        {
            get
            {
                return ServiceLocator.Instance.AuthService.CurrentUser.FullName;
            }
        }

        public RemoteImage UserAvatar
        {
            get
            {
                return new RemoteImage(ServiceLocator.Instance.GraphApiService.GetCurrentUserImage);
            }
        }
    }
}
