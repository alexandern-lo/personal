using System;
using LiveOakApp.iOS.TableSources;
using LiveOakApp.iOS.View.Skin;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.Cells
{
    public class MenuItemCell : CustomTableViewCell
    {
        public const string DefaultCellIdentifier = "MenuItemCell";
        public MainMenuItem MenuItem { get; private set; }

        [View]
        public UIImageView MenuItemIconImageView { get; private set; }

        [View]
        [LabelSkin("NormalRegularWhiteLabel")]
        public UILabel MenuItemTitleLabel { get; private set; }

        [View]
        [LabelSkin("NormalRegularWhiteLabel")]
        public UILabel CounterLabel { get; private set; }

        [View]
        public UIView SeparatorView { get; private set; }

        public MenuItemCell(String cellId) : base(UITableViewCellStyle.Default, cellId)
        {
            BackgroundColor = UIColor.Clear;
            SelectedBackgroundView = new UIView();
            SelectedBackgroundView.BackgroundColor = UIColor.White.ColorWithAlpha(0.2f);
            SeparatorView.BackgroundColor = UIColor.White.ColorWithAlpha(0.15f);
        }

        public void SetupCell(MainMenuItem menuItem)
        {
            MenuItem = menuItem;

            MenuItemTitleLabel.Text = menuItem.Title;
            CounterLabel.Text = "11";
            CounterLabel.Font = Fonts.LargeSemibold;
            MenuItemIconImageView.Image = menuItem.Image;
            SelectionStyle = UITableViewCellSelectionStyle.None;

            var isInboxItem = menuItem.Type == MainMenuItemType.Inbox;
            CounterLabel.Hidden = !isInboxItem;

            var isMyResourcesItem = menuItem.Type == MainMenuItemType.MyResources;
            var isSupportItem = menuItem.Type == MainMenuItemType.Support;
            SeparatorView.Hidden = isMyResourcesItem || isSupportItem;

            MenuItemIconImageView.Hidden = menuItem.Image == null;

            MenuItemTitleLabel.SizeToFit();
            CounterLabel.SizeToFit();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            MenuItemIconImageView.SizeToFit();
            MenuItemIconImageView.Frame = this.LayoutBox()
                                                .Width(MenuItemIconImageView.Bounds.Width)
                                                .Height(MenuItemIconImageView.Bounds.Height)
                                                .Left(20)
                                                .CenterVertically();

            nfloat leftMargin = MenuItemIconImageView.Frame.X + MenuItemIconImageView.Frame.Width + 15;
            nfloat rightMargin = 8.0f;

            CounterLabel.Frame = this.LayoutBox()
                                        .Height(CounterLabel.Frame.Height)
                                        .Width(CounterLabel.Frame.Width)
                                        .Right(20)
                                        .CenterVertically();

            SeparatorView.Frame = this.LayoutBox()
                                        .Height(1)
                                        .Left(leftMargin)
                                        .Right(0)
                                        .Bottom(0);

            MenuItemTitleLabel.Frame = this.LayoutBox()
                                        .Height(MenuItemTitleLabel.Frame.Height)
                                        .Left(leftMargin)
                                        .Right(rightMargin)
                                        .CenterVertically();

        }

        public override void SetHighlighted(bool highlighted, bool animated)
        {
            var color = SeparatorView.BackgroundColor;
            base.SetHighlighted(highlighted, animated);
            SeparatorView.BackgroundColor = color;
        }

        public override void SetSelected(bool selected, bool animated)
        {
            base.SetSelected(selected, animated);
            if (selected)
            {
                UIView backgroundView = new UIView();
                backgroundView.BackgroundColor = UIColor.Black.ColorWithAlpha(0.65f);
                BackgroundView = backgroundView;
                MenuItemIconImageView.Image = MenuItem.ActiveImage;
                MenuItemTitleLabel.Font = Fonts.NormalSemibold;
                MenuItemTitleLabel.TextColor = new UIColor(0.365f, 0.624f, 0.988f, 1.0f);
            }
            else {
                BackgroundView = null;
                MenuItemIconImageView.Image = MenuItem.Image;
                MenuItemTitleLabel.Font = Fonts.NormalRegular;
                MenuItemTitleLabel.TextColor = UIColor.White;
            }
		}

	}
}