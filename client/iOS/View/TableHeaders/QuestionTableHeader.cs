// TODO: Delete if will not need
using System;
using CoreGraphics;
using LiveOakApp.iOS.View.Skin;
using StudioMobile;
using UIKit;

namespace LiveOakApp.iOS.View.TableHeaders
{
    public class QuestionTableHeader : CustomView
    {
        [View(0)]
        [LabelSkin("QuestionTableHeader")]
        public UITextView TableHeader { get; private set; }

        protected override void CreateView()
        {
            base.CreateView();

            BackgroundColor = new UIColor(0.95f, 0.95f, 0.95f, 1f);
            TableHeader.Text = "long long string, as lorem ipsum, long long text, long long string, as lorem ipsum, long long text, long long string, as lorem ipsum, long long text, long long string, as lorem ipsum, long long text";
            TableHeader.ScrollEnabled = true;
            TableHeader.Editable = false;
            TableHeader.BackgroundColor = UIColor.White;

        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            TableHeader.Frame = this.LayoutBox()
                .Top(0)
                .Bottom(10)
                .Left(0)
                .Right(0);
        }
    }
}

