using System;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Net.Http;

namespace StudioMobile
{
	public class RemoteImage 
	{
		public RemoteImage()
		{
			IsLoaded = false;
		}

		public RemoteImage(Uri uri) : this()
		{
			if (uri == null)
				throw new ArgumentNullException ();
			Uri = uri;
			ImageDownloader = null;
		}

		public RemoteImage(Uri uri, Image img) : this(uri)
		{
			IsLoaded = true;
			Bitmap = img;
		}

		public RemoteImage(Func<Task<Stream>> imageDownloader) 
		{
			if (imageDownloader == null)
				throw new ArgumentNullException();
			ImageDownloader = imageDownloader;
			Uri = null;
		}

		public Uri Uri { get; set; }
		public bool IsLoaded { get; private set; }
		private Func<Task<Stream>> ImageDownloader;

		public Image Bitmap { get; private set; }

		public async Task<bool> Load()
		{
			return await Load (CancellationToken.None);
		}

		public async Task<bool> Load(CancellationToken token)
		{
			Stream stream = null;
			if (Uri != null)
			{
                var httpClient = CreateHttpClient();
				using (var response = await httpClient.GetAsync(Uri, token))
				{
					if (response.IsSuccessStatusCode)
					{
						stream = await response.Content.ReadAsStreamAsync();
					}
				}
			}
			else {
				stream = await ImageDownloader();
			}
			return await Setup(stream, token);
		}

		private async Task<bool> Setup(Stream stream, CancellationToken token)
		{
			if (stream == null)
			{
				return false;
			}
			var buffer = new MemoryStream();
			await stream.CopyToAsync(buffer, 4096, token);
			buffer.Position = 0;
			Bitmap = await Image.LoadFromStream(buffer, token);
			IsLoaded = true;
			return true;
		}

        public static Func<HttpClient> HttpClientBuilder;

        HttpClient CreateHttpClient()
        {
            if (HttpClientBuilder != null)
            {
                return HttpClientBuilder();
            }
            return new HttpClient();
        }
	}
	
}
