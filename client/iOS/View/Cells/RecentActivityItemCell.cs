using System;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Models;
using LiveOakApp.Models.Data.Entities;
using LiveOakApp.Models.Services;
using LiveOakApp.Models.ViewModels;
using LiveOakApp.Resources;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.Cells
{
    public class RecentActivityItemCell : CustomTableViewCell
    {
        public const string DefaultCellIdentifier = "RecentActivityItemCell";

        [View(0)]
        public UIImageView AvatarRemoteImageView { get; private set; }

        [View(1)]
        public UIActivityIndicatorView ImageIndicatorView { get; private set; }

        [View(2)]
        [LabelSkin("PersonCellFullNameLabel")]
        public UILabel FullNameLabel { get; private set; }

        [View(3)]
        [LabelSkin("LeadRecentActivityEventTitle")]
        public UILabel EventNameTitle { get; private set; }

        [View(4)]
        [LabelSkin("LeadRecentActivityEventLabel")]
        public UILabel EventNameLabel { get; private set; }

        [View(5)]
        [LabelSkin("SmallRegularBlackLabel")]
        public UILabel PerformedDateLabel { get; private set; }

        [View(6)]
        [LabelSkin("xSmallRegularGrayLabel")]
        public UILabel PerformedActionLabel { get; private set; }

        IScheduledWork photoLoadingWork;

        public RecentActivityItemCell(string cellId = DefaultCellIdentifier) : base(UITableViewCellStyle.Default, cellId)
        {
            AvatarRemoteImageView.Layer.MasksToBounds = true;
            ImageIndicatorView.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.Gray;
            EventNameTitle.Text = L10n.Localize("RecentActivityItemEventTitle", "Event:");
        }

        public void SetupCell(LeadRecentActivityViewModel leadRecentActivity)
        {
            FullNameLabel.Text = leadRecentActivity.FirstName + " " + leadRecentActivity.LastName;
            EventNameLabel.Text = leadRecentActivity.EventName;
            SetPhotoResource(leadRecentActivity.PhotoResource);
            PerformedActionLabel.Text = leadRecentActivity.PerformedAction.ToString();

            if (leadRecentActivity.PerformedAt == null) return;
            if (leadRecentActivity.PerformedAt.GetValueOrDefault().Date == DateTime.Today.Date)
                PerformedDateLabel.Text = L10n.Localize("TodayAtDateTitle", "Today at ") + ServiceLocator.Instance.DateTimeService.TimeToDisplayString(leadRecentActivity.PerformedAt);
            else if (leadRecentActivity.PerformedAt.GetValueOrDefault().Date == DateTime.Today.AddDays(-1))
                PerformedDateLabel.Text = L10n.Localize("YesterdayAtDateTitle", "Yesterday at ") + ServiceLocator.Instance.DateTimeService.TimeToDisplayString(leadRecentActivity.PerformedAt);
            else
                PerformedDateLabel.Text = ServiceLocator.Instance.DateTimeService.DateTimeToDisplayString(leadRecentActivity.PerformedAt);
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
            EventNameLabel.SizeToFit();
            EventNameTitle.SizeToFit();
            PerformedDateLabel.SizeToFit();
            PerformedActionLabel.SizeToFit();

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
            PerformedDateLabel.Frame = this.LayoutBox()
                .Height(PerformedDateLabel.Bounds.Height)
                .Width(PerformedDateLabel.Bounds.Width)
                .Top(pH * 0.105f)
                .Right(10);
            PerformedActionLabel.Frame = this.LayoutBox()
                .Height(PerformedActionLabel.Bounds.Height)
                .Width(PerformedActionLabel.Bounds.Width)
                .Below(PerformedDateLabel, -3)
                .Right(10);
            FullNameLabel.Frame = this.LayoutBox()
                .Height(FullNameLabel.Bounds.Height)
                .After(AvatarRemoteImageView, labelsLeftMargin)
                .Before(PerformedDateLabel, 5)
                .Top(PerformedDateLabel, -1.5f);
            EventNameTitle.Frame = this.LayoutBox()
                .Height(EventNameTitle.Bounds.Height)
                .After(AvatarRemoteImageView, labelsLeftMargin)
                .Width(EventNameTitle.Bounds.Width)
                .Below(FullNameLabel, -3);
            EventNameLabel.Frame = this.LayoutBox()
                .Height(EventNameLabel.Bounds.Height)
                .After(EventNameTitle, 3)
                .Before(PerformedActionLabel, 5)
                .Below(FullNameLabel, -3);

            // avatarDiameter isn't static value
            AvatarRemoteImageView.Layer.CornerRadius = avatarDiameter / 2.0f;
        }

        public static float RowHeight
        {
            get { return 50.0f; }
        }
    }
}
