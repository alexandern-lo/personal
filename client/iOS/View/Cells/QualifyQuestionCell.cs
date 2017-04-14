using UIKit;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.Models.ViewModels;
using StudioMobile;

namespace LiveOakApp.iOS.View.Cells
{
    public class QualifyQuestionCell : CustomTableViewCell
    {
        public const string DefaultCellIdentifier = "QualifyQuestionCell";

        public QualifyQuestionCell(string cellId) : base(UITableViewCellStyle.Default, cellId)
        {
            BackgroundColor = UIColor.White;
            TextLabel.Font = Fonts.NormalRegular;
            SelectedBackgroundView = new UIView();
            SelectedBackgroundView.BackgroundColor = UIColor.Clear;
            LayoutMargins = UIEdgeInsets.Zero;
            PreservesSuperviewLayoutMargins = false;
        }

        public void SetupCell(LeadDetailsAnswerViewModel question)
        {
            TextLabel.Text = question.Answer;
        }

        public override void SetSelected(bool selected, bool animated)
        {
            base.SetSelected(selected, animated);
            if (selected)
            {
                TextLabel.Font = Fonts.NormalSemibold;
                TextLabel.TextColor = UIColor.White;
                BackgroundColor = new UIColor(0.365f, 0.624f, 0.988f, 1.0f);
            }
            else {
                TextLabel.Font = Fonts.NormalRegular;
                TextLabel.TextColor = Colors.DarkGray;
                BackgroundColor = UIColor.White;
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

        }

        public static float RowHeight
        {
            get { return 55.0f; }
        }
    }
}

