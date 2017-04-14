
using System;

using Android.Content;
using Android.Runtime;
using Android.Util;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Views;
using FFImageLoading.Work;
using LiveOakApp.Models.Data.Entities;

namespace LiveOakApp.Droid.CustomViews
{
    public class CustomImageView : ImageViewAsync
    {
        string DataLocationUri;
        TaskParameter task;
        bool isLoading = false;
        public IScheduledWork ScheduledWork;
        ITransformation transformation;

        public CustomImageView(Context context) :
            base(context)
        {
            Initialize();
        }

        public CustomImageView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public CustomImageView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        public CustomImageView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Initialize();
        }

        void Initialize()
        {
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            if (w != 0 && h != 0 && !isLoading && task != null)
            {
                ScheduledWork = doLoad();
            }
        }

        public void LoadByUrl(string url)
        {
            task = ImageService.Instance.LoadUrl(url);
            DataLocationUri = url;
            transformation = new CircleTransformation();
            if (Width != 0 && Height != 0)
                ScheduledWork = doLoad();
        }

        public void LoadByPath(string path)
        {
            DataLocationUri = path;
            task = ImageService.Instance.LoadFile(path);
            transformation = new CircleTransformation();
            if (Width != 0 && Height != 0)
                ScheduledWork = doLoad();
        }

        public void LoadByResource(FileResource res, ITransformation transformation)
        {
            this.transformation = transformation;
            if (string.IsNullOrEmpty(res.AbsoluteLocalPath))
            {
                task = ImageService.Instance.LoadUrl(res.RemoteUrl);
            }
            else {
                task = ImageService.Instance.LoadFile(res.AbsoluteLocalPath);
            }
            if (Width != 0 && Height != 0)
                ScheduledWork = doLoad();
        }

        public void LoadByResource(FileResource res)
        {
            LoadByResource(res, null);
        }

        private IScheduledWork doLoad()
        {
            //Loading placeholder with animated drawable not showing by some reason
            SetImageResource(Resource.Drawable.progress_animation);
            //return null;
            TaskParameter param = task?
                .DownSample(Width, Height);
            if (task != null && transformation != null)
                param = param.Transform(transformation);
            return param?
                .ErrorPlaceholder("new_placeholder", ImageSource.CompiledResource)
                .Into(this);
        }
    }
}
