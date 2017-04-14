using System;
using System.Threading.Tasks;
using Android.Content;
using Android.Util;
using Android.Widget;
using SL4N;
using System.Threading;

namespace StudioMobile
{
	public class RemoteImageView : ImageView
	{
		protected static readonly ILogger LOG = LoggerFactory.GetLogger<RemoteImageView>();

		public RemoteImageView(IntPtr javaReference, Android.Runtime.JniHandleOwnership transfer) : base(javaReference, transfer)
		{
			Initialize();
		}

		public RemoteImageView(Context context) :
			base(context)
		{
			Initialize();
		}

		public RemoteImageView(Context context, IAttributeSet attrs) :
			base(context, attrs)
		{
			Initialize();
		}

		public RemoteImageView(Context context, IAttributeSet attrs, int defStyle) :
			base(context, attrs, defStyle)
		{
			Initialize();
		}

		public event EventHandler<EventArgs> ImageChanged;

		protected void OnImageChanged(EventArgs e)
		{
            if(ImageChanged != null)
                ImageChanged(this, e);
		}

		void Initialize()
		{
		}

		private RemoteImage _image;

		public RemoteImage Image { 
			get
			{
				return _image;
			} 
			set
			{
                if (_image == value)
                    return;

                DisposeImage();

				_image = value;
                if (!_image.IsLoaded)
				{
                       LoadImage().Ignore();
				}					
                else
				{
					SetImageBitmap(_image.Bitmap);
				}
				OnImageChanged(EventArgs.Empty);
			}
		}

        private string imageUrl;
        public string ImageUrl
        {
            get
            {
                return imageUrl;
            }
            set
            {
                if (value == imageUrl)
                    return;
                imageUrl = value;
                if (imageUrl == null)
                    return;
                try 
                {
                    var imageUri = new Uri(imageUrl);
                    Image = new RemoteImage(imageUri);
                } catch (UriFormatException e)
                {
                    LOG.Error(e.Message);
                }
            }
        }

        public CancellationTokenSource LoadImageTokenSource { get; private set; }

        async Task LoadImage()
        {
            try
            {
                await _image.Load(LoadImageTokenSource.Token);
                SetImageBitmap(_image.Bitmap);
            }
            catch (OperationCanceledException)
            {
            }
        }

        public void DisposeImage()
        {
            if (LoadImageTokenSource != null)
            {
                LoadImageTokenSource.Cancel();
            }
            LoadImageTokenSource = new CancellationTokenSource();

            SetImageBitmap(null); // clear image
            if(_image != null && _image.Bitmap.Native != null)
                _image.Bitmap.Dispose();
            _image = null;
        }

    }
}

