using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using UIKit;
using StudioMobile;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Models.Data.Entities;
using System;

namespace LiveOakApp.iOS.View.Cells
{
    public class PersonCell : CustomTableViewCell
    {
        public const string DefaultCellIdentifier = "PersonCell";

        [View(0)]
        public UIImageView AvatarRemoteImageView { get; private set; }

        [View(1)]
        public UIActivityIndicatorView ImageIndicatorView { get; private set; }

        [View(2)]
        [LabelSkin("PersonCellFullNameLabel")]
        public UILabel FullNameLabel { get; private set; }

        [View(3)]
        [LabelSkin("PersonCellCompanyLabel")]
        public UILabel DetailsLabel { get; private set; }

        IScheduledWork photoLoadingWork;

        public PersonCell(string cellId = DefaultCellIdentifier) : base(UITableViewCellStyle.Default, cellId)
        {
            AvatarRemoteImageView.Layer.MasksToBounds = true;
            ImageIndicatorView.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;
        }

        public void SetupCell(FileResource photo, string fullName, string details)
        {
            FullNameLabel.Text = fullName;
            DetailsLabel.Text = details;
            SetPhotoResource(photo);
        }

        void SetPhotoResource(FileResource photo)
        {
            ImageIndicatorView.Hidden = false;
            ImageIndicatorView.StartAnimating();
            if (photoLoadingWork != null)
            {
                photoLoadingWork.Cancel();
                photoLoadingWork = null;
            }
            var localPath = photo?.AbsoluteLocalPath;
            var remoteUrl = photo?.RemoteUrl;
            TaskParameter loadingSource = null;
            if (!String.IsNullOrWhiteSpace(remoteUrl))
            {
                loadingSource = ImageService.Instance.LoadUrl(remoteUrl);
            }
            else if (!String.IsNullOrWhiteSpace(localPath))
            {
                loadingSource = ImageService.Instance.LoadFile(localPath);
            }
            if (loadingSource != null)
            {
                photoLoadingWork = loadingSource
                        .DownSample((int)AvatarRemoteImageView.Bounds.Width, (int)AvatarRemoteImageView.Bounds.Height)
                        .ErrorPlaceholder("user-default-image", ImageSource.CompiledResource)
                        .Transform(new CircleTransformation())
                        .Finish((obj) =>
                        {
                            InvokeOnMainThread(() =>
                            {
                                ImageIndicatorView.Hidden = true;
                                ImageIndicatorView.StopAnimating();
                            });
                        })
                        .Into(AvatarRemoteImageView);
                return;
            }
            AvatarRemoteImageView.Image = UIImage.FromBundle("user-default-image");
            ImageIndicatorView.Hidden = true;
            ImageIndicatorView.StopAnimating();
        }

        public override void PrepareForReuse()
        {
            base.PrepareForReuse();
            if (photoLoadingWork != null)
            {
                photoLoadingWork.Cancel();
                photoLoadingWork = null;
            }
            AvatarRemoteImageView.Image = null;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            var pH = Bounds.Height;
            var pW = Bounds.Width;
            var labelsLeftMargin = pW * 0.031f;
            var avatarDiameter = pH * 0.8f;

            FullNameLabel.SizeToFit();
            DetailsLabel.SizeToFit();

            AvatarRemoteImageView.Frame = this.LayoutBox()
                .Width(avatarDiameter)
                .Height(avatarDiameter)
                .Left(pW * 0.04f)
                .CenterVertically();
            ImageIndicatorView.Frame = this.LayoutBox()
                .Width(avatarDiameter)
                .Height(avatarDiameter)
                .Left(pW * 0.04f)
                .CenterVertically();
            FullNameLabel.Frame = this.LayoutBox()
                .Height(FullNameLabel.Bounds.Height)
                .After(AvatarRemoteImageView, labelsLeftMargin)
                .Right(5.0f)
                .Top(pH * 0.10f);
            DetailsLabel.Frame = this.LayoutBox()
                .Height(DetailsLabel.Bounds.Height)
                .After(AvatarRemoteImageView, labelsLeftMargin)
                .Right(5.0f)
                .Top(pH * 0.46f);

            // avatarDiameter isn't static value
            AvatarRemoteImageView.Layer.CornerRadius = avatarDiameter / 2.0f;
        }

        public static float RowHeight
        {
            get { return 50.0f; }
        }
    }
}
