using System;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using SL4N;

#if __ANDROID__
using Android.App;
[assembly: UsesPermission(Name = "android.permission.INTERNET")]
#endif

namespace LiveOakApp.Models.Services
{
	public class GraphApiService
	{
		static readonly ILogger LOG = LoggerFactory.GetLogger<GraphApiService>();
		public GraphApiService()
		{
			Uri serviceRoot = new Uri(new Uri("https://graph.windows.net"), ApplicationConfig.AuthAuthorityAD);
			ActiveDirectoryClient = new ActiveDirectoryClient(serviceRoot, GetAccessToken);
		}

		public async Task<string> GetAccessToken()
		{
			await Task.Yield();
			return ServiceLocator.Instance.AuthService.GraphToken;
		}

		public async Task<Stream> GetCurrentUserImage()
		{
            //var user = await ActiveDirectoryClient.Me.ExecuteAsync();
            
            return await GetUserImage(null);
        }

		public async Task<Stream> GetUserImage(IUser user)
		{
			try
			{
                //  var streamResponse = await user.ThumbnailPhoto.DownloadAsync();

                // TODO: GraphApi Karyaev
                //HttpClient client = new HttpClient();
                //HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/beta/me/photo/$value");
                //message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", ServiceLocator.Instance.AuthService.GraphToken);
                //HttpResponseMessage response = await client.SendAsync(message);
                //var responseStream = await response.Content.ReadAsStreamAsync();
                
                //if (responseStream.Length > 500)
                //    return responseStream;

                //responseStream.Close();
            }
			catch (Exception ex)
			{
				LOG.Info("Failed to download image: {0}", ex.Message);
			}
            return null;
        }

        ActiveDirectoryClient ActiveDirectoryClient { get; set; }
	}
}

