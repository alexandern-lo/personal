using System;
using CoreAnimation;
using CoreGraphics;
using UIKit;
using StudioMobile;
using LiveOakApp.iOS.View.Cells;
using LiveOakApp.iOS.View.Skin;
using LiveOakApp.iOS.View.TableHeaders;
using LiveOakApp.Models.ViewModels;

namespace LiveOakApp.iOS.View.Content
{
    public class LeadQualifyQuestionView : CustomView
    {
        const int minTextViewHeight = 70;
        const int maxTextViewHeight = 130;
        public LeadDetailsQuestionViewModel Question { get; set; }
        public QuestionTableHeader TableHeader { get; private set; } = new QuestionTableHeader();

        [View(0)]
        public UITableView QuestionTableView { get; private set; }

        [View(1)]
        [CommonSkin("QualifyQuestionTextView")]
        public UITextView QuestionTextView { get; private set; }

        readonly CALayer TableViewSublayer = CALayer.Create();
        readonly CALayer TextViewSublayer = CALayer.Create();

        protected override void CreateView()
        {
            base.CreateView();

            BackgroundColor = new UIColor(0.95f, 0.95f, 0.95f, 1f);
            QuestionTableView.RowHeight = ResourceCell.RowHeight;
            QuestionTableView.TableFooterView = new UIView(new CGRect(0,0,0,0.1f));
            QuestionTableView.RowHeight = QualifyQuestionCell.RowHeight;
            QuestionTableView.Layer.CornerRadius = 6;
            QuestionTableView.SeparatorColor = new UIColor(0.97f, 0.97f, 0.97f, 1f);
            QuestionTableView.SeparatorInset = new UIEdgeInsets(0, 0, 0, 0);
            QuestionTableView.AllowsMultipleSelection = true;

            TableViewSublayer.BackgroundColor = UIColor.Black.CGColor;
            TableViewSublayer.ShadowOffset = new CGSize(0, 1);
            TableViewSublayer.ShadowRadius = 0.5f;
            TableViewSublayer.ShadowColor = Colors.DarkGray.CGColor;
            TableViewSublayer.ShadowOpacity = 0.2f;
            TableViewSublayer.CornerRadius = 7.0f;
            Layer.AddSublayer(TableViewSublayer);
            Layer.AddSublayer(QuestionTableView.Layer);

            TextViewSublayer.BackgroundColor = UIColor.Black.CGColor;
            TextViewSublayer.ShadowOffset = new CGSize(0, 1);
            TextViewSublayer.ShadowRadius = 0.5f;
            TextViewSublayer.ShadowColor = Colors.DarkGray.CGColor;
            TextViewSublayer.ShadowOpacity = 0.2f;
            TextViewSublayer.CornerRadius = 7.0f;
            Layer.AddSublayer(TextViewSublayer);
            Layer.AddSublayer(QuestionTextView.Layer);

        }

        public PlainUITableViewBinding<LeadDetailsAnswerViewModel> GetTableBinding(ObservableList<LeadDetailsAnswerViewModel> answers)
        {
            var tableViewBinding = new PlainUITableViewBinding<LeadDetailsAnswerViewModel>
            {
                DataSource = answers,
                CellFactory = (UITableView tableView, LeadDetailsAnswerViewModel item, int index) =>
                {
                    var cell = tableView.DequeueReusableCell(QualifyQuestionCell.DefaultCellIdentifier) as QualifyQuestionCell;
                    if (cell == null) cell = new QualifyQuestionCell(QualifyQuestionCell.DefaultCellIdentifier);

                    cell.SetupCell(item);
                    return cell;
                },
                TableView = QuestionTableView,
                MultipleSelection = true
            };
            tableViewBinding.PreserveSelection = true;
            return tableViewBinding;
        }

        public override void LayoutSubviews()
        {
            QuestionTextView.SizeToFit();
            var questionHeight = (float)QuestionTextView.Bounds.Height;
            questionHeight = Math.Max(questionHeight, minTextViewHeight);
            questionHeight = Math.Min(questionHeight, maxTextViewHeight);


            QuestionTextView.Frame = this.LayoutBox()
                .Top(0)
                .Height(questionHeight)
                .Left(15)
                .Right(15);


            QuestionTableView.Frame = this.LayoutBox()
                .Below(QuestionTextView, 15)
                .Bottom(2)
                .Left(15)
                .Right(15);

            TableViewSublayer.Frame = this.LayoutBox()
                .Below(QuestionTextView, 18)
                .Bottom(2)
                .Left(15)
                .Right(15);

            TextViewSublayer.Frame = this.LayoutBox()
                .Top(3)
                .Height(questionHeight-3)
                .Left(15)
                .Right(15);
        }
    }
}
